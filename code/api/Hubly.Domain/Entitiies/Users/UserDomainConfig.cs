namespace Hubly.Domain.Entities;

public class UsersDomainConfig
{
    public int TokenSizeInBytes { get; init; } = 32;
    public TimeSpan TokenTtl { get; init; } = TimeSpan.FromHours(24);
    public TimeSpan TokenRollingTtl { get; init; } = TimeSpan.FromHours(1);
    public int MaxTokensPerUser { get; init; } = 3;
    public int MinUsernameLength { get; init; } = 3;
    public int MinPasswordLength { get; init; } = 8;

    // Construtor para validar as regras ao criar a config
    public UsersDomainConfig()
    {
        if (TokenSizeInBytes <= 0) throw new ArgumentException("Token size must be positive");
        // Adicionar os outros 'requires' que tinhas no Kotlin aqui
    }
}