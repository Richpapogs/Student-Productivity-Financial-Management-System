using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SP_FMS
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);

            string hash = Convert.ToBase64String(
                new Rfc2898DeriveBytes(password, salt, 10000).GetBytes(32)
            );

            return Convert.ToBase64String(salt) + ":" + hash;
        }

        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);

            string hashedInput = Convert.ToBase64String(
                new Rfc2898DeriveBytes(inputPassword, salt, 10000).GetBytes(32)
            );

            return hashedInput == parts[1];
        }
    }
}
