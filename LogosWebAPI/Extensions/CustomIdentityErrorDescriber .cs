using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Extensions
{
  
        public class CustomIdentityErrorDescriber : IdentityErrorDescriber
        {
            public override IdentityError DefaultError() { return new IdentityError { Code = nameof(DefaultError), Description = $"Bilinmeyen hata oluştu." }; }
            public override IdentityError ConcurrencyFailure() { return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "Optimistic concurrency failure, object has been modified." }; }
            public override IdentityError PasswordMismatch() { return new IdentityError { Code = nameof(PasswordMismatch), Description = "Hatalı şifre" }; }
            public override IdentityError InvalidToken() { return new IdentityError { Code = nameof(InvalidToken), Description = "Hatalı token." }; }
            public override IdentityError LoginAlreadyAssociated() { return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "Kullanıcı zaten mevcut" }; }
            public override IdentityError InvalidUserName(string userName) { return new IdentityError { Code = nameof(InvalidUserName), Description = $"Kullanıcı '{userName}' sadece harfler ve sayıları içermelidir " }; }
            public override IdentityError InvalidEmail(string email) { return new IdentityError { Code = nameof(InvalidEmail), Description = $"Email '{email}' geçersiz." }; }
            public override IdentityError DuplicateUserName(string userName) { return new IdentityError { Code = nameof(DuplicateUserName), Description = $"Kullanıcı adı '{userName}' zaten kullanımda." }; }
            public override IdentityError DuplicateEmail(string email) { return new IdentityError { Code = nameof(DuplicateEmail), Description = $"Email '{email}' zaten kullanımda." }; }
            public override IdentityError InvalidRoleName(string role) { return new IdentityError { Code = nameof(InvalidRoleName), Description = $"Role name '{role}' is invalid." }; }
            public override IdentityError DuplicateRoleName(string role) { return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"Role name '{role}' is already taken." }; }
            public override IdentityError UserAlreadyHasPassword() { return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "Kullanıcı şifresi zaten kaydedilmiş." }; }
            public override IdentityError UserLockoutNotEnabled() { return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "Lockout işlemi bu kullanıcı için uygun değil." }; }
            public override IdentityError UserAlreadyInRole(string role) { return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"User already in role '{role}'." }; }
            public override IdentityError UserNotInRole(string role) { return new IdentityError { Code = nameof(UserNotInRole), Description = $"User is not in role '{role}'." }; }
            public override IdentityError PasswordTooShort(int length) { return new IdentityError { Code = nameof(PasswordTooShort), Description = $"Şifre en az {length} uzunlukta olmalıdır." }; }
            public override IdentityError PasswordRequiresNonAlphanumeric() { return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Şifre en az bir  işaret  (örnek: /*&%+'!) içermelidir" }; }
            public override IdentityError PasswordRequiresDigit() { return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "Şifre en az bir karakter sayısal değer içermelidir ('0'-'9')." }; }
            public override IdentityError PasswordRequiresLower() { return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "Şifre en az bir karakter küçük harf içermelidir ('a'-'z')." }; }
            public override IdentityError PasswordRequiresUpper() { return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "Şifre en az bir karakter büyük harf içermelidir  ('A'-'Z')." }; }
        }
    
}
