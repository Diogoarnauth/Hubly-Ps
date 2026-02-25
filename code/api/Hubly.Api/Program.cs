using Microsoft.EntityFrameworkCore;
using Hubly.Infrastructure.Data;
using Hubly.Domain.Entities; 
using Hubly.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HublyDbContext>(options =>
    options.UseNpgsql(connectionString));

var domainConfig = new UsersDomainConfig 
{
    TokenSizeInBytes = 32,
    TokenTtl = TimeSpan.FromHours(24),
    TokenRollingTtl = TimeSpan.FromHours(1),
    MaxTokensPerUser = 3,
    MinUsernameLength = 3,
    MinPasswordLength = 8
};
builder.Services.AddSingleton(domainConfig);

builder.Services.AddScoped<UsersDomain>();

builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers(); 

app.Run();