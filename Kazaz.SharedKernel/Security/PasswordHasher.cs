using Microsoft.AspNetCore.Identity;

namespace Kazaz.SharedKernel.Security;

public static class PasswordHasher
{
    private static readonly PasswordHasher<object> _hasher = new();

    public static string Hash(string senha)
        => _hasher.HashPassword(null!, senha);

    public static bool Verify(string senha, string hash)
        => _hasher.VerifyHashedPassword(null!, hash, senha)
           == PasswordVerificationResult.Success;
}
