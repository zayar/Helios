using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Helios.Web.Models {
    public class User {
        public int Id { get; private set; }

        [Required]
        [RegularExpression(@"[\w-]+")]
        public string UserName { get; set; }
        
        public bool IsAdmin { get; set; }

        public bool RequiredToChangePassword { get; set; }

        [Required]
        public string PasswordDigest { get; private set; }

        [Required]
        public string Salt { get; private set; }

        public void SetPassword(string password) {
            this.Salt = Guid.NewGuid().ToString("N");

            this.PasswordDigest = GenerateSaltedHash(password ?? "", this.Salt);
        }

        public bool VerifyPassword(string password) {
            var hash = GenerateSaltedHash(password, this.Salt);
            return hash == this.PasswordDigest;
        }

        static string GenerateSaltedHash(string plainPassword, string plainSalt) {            
            var password = Encoding.UTF8.GetBytes(plainPassword);
            var salt = Encoding.UTF8.GetBytes(plainSalt);

            HashAlgorithm algorithm = new SHA256Managed();
            var passwordWithSalt = new byte[password.Length + salt.Length];

            for (int i = 0; i < password.Length; i++) {
                passwordWithSalt[i] = password[i];
            }
            for (int i = 0; i < salt.Length; i++) {
                passwordWithSalt[password.Length + i] = salt[i];
            }

            byte[] hash = algorithm.ComputeHash(passwordWithSalt);

            return Convert.ToBase64String(hash);
        }
    }
}