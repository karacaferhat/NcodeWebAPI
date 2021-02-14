using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NCodeWebAPI.Domain;

namespace NCodeWebAPI.Services
{
    public interface IIdentityService
    {
        Task<AuthanticationResult> RegisterAsync(string email, string password, string city, DateTime dateOfBirth, string instruments);
        Task<AuthanticationResult> LoginAsync(string email, string password);
        Task<AuthanticationResult> RefreshTokenAsync(string token, string refreshToken);
    }
}
