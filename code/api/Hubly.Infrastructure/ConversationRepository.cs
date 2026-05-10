using Hubly.api.Infrastructure.Data;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hubly.api.Infrastructure
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly HublyDbContext _context;

        public ConversationRepository(HublyDbContext context)
        {
            _context = context;
        }
        public async Task<int> AddConversation(Conversation conversation)
        {
            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
            return conversation.Id;
        }
        public async Task<Conversation?> GetConversationByParticipants(
            int? sCompId, int? sProfId,
            int? rCompId, int? rProfId)
        {
            return await _context.Conversations
                .Include(c => c.Participants)
                // Garante que a conversa tem exatamente 2 pessoas
                .Where(c => c.Participants.Count == 2)
                .Where(c =>
                    // Verifica se um dos participantes é o Sender (Tipado)
                    c.Participants.Any(p =>
                        (sCompId != null && p.CompanyId == sCompId && p.SocialProfileId == null) ||
                        (sProfId != null && p.SocialProfileId == sProfId && p.CompanyId == null)
                    )
                    &&
                    // Verifica se o outro participante é o Receiver (Tipado)
                    c.Participants.Any(p =>
                        (rCompId != null && p.CompanyId == rCompId && p.SocialProfileId == null) ||
                        (rProfId != null && p.SocialProfileId == rProfId && p.CompanyId == null)
                    )
                )
                .FirstOrDefaultAsync();
        }
        public async Task<bool> IsUserParticipant(int conversationId, int userId)
        {
            return await _context.ConversationParticipants
                .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);
        }

        public async Task<Conversation?> GetById(int id)
        {
            return await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Conversation?> GetConversationWithParticipants(int id)
        {
            return await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task Update(Conversation conversation)
        {
            _context.Conversations.Update(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ConversationWithLastMessage>> GetCreatorConversationsByProfileExtended(int userId, int socialProfileId)
        {
            return await _context.Conversations
                .Where(c => c.Participants.Any(p => p.UserId == userId && p.SocialProfileId == socialProfileId))
                .Select(c => new ConversationWithLastMessage
                {
                    // PROJEÇÃO DA CONVERSA: Garantimos que os participantes vão lá dentro
                    Conversation = new Conversation
                    {
                        Id = c.Id,
                        CreatedAt = c.CreatedAt,
                        LastMessageAt = c.LastMessageAt,
                        Participants = c.Participants.Select(p => new ConversationParticipant
                        {
                            UserId = p.UserId,
                            CompanyId = p.CompanyId,
                            SocialProfileId = p.SocialProfileId,
                            // Incluímos os objetos de navegação para o Controller não dar null
                            User = p.User,
                            Company = p.Company,
                            SocialProfile = p.SocialProfile
                        }).ToList()
                    },

                    // LÓGICA DO NOME: Creator (User.Name) ou Company (Company.CompanyName)
                    OtherPartyName = c.Participants
                        .Where(p => p.UserId != userId)
                        .Select(p => p.CompanyId != null
                            ? p.Company.CompanyName
                            : p.User.Name)
                        .FirstOrDefault() ?? "Unknown",

                    PlatformId = c.Participants
                        .Where(p => p.UserId == userId)
                        .Select(p => p.SocialProfileId)
                        .FirstOrDefault(),

                    LastMessage = c.Messages
                        .Where(m => !m.IsDeleted)
                        .OrderByDescending(m => m.SentAt)
                        .FirstOrDefault(),

                    UnreadCount = c.Messages.Count(m =>
                        !m.IsDeleted &&
                        m.SenderId != userId &&
                        (!_context.MessageReadStatuses.Any(rs => rs.ConversationId == c.Id && rs.UserId == userId) ||
                         m.Id > _context.MessageReadStatuses
                            .Where(rs => rs.ConversationId == c.Id && rs.UserId == userId)
                            .Select(rs => rs.LastReadMessageId).FirstOrDefault())),

                    Tag = _context.ConversationTagAssignments
                        .Where(cta => cta.ConversationId == c.Id && cta.UserId == userId)
                        .Select(cta => cta.ConversationTag)
                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.Conversation.LastMessageAt)
                .ToListAsync();
        }
        public async Task<List<ConversationWithLastMessage>> GetCompanyConversationsExtended(int userId, int companyId)
        {
            return await _context.Conversations
                .Where(c => c.Participants.Any(p => p.UserId == userId && p.CompanyId == companyId))
                .Select(c => new ConversationWithLastMessage
                {
                    Conversation = new Conversation
                    {
                        Id = c.Id,
                        CreatedAt = c.CreatedAt,
                        LastMessageAt = c.LastMessageAt,
                        Participants = c.Participants.Select(p => new ConversationParticipant
                        {
                            UserId = p.UserId,
                            CompanyId = p.CompanyId,
                            SocialProfileId = p.SocialProfileId,
                            User = p.User,
                            Company = p.Company,
                            SocialProfile = p.SocialProfile
                        }).ToList()
                    },

                    // O outro lado para uma Company é SEMPRE um Creator (SocialProfile)
                    OtherPartyName = c.Participants
                        .Where(p => p.UserId != userId)
                        .Select(p => p.SocialProfile != null
                            ? p.SocialProfile.Creator.ArtisticName
                            : p.User.Name)
                        .FirstOrDefault() ?? "Unknown",

                    // Para Company, o PlatformId vem do perfil do Creator com quem ela fala
                    PlatformId = c.Participants
                        .Where(p => p.UserId != userId)
                        .Select(p => p.SocialProfileId)
                        .FirstOrDefault(),

                    LastMessage = c.Messages
                        .Where(m => !m.IsDeleted)
                        .OrderByDescending(m => m.SentAt)
                        .FirstOrDefault(),

                    UnreadCount = c.Messages.Count(m =>
                        !m.IsDeleted &&
                        m.SenderId != userId &&
                        (!_context.MessageReadStatuses.Any(rs => rs.ConversationId == c.Id && rs.UserId == userId) ||
                         m.Id > _context.MessageReadStatuses
                            .Where(rs => rs.ConversationId == c.Id && rs.UserId == userId)
                            .Select(rs => rs.LastReadMessageId).FirstOrDefault())),

                    Tag = _context.ConversationTagAssignments
                        .Where(cta => cta.ConversationId == c.Id && cta.UserId == userId)
                        .Select(cta => cta.ConversationTag)
                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.Conversation.LastMessageAt)
                .ToListAsync();
        }

        public async Task UpdateLastReadMessage(int conversationId, int userId, int messageId)
        {
            var readStatus = await _context.MessageReadStatuses
                .FirstOrDefaultAsync(rs => rs.ConversationId == conversationId && rs.UserId == userId);

            if (readStatus == null)
            {
                readStatus = new MessageReadStatus
                {
                    ConversationId = conversationId,
                    UserId = userId,
                    LastReadMessageId = messageId,
                    LastReadAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };
                _context.MessageReadStatuses.Add(readStatus);
            }
            else
            {
                readStatus.LastReadMessageId = messageId;
                readStatus.LastReadAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                _context.MessageReadStatuses.Update(readStatus);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int?> GetLastReadMessageId(int conversationId, int userId)
        {
            var readStatus = await _context.MessageReadStatuses
                .FirstOrDefaultAsync(rs => rs.ConversationId == conversationId && rs.UserId == userId);

            return readStatus?.LastReadMessageId;
        }

        public async Task<int> GetUnreadMessageCount(int conversationId, int userId)
        {
            var lastReadId = await GetLastReadMessageId(conversationId, userId);

            if (!lastReadId.HasValue)
            {
                // If never read, count all non-deleted messages
                return await _context.Messages
                    .CountAsync(m => m.ConversationId == conversationId && !m.IsDeleted);
            }

            return await _context.Messages
                .CountAsync(m => m.ConversationId == conversationId && m.Id > lastReadId.Value && !m.IsDeleted);
        }
    }
}