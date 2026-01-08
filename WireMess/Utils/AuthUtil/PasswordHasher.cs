using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WireMess.Utils.AuthUtil.Interfaces;

namespace WireMess.Utils.AuthUtil
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 128 / 8; // 16 bytes (128 bits)
        private const int HashSize = 256 / 8; // 32 bytes (256 bits)
        private const int IterationCount = 100000; // OWASP recommended

        public (string Hash, string Salt) CreateHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: HashSize);

            return (
                Hash: Convert.ToBase64String(hash),
                Salt: Convert.ToBase64String(salt));
        }

        public bool VerifyHash(string password, string hash, string salt)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;
            if (string.IsNullOrWhiteSpace(hash) || string.IsNullOrWhiteSpace(salt))
                return false;
            try
            {
                byte[] hashBytes = Convert.FromBase64String(hash);
                byte[] saltBytes = Convert.FromBase64String(salt);

                if (hashBytes.Length != HashSize || saltBytes.Length != SaltSize)
                    return false;
                byte[] computedHash = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: saltBytes,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: IterationCount,
                    numBytesRequested: HashSize);

                return CryptographicOperations.FixedTimeEquals(hashBytes, computedHash);
            }
            catch(FormatException)
            {
                return false;
            }
        }
    }
}
