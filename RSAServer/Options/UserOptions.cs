namespace RSAServer.Options;

public sealed class UsersOptions
{
    public const string SectionName = "Users";

    public Dictionary<string, string> Accounts { get; init; } = new();
}
