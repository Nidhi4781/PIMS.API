using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace PIMS.allsoft.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool verify(string passwordHash, string inputPassword);
    }
}
