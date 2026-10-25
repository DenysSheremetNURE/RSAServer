using RSAGen;
using System.Diagnostics;

namespace RSAServer.Services;

public interface IRsaKeyProvider
{
    RsaPublicKey PublicKey { get; }
    RsaPrivateKey PrivateKey { get; }
}

public sealed class RsaKeyProvider : IRsaKeyProvider
{
    private readonly RsaKeyPair _keys;

    public RsaKeyProvider(ILogger<RsaKeyProvider> logger)
    {
        var sw = Stopwatch.StartNew();
        _keys = RSAKey.GenerateKeys(2048);

        logger.LogInformation(
            "RSA keys generated in {Ms} ms. N: {Bits} bits, E: {E}",
            sw.ElapsedMilliseconds, _keys.N.GetBitLength(), _keys.E);
    }

    public RsaPublicKey PublicKey => _keys.Public;
    public RsaPrivateKey PrivateKey => _keys.Private;
}
