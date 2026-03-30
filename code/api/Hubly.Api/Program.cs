using Hubly.api.Pipeline;
using Hubly.api.Middlewares;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Infrastructure;
using Hubly.api.Services.Encoder;
using Microsoft.EntityFrameworkCore;
using Hubly.api.Services.Interfaces;
using Hubly.api.Services;
using Mapster;
using Hubly.api.DTOs;

var builder = WebApplication.CreateBuilder(args);

var userDomainConfig = new UsersDomainConfig
{
    MinUsernameLength = 3,
    MinPasswordLength = 8
};
var creatorDomainConfig = new CreatorsDomainConfig
{
    MinArtitisticNameLength = 2
};
var companyDomainConfig = new CompaniesDomainConfig
{

};

builder.Services.AddSingleton(userDomainConfig);
builder.Services.AddSingleton(creatorDomainConfig);
builder.Services.AddSingleton(companyDomainConfig);


builder.Services.AddScoped<UsersDomain>();
builder.Services.AddScoped<CreatorsDomain>();
builder.Services.AddScoped<CompaniesDomain>();
builder.Services.AddScoped<TokenProcessor>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();
//pipeline configuration
builder.Services.AddControllers(options =>
{
    options.Filters.Add<RequireAuthenticationAttribute>();
})
.AddMvcOptions(options =>
{
    options.ModelBinderProviders.Insert(0, new AuthenticatedUserModelBinderProvider());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//CORS Service
builder.Services.AddCors(options => //todo() maybe ngnix configuration
{
    options.AddPolicy("AllowFrontend",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

//Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICreatorService, CreatorService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ISocialPlatformService, SocialPlatformService>();


//Encoders
builder.Services.AddScoped<ITokenEncoder, Sha256TokenEncoder>();
builder.Services.AddScoped<IPasswordEncoder, Sha256PasswordEncoder>();

//Repositories
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailConfirmationRepository, EmailConfirmationRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICreatorRepository, CreatorRepository>();
builder.Services.AddScoped<ISocialPlatformRepository, SocialPlatformRepository>();
builder.Services.AddScoped<ICreatorSocialRepository, CreatorSocialRepository>();
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();


TypeAdapterConfig<Company, CompanyCreateOutputModel>
    .NewConfig()
    .Map(dest => dest.Sector, src => src.Sector != null ? src.Sector.SectorName : string.Empty)
    .Map(dest => dest.SubSector, src => src.SubSector != null ? src.SubSector.SubSectorName : null);

TypeAdapterConfig<Company, CompanyEditOutputModel>
    .NewConfig()
    .Map(dest => dest.Sector, src => src.Sector.SectorName)
    .Map(dest => dest.SubSector, src => src.SubSector.SubSectorName);

TypeAdapterConfig<Company, GetCompanyOutputModel>
    .NewConfig()
    .Map(dest => dest.Sector, src => src.Sector != null ? src.Sector.SectorName : string.Empty)
    .Map(dest => dest.SubSector, src => src.SubSector != null ? src.SubSector.SubSectorName : null);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HublyDbContext>(options =>
    options.UseNpgsql(connectionString));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// Middlewares
app.UseMiddleware<ExceptionMiddleware>();


app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// app.UseMiddleware<ExceptionMiddleware>(); //TODO() we dont have

// if (app.Environment.IsDevelopment())
// {        
//     app.UseSwagger();
//     app.UseSwaggerUI(c => {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hubly API V1");
//         c.RoutePrefix = ""; 
//     });
// }
