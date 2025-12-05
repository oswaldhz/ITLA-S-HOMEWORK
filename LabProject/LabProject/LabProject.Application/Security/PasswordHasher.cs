using System.Security.Cryptography;
using System.Text;

namespace LabProject.Application.Security;

public static class PasswordHasher
{
    public static string Hash(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToBase64String(hashBytes);
    }

    public static bool Verify(string password, string hash)
    {
        return Hash(password) == hash;
    }
}
