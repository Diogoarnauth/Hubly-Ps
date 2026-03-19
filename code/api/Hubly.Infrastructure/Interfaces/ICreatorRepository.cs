using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces
{
    public interface ICreatorRepository
    {
        Task<bool> RegisterCreator(Creator newCreator); 
        Task<bool> ExistsByUserId(int userId);
        Task<Creator?> GetByUserId(int userId);
        Task<Creator?> UpdateStatus(int userId, string newStatus);
        
        // Task EditProfile(int userId, string artisticName, string availabilityStatus);        
        // Task<IEnumerable<Creator>> GetTopRatedCreators(int count);
        // Task<IEnumerable<Creator>> GetMostActiveCreators(int count);
        // get top content creators
        // get most contracted creators by content
        // ...
    }

}