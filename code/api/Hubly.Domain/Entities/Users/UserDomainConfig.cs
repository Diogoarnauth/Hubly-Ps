namespace Hubly.api.Domain.Entities;

public class UsersDomainConfig
{
   
    public int MaxTokensPerUser { get; init; } = 3;
    public int MinUsernameLength { get; init; } = 3;
    public int MinPasswordLength { get; init; } = 8;

    // Construtor para validar as regras ao criar a config
    public UsersDomainConfig()
    {
        // Adicionar requires se necessário
    }
}