
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Infrastructure.Data;
using dotnet_library_api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using dotnet_library_api.Application.Common;
using dotnet_library_api.Infrastructure.Services;
namespace dotnet_library_api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("LibraryDb");

        builder.Services.AddDbContext<LibraryDbContext>(options => options.UseNpgsql(connectionString));

        builder.Services.AddScoped<IBookRepository, BookRepository>();
        builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
        builder.Services.AddScoped<IGenreRepository, GenreRepository>();
        builder.Services.AddScoped<ILoanRepository, LoanRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ITokenService, TokenService>();

        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var jwtKey = jwtSettings["Key"];
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

        builder.Services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).
        AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
            };
        });

        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Library API", Version = "v1" });
            options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Library API", Version = "v2" });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "Library API v2");
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
