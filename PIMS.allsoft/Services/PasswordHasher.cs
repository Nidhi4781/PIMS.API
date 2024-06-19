using PIMS.allsoft.Interfaces;
using System.Security.Cryptography;

namespace PIMS.allsoft.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 128 / 8;
        private const int KeySize = 258 / 8;
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName _hashAlgorithmNAme=HashAlgorithmName.SHA256;
        private static char Delimiter=';';
        string IPasswordHasher.Hash(string password)
        {
            var salt=RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithmNAme, KeySize);
return string.Join(Delimiter, Convert.ToBase64String(salt),Convert.ToBase64String(hash));
           // throw new NotImplementedException();
        }

        bool IPasswordHasher.verify(string passwordHash, string inputPassword)
        {
            var elements=passwordHash.Split(Delimiter);
            var salt = Convert.FromBase64String(elements[0]);
            var hash = Convert.FromBase64String(elements[1]);
            var hashInput = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, Iterations, _hashAlgorithmNAme, KeySize);
           return CryptographicOperations.FixedTimeEquals(hash, hashInput);
            // throw new NotImplementedException();
        }
    }
}
