using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RSAGen;
using RSAServer.Options;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static RSAGen.DTOs;

namespace RSAServer.Services;

public enum LoginStatus { Success, InvalidCredentials, MalformedPayload }

public sealed record LoginResult(LoginStatus Status, string? Login = null);

public interface ILoginService
{
    LoginResult Login(EncryptedLoginDto request);
}

public sealed class LoginService(
    IRsaKeyProvider keys,
    IOptions<UsersOptions> users,
    ILogger<LoginService> logger) : ILoginService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public LoginResult Login(EncryptedLoginDto request)
    {
        if (!TryDecrypt(request, out var creds))
            return new LoginResult(LoginStatus.MalformedPayload);

        bool valid = users.Value.Accounts.TryGetValue(creds.Login, out var password)
                     && password == creds.Password;

        return valid
            ? new LoginResult(LoginStatus.Success, creds.Login)
            : new LoginResult(LoginStatus.InvalidCredentials, creds.Login);
    }

    private bool TryDecrypt(EncryptedLoginDto request, [NotNullWhen(true)] out LoginRequest? creds)
    {
        creds = null;
        try
        {
            logger.LogInformation("Received {Count} encrypted block(s)", request.Blocks.Length);

            var blocks = request.Blocks.Select(b => b.FromBase64());
            string json = Encoding.UTF8.GetString(RSAKey.Decrypt(blocks, keys.PrivateKey));

            // DEMO ONLY
            logger.LogInformation("Decrypted payload: {Json}", json);

            creds = JsonSerializer.Deserialize<LoginRequest>(json, JsonOptions);
            return creds is { Login: not null, Password: not null };
        }
        catch (Exception ex) when (ex is CryptographicException or JsonException or FormatException)
        {
            logger.LogWarning("Failed to decrypt payload: {Error}", ex.Message);
            return false;
        }
    }
}