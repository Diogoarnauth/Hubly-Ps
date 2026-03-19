namespace Hubly.api.Domain.Entities;

public class CreatorsDomainConfig
{
   
    public int MinArtitisticNameLength { get; init; } = 2;

    // Construtor para validar as regras ao criar a config
    public CreatorsDomainConfig()
    {
        // Adicionar requires se necessário
    }
}