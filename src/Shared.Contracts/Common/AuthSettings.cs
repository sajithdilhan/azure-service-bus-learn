namespace Shared.Common;

public sealed class AuthSettings(string issuer, string audience, string secretKey)
{
    public string Issuer { get; private set; } = issuer ?? throw new ArgumentNullException(nameof(issuer));
    public string Audience { get; private set; } = audience ?? throw new ArgumentNullException(nameof(audience));
    public string SecretKey { get; set; } = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
}
