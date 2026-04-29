using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Hubly.api.Infrastructure
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly HublyDbContext _context;

        public CompanyRepository(HublyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterCompany(Company newCompany)
        {
            await _context.Companies.AddAsync(newCompany);
            // O SaveChangesAsync será gerido pelo TransactionManager no Service
            return true;
        }

        public async Task<bool> ExistsByUserId(int userId)
        {
            return await _context.Companies.AnyAsync(com => com.Id == userId);
        }

        public async Task<Company?> GetByUserId(int userId)
        {
            return await _context.Companies
                .Include(c => c.Sectors)
                .AsNoTracking()
                .FirstOrDefaultAsync(com => com.Id == userId);
        }

        public async Task<Company?> EditProfile(int user_id, string company_size, string company_name, string description, List<Sector> sectors, string website_link, string country_headquarters)
        {
            var company = await _context.Companies
                .Include(c => c.Sectors)
                .FirstOrDefaultAsync(c => c.Id == user_id);

            if (company == null) return null;

            company.CompanyName = company_name;
            company.Description = description;
            company.CompanySize = company_size;
            company.WebsiteLink = website_link;
            company.CountryHeadquarters = country_headquarters;

            company.Sectors.Clear();
            foreach (var sector in sectors)
            {
                _context.Set<Sector>().Attach(sector);
                company.Sectors.Add(sector);
            }
            await _context.SaveChangesAsync();

            return company;
        }

        //About Sector
        public async Task<List<Sector>> GetSectorByName(List<string> sectorName)
        {
            return await _context.Sectors
                .Where(s => sectorName.Contains(s.SectorName))
                .ToListAsync();
        }



        public async Task<PagedResponse<Company>> Search(string? Name, List<string>? sectors, string? CompanySize, List<string>? countries, int page, int pageSize)
        {
            var query = _context.Companies
                .Include(c => c.Sectors)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(c => EF.Functions.ILike(c.CompanyName, $"%{Name}%"));
            }

            if (sectors != null && sectors.Any())
            {
                var sectorsLower = sectors.Select(s => s.ToLower()).ToList();
                query = query.Where(c => c.Sectors.Any(s => sectorsLower.Contains(s.SectorName.ToLower())));
            }

            if (!string.IsNullOrWhiteSpace(CompanySize))
            {
                query = query.Where(c => c.CompanySize == CompanySize);
            }

            if (countries != null && countries.Any())
            {
                var countriesLower = countries.Select(c => c.ToLower()).ToList();
                query = query.Where(c => countriesLower.Contains(c.CountryHeadquarters.ToLower()));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.CompanyName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<Company>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<List<Company>> GetRecommendedByScore(int userId, UserInterestProfile profile)
        {
            var payload = new
            {
                sectors = profile.SectorFrequencies,
                countries = profile.CountryFrequencies,
                sizes = profile.SizeFrequencies
            };

            string interestsJson = JsonSerializer.Serialize(payload);

            var rawResults = await _context.Database.SqlQueryRaw<CompanyRecommendationDto>(
                "SELECT * FROM dbo.get_recommended_companies({0}, {1}::jsonb)",
                userId, interestsJson
            ).ToListAsync();

            // 2. Printamos os valores no terminal
            Console.WriteLine("\n--- DEBUG: PONTUAÇÃO DE RECOMENDAÇÕES ---");
            foreach (var item in rawResults)
            {
                Console.WriteLine($"Empresa: {item.company_name.PadRight(20)} | Pontos: {item.recommendation_score}");
            }
            Console.WriteLine("------------------------------------------\n");

            var companyIds = rawResults.Select(r => r.user_id).ToList();

            return await _context.Companies
                .Where(c => companyIds.Contains(c.Id))
                .Include(c => c.Sectors)
                .AsNoTracking()
                .ToListAsync();
        }

    }
     public class CompanyRecommendationDto
        {
            public int user_id { get; set; }
            public string company_name { get; set; }
            public int recommendation_score { get; set; }
        }
}