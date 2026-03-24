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
                .FirstOrDefaultAsync(com => com.Id == userId);
        }

        public async Task<Company?> EditProfile(int user_id, string company_size, string company_name, string description, string sector, string website_link, string country_headquarters)
        {
            var company = await _context.Companies.FindAsync(user_id);
            if (company == null) return null;
            company.CompanyName = company_name;
            company.Description = description;
            company.Sector = sector;
            company.CompanySize = company_size;
            company.WebsiteLink = website_link;
            company.CountryHeadquarters = country_headquarters;
            _context.Companies.Update(company);
            
            await _context.SaveChangesAsync();

            return company;
        }


    

    public async Task<PagedResponse<Company>> Search( string Name, string sector, string CompanySize, string CountryHeadquarters , int Page, int PageSize)
        {
            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(c => EF.Functions.Like(c.CompanyName, $"%{Name}%"));
            }

            if (!string.IsNullOrWhiteSpace(sector))
            {
                query = query.Where(c => c.Sector == sector);
            }

            if (!string.IsNullOrWhiteSpace(CompanySize))
            {
                query = query.Where(c => c.CompanySize == CompanySize);
            }

            if (!string.IsNullOrWhiteSpace(CountryHeadquarters))
            {
                query = query.Where(c => c.CountryHeadquarters == CountryHeadquarters);
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