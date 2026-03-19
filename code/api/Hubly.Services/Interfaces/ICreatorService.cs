using Hubly.api.Domain.Entities;
using Hubly.api.Services.Problems;
using OneOf;

namespace Hubly.api.Services.Interfaces
{

    public interface ICreatorService
    {
        Task<OneOf<Creator, CreatorError>> Register(int userId, string ArtisticName);
        Task<OneOf<Creator, CreatorError>> UpdateStatus(int userId, string newStatus);
    }

}   
