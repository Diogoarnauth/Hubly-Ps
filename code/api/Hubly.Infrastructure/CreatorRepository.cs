using Microsoft.EntityFrameworkCore;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using System.Text.Json;

namespace Hubly.api.Infrastructure
{
    public class CreatorRepository : ICreatorRepository
    {
        private readonly HublyDbContext _context;

        public CreatorRepository(HublyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterCreator(Creator newCreator)
        {
            await _context.Creators.AddAsync(newCreator);
            return true;
        }

        public async Task<bool> ExistsByUserId(int userId)
        {
            return await _context.Creators.AnyAsync(c => c.Id == userId);
        }

        public async Task<Creator?> GetByUserId(int userId)
        {
            return await _context.Creators
                .FirstOrDefaultAsync(c => c.Id == userId);
        }

        public async Task<Creator?> UpdateStatus(int userId, string newStatus)
        {
            var creator = await _context.Creators.FindAsync(userId);
            if (creator == null) return null;
            creator.AvailabilityStatus = newStatus;
            _context.Creators.Update(creator);
            await _context.SaveChangesAsync();
            return creator;
        }

        public async Task<bool> UpdateRating(Creator creator)
        {
            _context.Creators.Update(creator);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IncrementChatsStarted(int userId)
        {
            var creator = await _context.Creators.FindAsync(userId);
            if (creator == null) return false;

            creator.ChatsStartedCount++;

            _context.Creators.Update(creator);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IncrementChatsResponded(int userId)
        {
            var creator = await _context.Creators.FindAsync(userId);
            if (creator == null) return false;

            creator.ChatsRespondedCount++;

            _context.Creators.Update(creator);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Creator?> GetByUserIdSocialProfiles(int userId)
        {
            return await _context.Creators
            .Include(c => c.SocialProfiles)
                .ThenInclude(csp => csp.Platform)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == userId);
        }


        //About Sector
        public async Task<List<Sector>> GetSectorByName(List<string> sectorName)
        {
            return await _context.Sectors
                .Where(s => sectorName.Contains(s.SectorName))
                .ToListAsync();
        }

        public async Task<List<Sector>> GetAllSectors()
        {
            return await _context.Sectors.ToListAsync();
        }

        public async Task<CreatorRating?> GetUserRating(int userId, int creatorId)
        {
            return await _context.CreatorRatings
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.EvaluatorId == userId && r.TargetCreatorId == creatorId);
        }


        public async Task<Creator?> Edit(int user_id, string artisticName)
        {
            var creator = await _context.Creators.FirstOrDefaultAsync(c => c.Id == user_id);
            if (creator == null) return null;
            creator.ArtisticName = artisticName;
            await _context.SaveChangesAsync();
            return creator;

        }


        public async Task<bool> HasUserRatedCreator(int userId, int creatorId)
        {
            return await _context.CreatorRatings
                .AnyAsync(r => r.EvaluatorId == userId && r.TargetCreatorId == creatorId);
        }

        public async Task AddRating(CreatorRating rating)
        {
            await _context.CreatorRatings.AddAsync(rating);
        }


        public async Task<List<CreatorSocialProfile>> GetRecommendedSocialProfilesByScore(int userId, CreatorInterestProfile profile)
        {
            var payload = new
            {
                sectors = profile.SectorFrequencies,
                platforms = profile.PlatformFrequencies,
                avg_price = profile.AveragePriceViewed
            };

            // Debug: Print averagePriceViewed
            Console.WriteLine($"\n--- DEBUG: Average Price Viewed: {profile.AveragePriceViewed} ---");
            Console.WriteLine($"Payload: {JsonSerializer.Serialize(payload)}\n");

            string json = JsonSerializer.Serialize(payload);

            var rawResults = await _context.Database.SqlQueryRaw<ProfileRecommendationDto>(
                "SELECT * FROM dbo.get_recommended_social_profiles({0}, {1}::jsonb)",
                userId, json
            ).ToListAsync();

            // 2. Printamos os valores no terminal
            Console.WriteLine("\n--- DEBUG: PONTUAÇÃO DE RECOMENDAÇÕES DE CREATORS ---");
            foreach (var item in rawResults)
            {
                Console.WriteLine($"Social Profile ID: {item.social_profile_id} | Pontos: {item.recommendation_score}");
            }
            Console.WriteLine("------------------------------------------\n");

            var ids = rawResults.Select(r => r.social_profile_id).ToList();
            var orderMap = ids.Select((id, index) => new { id, index }).ToDictionary(x => x.id, x => x.index);

            var profiles = await _context.CreatorSocialProfiles
                .Include(sp => sp.Creator)
                .Include(sp => sp.Platform)
                .Include(sp => sp.Sectors)
                .Where(sp => ids.Contains(sp.Id))
                .AsNoTracking()
                .ToListAsync();

            return profiles
                .OrderBy(sp => orderMap.ContainsKey(sp.Id) ? orderMap[sp.Id] : int.MaxValue)
                .ToList();
        }

        public class ProfileRecommendationDto
        {
            public int social_profile_id { get; set; }
            public int recommendation_score { get; set; }
        }
    }
}
