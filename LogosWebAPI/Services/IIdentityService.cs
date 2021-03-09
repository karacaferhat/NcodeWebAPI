using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NCodeWebAPI.Controllers.v1.Responses;
using NCodeWebAPI.Domain;

namespace NCodeWebAPI.Services
{
    public interface IIdentityService
    {
        Task<AuthanticationResult> RegisterAsync(string email, string password, string city, DateTime dateOfBirth, string instruments,string name,string surname );
        Task<AuthanticationResult> LoginAsync(string email, string password);
        Task<AuthanticationResult> RefreshTokenAsync(string token, string refreshToken);

        Task<UserInfoUpdateResponse> UpdateUserInfoAsync(string email,  string city, DateTime dateOfBirth, string instruments, string name, string surname);
        Task<ResetPasResponse> ResetAsync(string email);

        Task<ChangePasResponse> ChangeAsync(string email, string p_old, string p_new);
    }
}
