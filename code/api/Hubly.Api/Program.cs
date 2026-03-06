using Microsoft.EntityFrameworkCore;
using Hubly.api.Pipeline;
using Hubly.api.Middlewares;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Data;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Infrastructure;
using Hubly.api.Services.Encoder;
using Hubly.api.Services.Interfaces;
using Hubly.api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

//BD
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HublyDbContext>(options =>
    options.UseNpgsql(connectionString));

var domainConfig = new UsersDomainConfig 
{
    MinUsernameLength = 3,
    MinPasswordLength = 8
};

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

builder.Services.AddScoped<TokenProcessor>();

//Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();

//Encoders
builder.Services.AddScoped<ITokenEncoder, Sha256TokenEncoder>();
builder.Services.AddScoped<IPasswordEncoder, Sha256PasswordEncoder>();

//Repositories
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

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
