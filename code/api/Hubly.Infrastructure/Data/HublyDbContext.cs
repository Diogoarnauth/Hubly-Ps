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
    public DbSet<SubSector> SubSectors { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<EmailConfirmation> EmailConfirmations { get; set; }
    public DbSet<ProfileViewHistory> ProfileViewsHistory { get; set; }



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

            // Mapeia a propriedade Id para a coluna user_id do SQL
            entity.Property(c => c.Id).HasColumnName("user_id").ValueGeneratedNever();

            entity.HasOne(c => c.User)
                  .WithOne(u => u.Creator)
                  .HasForeignKey<Creator>(c => c.Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Sector>(entity =>
        {
            entity.ToTable("sectors", "dbo");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id).HasColumnName("id");
            entity.Property(s => s.SectorName).HasColumnName("sector_name");

            entity.HasIndex(s => s.SectorName).IsUnique();
        });


        modelBuilder.Entity<SubSector>(entity =>
        {
            entity.ToTable("sub_sectors", "dbo");
            entity.HasKey(ss => ss.Id);

            entity.Property(ss => ss.Id).HasColumnName("id");
            entity.Property(ss => ss.SubSectorName).HasColumnName("subsector_name");

            entity.Property(ss => ss.SectorId).HasColumnName("sector_id");

            entity.HasOne(ss => ss.Sector)
                  .WithMany(s => s.SubSectors)
                  .HasForeignKey(ss => ss.SectorId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(ss => new { ss.SectorId, ss.SubSectorName }).IsUnique();
        });

        // CONFIGURAÇÃO: Company (PK é FK do User)
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("companies", "dbo");
            entity.HasKey(c => c.Id);

            // Mapeia a propriedade Id para a coluna user_id do SQL
            entity.Property(c => c.Id).HasColumnName("user_id").ValueGeneratedNever();

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
            entity.Property(h => h.ViewedCreatorId).HasColumnName("viewed_creator_id");
            entity.Property(h => h.ViewedAt).HasColumnName("viewed_at");

            entity.HasOne(h => h.ViewedCompany)
          .WithMany()
          .HasForeignKey(h => h.ViewedCompanyId);

            entity.HasOne(h => h.ViewedCreator)
                  .WithMany()
                  .HasForeignKey(h => h.ViewedCreatorId);
        });
    }
}