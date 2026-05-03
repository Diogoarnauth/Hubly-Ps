using Microsoft.EntityFrameworkCore;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;

namespace Hubly.api.Infrastructure
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly HublyDbContext _context;

        public HistoryRepository(HublyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddView(ProfileViewHistory history)
        {
            await _context.Set<ProfileViewHistory>().AddAsync(history);
            return true;
        }

        public async Task<List<CreatorSocialProfile>> GetTopTrendingCreators(int limit)
        {
            var topProfileIds = await _context.ProfileViewHistory
                .Where(h => h.ViewedSocialProfileId.HasValue) 
                .GroupBy(h => h.ViewedSocialProfileId)
                .OrderByDescending(g => g.Count())
                .Take(limit)
                .Select(g => g.Key!.Value) 
                .ToListAsync();

            if (topProfileIds == null || !topProfileIds.Any())
                return new List<CreatorSocialProfile>();

            var profiles = await _context.CreatorSocialProfiles
                .Include(sp => sp.Creator)
                .Include(sp => sp.Platform)
                .Include(sp => sp.Sectors)
                .Where(sp => topProfileIds.Contains(sp.Id))
                .ToListAsync();

            return topProfileIds
                .Select(id => profiles.FirstOrDefault(p => p.Id == id))
                .Where(p => p != null)
                .Cast<CreatorSocialProfile>()
                .ToList();
        }
        public async Task<List<Company>> GetTopTrendingCompanies(int limit)
        {
            var topIds = await _context.ProfileViewHistory
                .Where(h => h.ViewedCompanyId != null)
                .GroupBy(h => h.ViewedCompanyId)
                .OrderByDescending(g => g.Count())
                .Take(limit)
                .Select(g => g.Key.Value)
                .ToListAsync();

            var companies = await _context.Companies
                .Include(c => c.Sectors)
                .Where(c => topIds.Contains(c.Id))
                .ToListAsync();

            return companies
                .OrderBy(c => topIds.IndexOf(c.Id))
                .ToList();
        }

        public async Task<List<ProfileViewHistory>> GetUserHistory(int userId, int limit = 20)
        {
            return await _context.ProfileViewHistory
                .Include(h => h.ViewedCompany)
                .Include(h => h.ViewedSocialProfile)
                    .ThenInclude(sp => sp.Creator)
                .Where(h => h.ViewerUserId == userId)
                .OrderByDescending(h => h.ViewedAt)
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserInterestProfile> GetUserInterests(int userId, int limit = 50)
        {
            var history = await _context.ProfileViewHistory
                .Where(h => h.ViewerUserId == userId && h.ViewedCompanyId != null)
                .OrderByDescending(h => h.ViewedAt)
                .Take(limit)
                .Include(h => h.ViewedCompany)
                    .ThenInclude(c => c.Sectors)
                .ToListAsync();

            if (!history.Any()) return new UserInterestProfile(new(), new(), new());

            // Frequência de Setores
            var sectorFreq = history
                .SelectMany(h => h.ViewedCompany.Sectors)
                .GroupBy(s => s.Id)
                .ToDictionary(g => g.Key, g => g.Count());

            // Frequência de Países
            var countryFreq = history
                .GroupBy(h => h.ViewedCompany.CountryHeadquarters)
                .ToDictionary(g => g.Key, g => g.Count());

            // Frequência de Tamanhos (usando as strings exatas: "0 a 100", etc)
            var sizeFreq = history
                .GroupBy(h => h.ViewedCompany.CompanySize)
                .ToDictionary(g => g.Key, g => g.Count());

            return new UserInterestProfile(sectorFreq, countryFreq, sizeFreq);
        }


        public async Task<CreatorInterestProfile> GetCreatorInterests(int userId, int limit = 50)
        {
            // 1. Procurar o histórico filtrando apenas por visualizações de perfis sociais
            var history = await _context.ProfileViewHistory
                .Where(h => h.ViewerUserId == userId && h.ViewedSocialProfileId != null)
                .OrderByDescending(h => h.ViewedAt)
                .Take(limit)
                .Include(h => h.ViewedSocialProfile)
                    .ThenInclude(sp => sp.Sectors)
                .ToListAsync();

            if (!history.Any()) return new CreatorInterestProfile();

            // 2 Frequência de Setores 
            var sectorFreq = history
                .Where(h => h.ViewedSocialProfile?.Sectors != null)
                .SelectMany(h => h.ViewedSocialProfile.Sectors)
                .GroupBy(s => s.Id)
                .ToDictionary(g => g.Key, g => g.Count());

            // 3. Frequência de Plataformas
            var platformFreq = history
                .Where(h => h.ViewedSocialProfile != null)
                .GroupBy(h => h.ViewedSocialProfile.PlatformId)
                .ToDictionary(g => g.Key, g => g.Count());

            // 4. Média de Preços
            var prices = history
                .Where(h => h.ViewedSocialProfile?.PriceMin != null)
                .Select(h => (double)h.ViewedSocialProfile.PriceMin!.Value)
                .ToList();

            double avgPrice = prices.Any() ? prices.Average() : 0;

            return new CreatorInterestProfile(sectorFreq, platformFreq, avgPrice);
        }
    }
}