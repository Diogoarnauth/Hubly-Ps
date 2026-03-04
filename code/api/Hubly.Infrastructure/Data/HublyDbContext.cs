using Microsoft.EntityFrameworkCore;
using Hubly.api.Domain.Entities;

namespace Hubly.api.Infrastructure.Data;

public class HublyDbContext : DbContext
{
    public HublyDbContext(DbContextOptions<HublyDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Creator> Creators { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Token> Tokens { get; set; }
    
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração TPT (Table Per Type)
        // Dizemos ao EF que estas tabelas herdam da tabela base 'users'
        modelBuilder.Entity<User>().ToTable("users", "dbo");
        modelBuilder.Entity<Creator>().ToTable("creators", "dbo");
        modelBuilder.Entity<Company>().ToTable("companies", "dbo");
        modelBuilder.Entity<Token>().ToTable("tokens", "dbo");

    }
}