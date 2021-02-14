using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NCodeWebAPI.Data;
using NCodeWebAPI.Domain;
using NCodeWebAPI.Options;

namespace NCodeWebAPI.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _context;
        
        public IdentityService(UserManager<IdentityUser> userManager,JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters,DataContext dataContext)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _context = dataContext;
        }
            
        public async Task<AuthanticationResult> RegisterAsync(string email, string password, string city, DateTime dateOfBith, string instrument)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "User with this email already exists" }
                };

            }
            var newUser = new IdentityUser
            {
                Email = email,
                UserName = email,

            };
            var createdUser = await _userManager.CreateAsync(newUser, password);

            if (!createdUser.Succeeded)
            {
                return new AuthanticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }
          
            return await GenerateAuthanticationResultForUserAync(newUser);
            


        }

        private async Task <AuthanticationResult> GenerateAuthanticationResultForUserAync(IdentityUser user)
        {
            
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),                    
                    new Claim("id",user.Id),
                }),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };            
            SecurityToken  token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)

            };
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();



            return new AuthanticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
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

            return await GenerateAuthanticationResultForUserAync(user);
          

        }

        public async Task<AuthanticationResult> RefreshTokenAsync(string token, string refreshToken)
        {

            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken==null) {
                return new AuthanticationResult { Errors = new[] { "Invalid Token" } };
            }
            var expiryDateUnix=
                long.Parse(validatedToken.Claims.Single(x=>x.Type== JwtRegisteredClaimNames.Exp).Value);

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
            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken==null)
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

            if ( storedRefreshToken.Invalidated)
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

            if (storedRefreshToken.JwtId!=jti)
            {
                return new AuthanticationResult
                {
                    Errors = new[] { "This refresh token do not match JWT" }
                };
            }
            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthanticationResultForUserAync(user);


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
