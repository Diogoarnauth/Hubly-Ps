using Microsoft.EntityFrameworkCore;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;

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

        public async Task EditProfile(int user_id, int company_size, string company_name, string description, string sector, string website_link, string country_headquarters)
        {
            var company = await _context.Companies.FindAsync(user_id);
            company.CompanyName = company_name;
            company.Description = description;
            company.Sector = sector;
            company.CompanySize = company_size;
            company.WebsiteLink = website_link;
            company.CountryHeadquarters = country_headquarters;
            _context.Companies.Update(company);
        }


    }
}