using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Interfaces
{
    public interface ICreatorRepository
    {
        Task<bool> RegisterCreator(Creator newCreator); 
        Task<bool> ExistsByUserId(int userId);
        Task<Creator?> GetByUserId(int userId);
        Task<Creator?> UpdateStatus(int userId, string newStatus);
        Task<bool> UpdateRating(Creator creator);
        Task<bool> IncrementChatsStarted(int userId);
        Task<bool> IncrementChatsResponded(int userId);
        Task<Creator?> GetByUserIdSocialProfiles(int userId);
        Task<List<Sector>> GetSectorByName(List<string> sectorName);
        Task<List<Sector>> GetAllSectors();
        Task<CreatorRating?> GetUserRating(int userId, int creatorId);
        Task<Creator?> Edit(int user_id, string artisticName);
        Task<bool> HasUserRatedCreator(int userId, int creatorId);
        Task AddRating(CreatorRating rating);

        
        

        
        // Task<IEnumerable<Creator>> GetTopRatedCreators(int count);
        // Task<IEnumerable<Creator>> GetMostActiveCreators(int count);
        // get top content creators
        // get most contracted creators by content
        // ...
    }

}