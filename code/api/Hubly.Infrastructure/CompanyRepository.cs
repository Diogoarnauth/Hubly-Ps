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

    }
}