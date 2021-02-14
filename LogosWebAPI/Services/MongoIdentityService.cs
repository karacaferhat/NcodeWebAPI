using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        private readonly IMongoCollection<ApplicationUser> _userCollection;

        public MongoIdentityService(UserManager<ApplicationUser> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters, IMongoCollection<RefreshToken> tokenCollection, IMongoCollection<ApplicationUser> userCollection)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _tokenCollection = tokenCollection;
            _userCollection = userCollection;

        }
        public async Task<AuthanticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "User does not exists" }
                };

            }
            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!userHasValidPassword)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "Username or password  error" }
                };

            }
            

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthanticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
            {
                return new AuthanticationResult { Errors = new[] { "Invalid Token" } };
            }
            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);



            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "This Token has not expired yet" }
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
                    Errors = new[] { "This refresh token does not exists" }
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

        public async Task<AuthanticationResult> RegisterAsync(string email, string password, string city, DateTime dateOfBirth,string instrument)
        {

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "User with this email already exists" }
                };

            }
            var newUser = new ApplicationUser()
            {
                Email = email,
                UserName = email,
                City = city,
                DateOfBirth = dateOfBirth,
                Instrument = instrument

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
                RefreshToken = refreshToken.Token
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
