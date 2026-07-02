using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hubly.api.Infrastructure;

public class CoWorkerRepository : ICoWorkerRepository
{
    private readonly HublyDbContext _context;

    public CoWorkerRepository(HublyDbContext context)
    {
        _context = context;
    }

    public async Task<CoWorker?> GetCoWorker(int userId)
    {
        return await _context.CoWorkers
            .FirstOrDefaultAsync(cw => cw.UserId == userId);
    }

    public async Task<bool> InviteExists(int ownerId, string email)
    {
        return await _context.CoWorkerInvites
            .AnyAsync(invite => invite.OwnerId == ownerId
                && invite.CoWorkerEmail == email
                && invite.Status == "WAITING");
    }

    public async Task CreateInvite(int ownerId, string email)
    {
        var invite = new CoWorkerInvite
        {
            OwnerId = ownerId,
            CoWorkerEmail = email,
            Status = "WAITING",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _context.CoWorkerInvites.AddAsync(invite);
    }

    public async Task<CoWorkerInvite?> GetInviteById(int inviteId)
    {
        return await _context.CoWorkerInvites
            .FirstOrDefaultAsync(invite => invite.Id == inviteId);
    }

    public async Task UpdateStatus(int inviteId, string status)
    {
        var invite = await _context.CoWorkerInvites.FindAsync(inviteId);
        if (invite == null) return;

        invite.Status = status;
        _context.Entry(invite).Property(i => i.Status).IsModified = true;
    }

    public async Task<List<CoWorkerInvite>> GetInvitesByEmail(string email)
    {
        return await _context.CoWorkerInvites
            .Where(invite => invite.CoWorkerEmail == email)
            .ToListAsync();
    }

    public async Task<List<CoWorkerInvite>> GetInvitesByOwner(int ownerId)
    {
        return await _context.CoWorkerInvites
            .Where(invite => invite.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task CreateCoWorker(int userId, int ownerId)
    {
        var coWorker = new CoWorker
        {
            UserId = userId,
            OwnerId = ownerId,
            JoinedAt = DateTime.UtcNow
        };

        await _context.CoWorkers.AddAsync(coWorker);
    }

    public async Task DeleteCoWorker(int userId)
    {
        var coWorker = await _context.CoWorkers.FirstOrDefaultAsync(cw => cw.UserId == userId);
        if (coWorker != null)
        {
            _context.CoWorkers.Remove(coWorker);
        }
    }
    public async Task DeleteAcceptedInvite(int ownerId, string email)
    {
        var invite = await _context.CoWorkerInvites
            .FirstOrDefaultAsync(i => i.OwnerId == ownerId
                                   && i.CoWorkerEmail == email
                                   && i.Status == "ACCEPTED");

        if (invite != null)
        {
            _context.CoWorkerInvites.Remove(invite);
        }
    }
    public async Task<CoWorker?> GetCoWorkerByOwnerAndUser(int ownerId, int coWorkerUserId)
    {
        return await _context.CoWorkers
            .FirstOrDefaultAsync(cw => cw.OwnerId == ownerId && cw.UserId == coWorkerUserId);
    }


    public async Task DeleteCoWorkerByIds(int ownerId, int coWorkerUserId)
    {
        var coWorker = await GetCoWorkerByOwnerAndUser(ownerId, coWorkerUserId);
        if (coWorker != null)
        {
            _context.CoWorkers.Remove(coWorker);
        }
    }

    public async Task<List<CoWorker>> GetTeamByOwnerId(int ownerId)
    {
        return await _context.CoWorkers
            .Include(cw => cw.User)
            .Where(cw => cw.OwnerId == ownerId)
            .ToListAsync();
    }
}
