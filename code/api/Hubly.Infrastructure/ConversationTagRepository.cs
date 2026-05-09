using Hubly.api.Infrastructure.Data;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hubly.api.Infrastructure;

public class ConversationTagRepository : IConversationTagRepository
{
    private readonly HublyDbContext _context;

    public ConversationTagRepository(HublyDbContext context)
    {
        _context = context;
    }

    public async Task<ConversationTag?> GetById(int tagId)
    {
        return await _context.ConversationTags.FirstOrDefaultAsync(t => t.Id == tagId);
    }

    public async Task<List<ConversationTag>> GetUserTags(int userId)
    {
        return await _context.ConversationTags
            .Where(t => t.UserId == userId || t.UserId == null)
            .OrderBy(t => t.TagName)
            .ToListAsync();
    }

    public async Task<int> CreateTag(ConversationTag tag)
    {
        await _context.ConversationTags.AddAsync(tag);
        await _context.SaveChangesAsync();
        return tag.Id;
    }

    public async Task UpdateTag(ConversationTag tag)
    {
        _context.ConversationTags.Update(tag);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTag(int tagId)
    {
        var tag = await _context.ConversationTags.FirstOrDefaultAsync(t => t.Id == tagId);
        if (tag != null)
        {
            _context.ConversationTags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<ConversationTag?> GetAssignment(int userId, int conversationId)
    {
        return await _context.ConversationTagAssignments
            .FirstOrDefaultAsync(a => a.UserId == userId && a.ConversationId == conversationId);
    }

    public async Task<List<ConversationTag>> GetConversationTags(int userId, int conversationId)
    {
        return await _context.ConversationTagAssignments
            .Where(a => a.UserId == userId && a.ConversationId == conversationId)
            .Include(a => a.Tag)
            .Select(a => a.Tag!)
            .ToListAsync();
    }

    public async Task AssignTag(ConversationTagAssignment assignment)
    {
        // Remove assignment anterior se existir
        var existing = await _context.ConversationTagAssignments
            .FirstOrDefaultAsync(a => a.UserId == assignment.UserId && a.ConversationId == assignment.ConversationId);
        
        if (existing != null)
        {
            _context.ConversationTagAssignments.Remove(existing);
        }

        await _context.ConversationTagAssignments.AddAsync(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveTag(int userId, int conversationId)
    {
        var assignment = await _context.ConversationTagAssignments
            .FirstOrDefaultAsync(a => a.UserId == userId && a.ConversationId == conversationId);
        
        if (assignment != null)
        {
            _context.ConversationTagAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> TagNameExistsForUser(int userId, string tagName)
    {
        return await _context.ConversationTags
            .AnyAsync(t => (t.UserId == userId || t.UserId == null) && t.TagName.ToLower() == tagName.ToLower());
    }
}
