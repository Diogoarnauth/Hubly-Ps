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
using Hubly.api.Services.Hubs;
using Hubly.api.Problems;

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
builder.Services.AddSingleton<Hubly.api.Infrastructure.Audit.AuditQueue>();
builder.Services.AddHostedService<Hubly.api.BackgroundServices.AuditBackgroundProcessor>();


builder.Services.AddScoped<UsersDomain>();
builder.Services.AddScoped<CreatorsDomain>();
builder.Services.AddScoped<CompaniesDomain>();
builder.Services.AddScoped<TokenProcessor>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();
//pipeline configuration
// Pipeline configuration e Interceção de Erros de DTOs antes do Controller
builder.Services.AddControllers(options =>
{
    options.Filters.Add<RequireAuthenticationAttribute>();
})
.AddMvcOptions(options =>
{
    options.ModelBinderProviders.Insert(0, new AuthenticatedUserModelBinderProvider());
})
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errorMessage = actionContext.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .FirstOrDefault() ?? "Validation failed.";

        return ProblemResponse.ValidationError(errorMessage).ToResponse();
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//CORS Service
builder.Services.AddCors(options => //todo() maybe ngnix configuration
{
    options.AddPolicy("AllowFrontend",
        builder =>
        {
            builder.WithOrigins("http://localhost:3001")
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
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IConversationTagService, ConversationTagService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICoWorkerService, CoWorkerService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddSignalR(); //todo()

//Encoders
builder.Services.AddScoped<ITokenEncoder, Sha256TokenEncoder>();
builder.Services.AddScoped<IPasswordEncoder, BCryptPasswordEncoder>();

//Repositories
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailConfirmationRepository, EmailConfirmationRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICreatorRepository, CreatorRepository>();
builder.Services.AddScoped<ISocialPlatformRepository, SocialPlatformRepository>();
builder.Services.AddScoped<ICreatorSocialRepository, CreatorSocialRepository>();
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<ICoWorkerRepository, CoWorkerRepository>();




TypeAdapterConfig<CreatorSocialProfile, SocialProfileOutputModel>
    .NewConfig()
    .Map(dest => dest.PlatformName, src => src.Platform.NamePlatform);
TypeAdapterConfig<CreatorSocialProfile, GetSocialProfileOutputModel>
    .NewConfig()
    .Map(dest => dest.Sectors, src => src.Sectors.Select(s => s.SectorName))
    .Map(dest => dest.PlatformName, src => src.Platform.NamePlatform);
TypeAdapterConfig<Company, CompanyOutputModel>
    .NewConfig()
    .Map(dest => dest.Sectors, src => src.Sectors.Select(s => s.SectorName));
TypeAdapterConfig<Company, CompanyOutputModel>
    .NewConfig()
    .Map(dest => dest.Sectors, src => src.Sectors.Select(s => s.SectorName));
TypeAdapterConfig<CoWorker, GetMyCoWorkerWithEmailOutputModel>
    .NewConfig()
    .Map(dest => dest.CoWorkerEmail, src => src.User.Email);

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

app.MapHub<HublyHub>("/api/hubly-events");
//app.UseHttpsRedirection();

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
