using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hubly.api.Infrastructure
{
    public class AuditRepository : IAuditRepository
    {
        private readonly HublyDbContext _context;

        public AuditRepository(HublyDbContext context)
        {
            _context = context;
        }

        public async Task AddLog(AuditLog log)
        {
            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();

        }

        public async Task<PagedResponse<AuditLog>> Search(int userId, int page, int pageSize)
        {
            var query = _context.AuditLogs
                .AsNoTracking()
                .AsQueryable();

            if (userId > 0)
            {
                query = query.Where(a => a.UserId == userId);
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<AuditLog>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}