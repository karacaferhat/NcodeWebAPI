using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NCodeWebAPI.Controllers.v1.Responses;
using NCodeWebAPI.Domain;
using NCodeWebAPI.Options;

namespace NCodeWebAPI.Services
{
    public class MongoIdentityService : IIdentityService

    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IMongoCollection<RefreshToken> _tokenCollection;
        private readonly IMailService _mailService;
        private readonly SmtpSettings _smtpSettings;


        public MongoIdentityService(UserManager<ApplicationUser> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters, IMongoCollection<RefreshToken> tokenCollection, IMailService mailService)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _tokenCollection = tokenCollection;
            _mailService = mailService;
        
        }

        public async Task<ChangePasResponse> ChangeAsync(string email, string p_old, string p_new)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ChangePasResponse
                {
                    IsSuccesfull = false,
                    Errors = new[] { "Kullanıcı bulunamadı" }
                };

            }

            var changedUser= await _userManager.ChangePasswordAsync(user,p_old,p_new);

            if (!changedUser.Succeeded)
            {
                return new ChangePasResponse
                {
                    Errors = changedUser.Errors.Select(x => x.Description)
                };
            }

            return new ChangePasResponse
            {
                IsSuccesfull = true
                
            };
        }

        public async Task<AuthanticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "Kullanıcı bulunamadı" }
                };

            }
            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!userHasValidPassword)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "Kullanıcı adı yada şifre hatalı" }
                };

            }
            

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthanticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
            {
                return new AuthanticationResult { Errors = new[] { "Geçersiz Token" } };
            }
            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);



            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "Token aktif" }
                };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
           var findTask= await _tokenCollection.FindAsync(x => x.Token == refreshToken); //_context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);
            var list = await findTask.ToListAsync();
            var storedRefreshToken = list.FirstOrDefault();

            

            if (storedRefreshToken == null)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "Refresh token does not exists" }
                };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "This refresh token has expired" }
                };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "This refresh token has been invalidated" }
                };
            }

            if (storedRefreshToken.Used)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "This refresh token has been used" }
                };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "This refresh token do not match JWT" }
                };
            }
            storedRefreshToken.Used = true;
            // _context.RefreshTokens.Update(storedRefreshToken);
            await _tokenCollection.FindOneAndReplaceAsync(x => x.Token == refreshToken, storedRefreshToken);
            


            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthanticationResult> RegisterAsync(string email, string password, string city, DateTime dateOfBirth,string instrument,string name,string surname)
        {
            
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "Bu email adresine sahip kullancı zaten mevcut" }
                };

            }
            var newUser = new ApplicationUser()
            {
                Email = email,
                UserName = email,
                City = city,
                DateOfBirth = dateOfBirth,
                Instrument = instrument,
                Name = name,
                Surname = surname,
                Profile = "F"

            };
            var createdUser = await _userManager.CreateAsync(newUser, password);
            
            if (!createdUser.Succeeded)
            {
                return new AuthanticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }

            return await GenerateAuthenticationResultForUserAsync(newUser);

        }

        public async Task<ResetPasResponse> ResetAsync(string email)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                return new ResetPasResponse
                {
                    IsSuccesfull = false,
                    Errors = new[] { "Bu email adresine sahip kullancı bulunamadı" }
                };

            }
            var resetPassToken =   _userManager.GeneratePasswordResetTokenAsync(existingUser).Result;
            var newpass="Logs2!" + DateTime.Now.ToString("mmss");
            var body =  newpass;

   
           


            var identityRes= await _userManager.ResetPasswordAsync(existingUser, resetPassToken, newpass);
           if (!identityRes.Succeeded)
           {
               return new ResetPasResponse
               {
                   IsSuccesfull = false,
                   Errors = identityRes.Errors.Select(x => x.Description) 

               };
            }

           var mailresponse = await _mailService.SendMailAsync(email, "Şifre Sıfırlama", newpass);

           if (!mailresponse.Success)
           {
               return new ResetPasResponse
               {
                   IsSuccesfull = false,
                   Errors = new[] { mailresponse.Error }
               };
           }


            return new ResetPasResponse
               {
                   IsSuccesfull = true,
                   Errors=new[] { body }

               };


        }

        public async Task<UserInfoUpdateResponse> UpdateUserInfoAsync(string email, string city, DateTime dateOfBirth, string instrument, string name, string surname)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new UserInfoUpdateResponse
                {
                    IsSuccesfull = false,
                    Errors = new[] { "Kullanıcı bulunamadı" }
                };

            }

            user.City = city;
            user.Instrument = instrument;
            user.Name = name;
            user.Surname = surname;
            user.DateOfBirth = dateOfBirth;

            await _userManager.UpdateAsync(user);
            return new UserInfoUpdateResponse
            {
                IsSuccesfull = true,
               userInfo = new UserInfo()
               {
                   Name = user.Name,
                   Surname= user.Surname,
                   City = user.City,
                   Instrument= user.Instrument,
                   DateOfBirth = user.DateOfBirth,
                   EMail= user.Email,
                   Profile = user.Profile
               }
            };

        }

        private async Task<AuthanticationResult> GenerateAuthenticationResultForUserAsync(ApplicationUser user)
        {

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim("id",user.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id.ToString(),
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString()
                
            };
            await _tokenCollection.InsertOneAsync(refreshToken);





            return new AuthanticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
                userInfo = new UserInfo()
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    Profile = user.Profile,
                    City = user.City,
                    DateOfBirth = user.DateOfBirth,
                    Instrument = user.Instrument,
                    EMail = user.Email
                }
            };
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }
        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);


        }
    }
}
