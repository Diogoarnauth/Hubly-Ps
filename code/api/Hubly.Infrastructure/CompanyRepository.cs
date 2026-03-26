using Microsoft.EntityFrameworkCore;
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
                .Include(c => c.Sector)
                .Include(c => c.SubSector)
                .AsNoTracking()
                .FirstOrDefaultAsync(com => com.Id == userId);
        }

        public async Task<Company?> EditProfile(int user_id, string company_size, string company_name, string description, int sectorId, int? subSectorId, string website_link, string country_headquarters)
        {
            var company = await _context.Companies.FindAsync(user_id);
            if (company == null) return null;

            company.CompanyName = company_name;
            company.Description = description;
            company.SectorId = sectorId;
            company.SubSectorId = subSectorId;
            company.CompanySize = company_size;
            company.WebsiteLink = website_link;
            company.CountryHeadquarters = country_headquarters;

            _context.Companies.Update(company);
            await _context.SaveChangesAsync();

            _context.Entry(company).State = EntityState.Detached;

            return company;
        }

        //About Sectors
        public async Task<int?> GetSectorIdByName(string sectorName)
        {
            var sector = await _context.Sectors
                .FirstOrDefaultAsync(s => s.SectorName.ToLower() == sectorName.ToLower());
            return sector?.Id;
        }

        public async Task<int?> GetSubSectorIdByName(int sectorId, string subSectorName)
        {
            var subSector = await _context.SubSectors
                .FirstOrDefaultAsync(s => s.SectorId == sectorId && s.SubSectorName.ToLower() == subSectorName.ToLower());
            return subSector?.Id;
        }



        public async Task<PagedResponse<Company>> Search(string? Name, string? sector, List<string>? subSectors, string? CompanySize, List<string>? countries, int Page, int PageSize)
        {
            var query = _context.Companies
                .Include(c => c.Sector)
                .Include(c => c.SubSector)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(c => EF.Functions.Like(c.CompanyName, $"%{Name}%"));
            }

            if (!string.IsNullOrWhiteSpace(sector))
            {
                query = query.Where(c => c.Sector.SectorName.ToLower() == sector.ToLower());
            }

            if (subSectors != null && subSectors.Any())
            {
                var subSectorsLower = subSectors.Select(s => s.ToLower()).ToList();
                query = query.Where(c => subSectorsLower.Contains(c.SubSector.SubSectorName.ToLower()));
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
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return new PagedResponse<Company>
            {
                Items = items,
                TotalItems = totalItems,
                Page = Page,
                PageSize = PageSize
            };
        }

    }
}