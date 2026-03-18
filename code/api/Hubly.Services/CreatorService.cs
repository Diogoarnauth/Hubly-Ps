using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using OneOf;
using System.Data.Common;
using System.Linq;
using Microsoft.Extensions.Configuration;


namespace Hubly.api.Services
{
    public class CreatorService: ICreatorService
    {
        private readonly ITransactionManager _transactionManager;
        //private readonly UsersDomain _usersDomain; corrigir depois

    public CreatorService(
        ITransactionManager transactionManager,
        IConfiguration configuration//,
        //UsersDomain usersDomain
    )
    {
        _transactionManager = transactionManager;
        //_usersDomain = usersDomain;
        
        }

    public async Task<OneOf<Creator, CreatorError>> Register(int userId, string artisticName)
{
    // Validação de Domínio única agora
    
    /*if (!_creatorsDomain.IsValidArtisticName(artisticName)) 
        return new CreatorError.InvalidArtisticName();*/

    return await _transactionManager.Run<OneOf<Creator, CreatorError>>(async (context) =>
    {
        // Verificações cruzadas
        if (await context.CreatorRepository.ExistsByUserId(userId)) 
            return new CreatorError.CreatorAlreadyExists();

        if (await context.CompanyRepository.ExistsByUserId(userId)) 
            return new CreatorError.UserAlreadyRegisteredAsCompany();

        var newCreator = new Creator 
        {
            Id = userId,
            ArtisticName = artisticName,
            // Os outros campos (rating, counts) são inicializados 
            // pelos defaults da BD ou pelos valores padrão da classe.
            IsVerified = false,
            AvailabilityStatus = "AVAILABLE",
            GlobalRating = 0,
            RatingsCount = 0,
            ChatsStartedCount = 0,
            ChatsRespondedCount = 0
        };

        await context.CreatorRepository.RegisterCreator(newCreator);
        
        return newCreator; 
    });
}
}
}