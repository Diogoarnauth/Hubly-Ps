using Microsoft.EntityFrameworkCore;
using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Data;

public class HublyDbContext : DbContext
{
    public HublyDbContext(DbContextOptions<HublyDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<Creator> Creators { get; set; }
    public DbSet<Sector> Sectors { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<EmailConfirmation> EmailConfirmations { get; set; }
    public DbSet<SocialPlatform> SocialPlatforms { get; set; }
    public DbSet<CreatorSocialProfile> CreatorSocialProfiles { get; set; }
    public DbSet<ProfileViewHistory> ProfileViewHistory { get; set; }
    public DbSet<CreatorRating> CreatorRatings { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ConversationTag> ConversationTags { get; set; }
    public DbSet<ConversationTagAssignment> ConversationTagAssignments { get; set; }
    public DbSet<MessageReadStatus> MessageReadStatuses { get; set; }
    public DbSet<CoWorker> CoWorkers { get; set; }
    public DbSet<CoWorkerInvite> CoWorkerInvites { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CONFIGURAÇÃO: User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", "dbo");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("id").ValueGeneratedOnAdd();
        });


        // CONFIGURAÇÃO: Token
        modelBuilder.Entity<Token>(entity =>
        {
            // Nota: No teu SQL está "dbo.token" (singular)
            entity.ToTable("token", "dbo");
            entity.HasKey(t => t.TokenValidation);

            entity.Property(t => t.TokenValidation).HasColumnName("token_validation");
            entity.Property(t => t.UserId).HasColumnName("user_id");
            entity.Property(t => t.CreatedAt).HasColumnName("created_at");
            entity.Property(t => t.LastUsedAt).HasColumnName("last_used_at");
        });

        // CONFIGURAÇÃO: Creator (PK é FK do User)
        modelBuilder.Entity<Creator>(entity =>
        {
            entity.ToTable("creators", "dbo");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("user_id").ValueGeneratedNever();

            entity.HasOne(c => c.User)
                  .WithOne(u => u.Creator)
                  .HasForeignKey<Creator>(c => c.Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CreatorRating>(entity =>
        {
            entity.ToTable("creator_ratings", "dbo");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).HasColumnName("id").ValueGeneratedOnAdd();

            entity.Property(r => r.EvaluatorId).HasColumnName("evaluator_id");
            entity.Property(r => r.TargetCreatorId).HasColumnName("target_creator_id");
            entity.Property(r => r.RatingValue).HasColumnName("rating_value");
            entity.Property(r => r.RatedAt).HasColumnName("rated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(r => new { r.EvaluatorId, r.TargetCreatorId }).IsUnique();

            entity.HasOne(r => r.Evaluator)
                .WithMany()
                .HasForeignKey(r => r.EvaluatorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.TargetCreator)
                .WithMany()
                .HasForeignKey(r => r.TargetCreatorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SocialPlatform>(entity =>
        {
            entity.ToTable("social_platforms", "dbo");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(p => p.NamePlatform).HasColumnName("name_platform").IsRequired();

            entity.HasIndex(p => p.NamePlatform).IsUnique();
        });

        modelBuilder.Entity<CreatorSocialProfile>(entity =>
        {
            entity.ToTable("creator_social_profiles", "dbo");
            entity.HasKey(csp => csp.Id);
            entity.Property(csp => csp.Id).HasColumnName("id").ValueGeneratedOnAdd();

            entity.Property(csp => csp.CreatorId).HasColumnName("creator_id");
            entity.Property(csp => csp.PlatformId).HasColumnName("platform_id");
            entity.Property(csp => csp.PlatformUserName).HasColumnName("platform_user_name");
            entity.Property(csp => csp.Link).HasColumnName("link");
            entity.Property(csp => csp.Description).HasColumnName("description");
            entity.Property(csp => csp.FollowersCount).HasColumnName("followers_count");

            entity.Property(csp => csp.PriceMin).HasColumnName("price_min").HasPrecision(10, 2);
            entity.Property(csp => csp.PriceMax).HasColumnName("price_max").HasPrecision(10, 2);

            entity.HasMany(csp => csp.Sectors)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "creator_profile_sectors",
                    j => j.HasOne<Sector>()
                            .WithMany()
                            .HasForeignKey("sector_id"),
                    j => j.HasOne<CreatorSocialProfile>()
                            .WithMany()
                            .HasForeignKey("profile_id"),
                    j =>
                    {
                        j.ToTable("creator_profile_sectors", "dbo");
                        j.HasKey("profile_id", "sector_id");
                    });

            entity.HasOne(csp => csp.Creator)
                  .WithMany(c => c.SocialProfiles)
                  .HasForeignKey(csp => csp.CreatorId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(csp => csp.Platform)
                  .WithMany(p => p.CreatorProfiles)
                  .HasForeignKey(csp => csp.PlatformId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(csp => new { csp.PlatformId, csp.PlatformUserName }).IsUnique();
        });

        modelBuilder.Entity<Sector>(entity =>
        {
            entity.ToTable("sectors", "dbo");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id).HasColumnName("id");
            entity.Property(s => s.SectorName).HasColumnName("sector_name");

            entity.HasIndex(s => s.SectorName).IsUnique();
        });

        // CONFIGURAÇÃO: Company (PK é FK do User)
        modelBuilder.Entity<Company>(entity =>
    {
        entity.ToTable("companies", "dbo");
        entity.HasKey(c => c.Id);
        entity.Property(c => c.Id).HasColumnName("user_id").ValueGeneratedNever();

        entity.HasMany(c => c.Sectors)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "company_sectors",
                j => j.HasOne<Sector>()
                        .WithMany()
                        .HasForeignKey("sector_id"),
                j => j.HasOne<Company>()
                        .WithMany()
                        .HasForeignKey("company_user_id"),
                j =>
                {
                    j.ToTable("company_sectors", "dbo");
                    j.HasKey("company_user_id", "sector_id");
                });

        entity.HasOne(c => c.User)
            .WithOne(u => u.Company)
            .HasForeignKey<Company>(c => c.Id)
            .OnDelete(DeleteBehavior.Cascade);
    });
        // TODO() VER MELHOR
        modelBuilder.Entity<EmailConfirmation>(entity =>
        {
            entity.ToTable("email_confirmation", "dbo");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.ConfirmationCode)
                .HasColumnName("confirmation_code")
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at").IsRequired();
            entity.Property(e => e.Used)
                .HasColumnName("used")
                .IsRequired()
                .HasDefaultValue(false);

            entity.HasOne(e => e.User)
                .WithMany(u => u.EmailConfirmations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<ProfileViewHistory>(entity =>
        {
            entity.ToTable("profile_views_history", "dbo");
            entity.HasKey(h => h.Id);

            entity.Property(h => h.Id).HasColumnName("id");
            entity.Property(h => h.ViewerUserId).HasColumnName("viewer_user_id");
            entity.Property(h => h.ViewedCompanyId).HasColumnName("viewed_company_id");
            entity.Property(h => h.ViewedSocialProfileId).HasColumnName("viewed_social_profile_id");
            entity.Property(h => h.ViewedAt).HasColumnName("viewed_at");

            entity.HasOne(h => h.ViewedCompany)
          .WithMany()
          .HasForeignKey(h => h.ViewedCompanyId);

            entity.HasOne(h => h.ViewedSocialProfile)
                  .WithMany()
                  .HasForeignKey(h => h.ViewedSocialProfileId);
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.ToTable("conversations", "dbo");
            entity.HasKey(con => con.Id);
            entity.Property(con => con.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(con => con.CreatedAt).HasColumnName("created_at");
            entity.Property(con => con.LastMessageAt).HasColumnName("last_message_at");
        });

        modelBuilder.Entity<ConversationParticipant>(entity =>
        {
            entity.ToTable("conversation_participants", "dbo");

            entity.HasKey(cp => new { cp.ConversationId, cp.UserId });

            entity.Property(cp => cp.ConversationId).HasColumnName("conversation_id");
            entity.Property(cp => cp.UserId).HasColumnName("user_id");
            entity.Property(cp => cp.CompanyId).HasColumnName("company_id");
            entity.Property(cp => cp.SocialProfileId).HasColumnName("social_profile_id");

            entity.HasOne(cp => cp.Conversation)
                .WithMany(con => con.Participants)
                .HasForeignKey(cp => cp.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cp => cp.User)
                .WithMany()
                .HasForeignKey(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cp => cp.Company)
                .WithMany()
                .HasForeignKey(cp => cp.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(cp => cp.SocialProfile)
                .WithMany()
                .HasForeignKey(cp => cp.SocialProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("messages", "dbo");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).HasColumnName("id").ValueGeneratedOnAdd();

            entity.Property(m => m.ConversationId).HasColumnName("conversation_id");
            entity.Property(m => m.SenderId).HasColumnName("sender_id");
            entity.Property(m => m.Content).HasColumnName("content").IsRequired();
            entity.Property(m => m.SentAt).HasColumnName("sent_at");
            entity.Property(m => m.IsEdited).HasColumnName("is_edited").HasDefaultValue(false);
            entity.Property(m => m.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

            entity.HasOne(m => m.Conversation)
                .WithMany(con => con.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CoWorker>(entity =>
        {
            entity.ToTable("co_workers", "dbo");
            entity.HasKey(cw => cw.Id);
            entity.Property(cw => cw.Id).HasColumnName("id").ValueGeneratedOnAdd();

            entity.Property(cw => cw.UserId).HasColumnName("user_id");
            entity.Property(cw => cw.OwnerId).HasColumnName("owner_id");
            entity.Property(cw => cw.JoinedAt).HasColumnName("joined_at");

            entity.HasOne(cw => cw.User)
                .WithMany()
                .HasForeignKey(cw => cw.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cw => cw.Owner)
                .WithMany()
                .HasForeignKey(cw => cw.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(cw => cw.UserId).IsUnique();
        });

        modelBuilder.Entity<CoWorkerInvite>(entity =>
        {
            entity.ToTable("co_worker_invites", "dbo");
            entity.HasKey(ci => ci.Id);
            entity.Property(ci => ci.Id).HasColumnName("id").ValueGeneratedOnAdd();

            entity.Property(ci => ci.OwnerId).HasColumnName("owner_id");
            entity.Property(ci => ci.CoWorkerEmail).HasColumnName("co_worker_email").IsRequired().HasMaxLength(150);
            entity.Property(ci => ci.Status).HasColumnName("status").IsRequired().HasMaxLength(20).HasDefaultValue("WAITING");
            entity.Property(ci => ci.CreatedAt).HasColumnName("created_at");
            entity.Property(ci => ci.ExpiresAt).HasColumnName("expires_at");

            entity.HasOne(ci => ci.Owner)
                .WithMany()
                .HasForeignKey(ci => ci.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(ci => new { ci.OwnerId, ci.CoWorkerEmail, ci.Status }).IsUnique();
        });

        modelBuilder.Entity<ConversationTag>(entity =>
        {
            entity.ToTable("conversation_tags", "dbo");
            entity.HasKey(ct => ct.Id);
            entity.Property(ct => ct.Id).HasColumnName("id").ValueGeneratedOnAdd();

            entity.Property(ct => ct.UserId).HasColumnName("user_id").IsRequired(false);

            entity.Property(ct => ct.TagName).HasColumnName("tag_name").IsRequired().HasMaxLength(50);
            entity.Property(ct => ct.ColorHex).HasColumnName("color_hex").HasDefaultValue("#808080").HasMaxLength(7);
            entity.Property(ct => ct.CreatedAt).HasColumnName("created_at");

            entity.HasIndex(ct => new { ct.UserId, ct.TagName }).IsUnique();

            entity.HasOne(ct => ct.User)
                .WithMany()
                .HasForeignKey(ct => ct.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ConversationTagAssignment>(entity =>
        {
            entity.ToTable("conversation_tag_assignments", "dbo");
            entity.HasKey(cta => new { cta.UserId, cta.ConversationId });

            entity.Property(cta => cta.UserId).HasColumnName("user_id");
            entity.Property(cta => cta.ConversationId).HasColumnName("conversation_id");
            entity.Property(cta => cta.TagId).HasColumnName("tag_id");
            entity.Property(cta => cta.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(cta => cta.User)
                .WithMany()
                .HasForeignKey(cta => cta.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cta => cta.Conversation)
                .WithMany(c => c.TagAssignments) 
                .HasForeignKey(cta => cta.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cta => cta.ConversationTag)
                .WithMany()
                .HasForeignKey(cta => cta.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<MessageReadStatus>(entity =>
        {
            entity.ToTable("message_read_status", "dbo");
            entity.HasKey(mrs => new { mrs.ConversationId, mrs.UserId });

            entity.Property(mrs => mrs.ConversationId).HasColumnName("conversation_id");
            entity.Property(mrs => mrs.UserId).HasColumnName("user_id");
            entity.Property(mrs => mrs.LastReadMessageId).HasColumnName("last_read_message_id");
            entity.Property(mrs => mrs.LastReadAt).HasColumnName("last_read_at");

            entity.HasOne(mrs => mrs.Conversation)
                .WithMany()
                .HasForeignKey(mrs => mrs.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(mrs => mrs.User)
                .WithMany()
                .HasForeignKey(mrs => mrs.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(mrs => mrs.LastReadMessage)
                .WithMany()
                .HasForeignKey(mrs => mrs.LastReadMessageId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("auditlogs", "dbo");
            entity.HasKey(al => al.Id);
            entity.Property(al => al.Id).HasColumnName("id").ValueGeneratedOnAdd();

            entity.Property(al => al.UserId).HasColumnName("userid");
            entity.Property(al => al.CoWorkerId).HasColumnName("coworkerid");
            entity.Property(al => al.Timestamp).HasColumnName("timestamp");
            entity.Property(al => al.Action).HasColumnName("action").IsRequired();
        });
    }
}