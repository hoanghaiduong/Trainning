using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Trainning.Models;
using Trainning.Services;

namespace Trainning;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

        // Đọc Issuer và Audience từ appsettings.json
        var issuer = builder.Configuration["AppSettings:Issuer"];
        var audience = builder.Configuration["AppSettings:Audience"];
        // Đọc cấu hình secret keys từ appsettings.json
        var access_secret = builder.Configuration["AppSettings:AccessTokenSecret"];
        var refresh_secret = builder.Configuration["AppSettings:RefreshTokenSecret"];
        Console.WriteLine(issuer+audience+access_secret+refresh_secret);
        builder.Services.AddAuthentication(op =>
        {
            op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(access_secret!)),
                ValidateIssuer = true,
                ValidIssuer = issuer,  // Thêm Issuer
                ValidateAudience = true,
                ValidAudience = audience, // Thêm Audience
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }).AddJwtBearer("RefreshToken", y =>
            {
                y.SaveToken = false;
                y.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refresh_secret!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen((configure) =>
        {
            configure.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer ey...')",
            });
            configure.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                     new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        
        builder.Services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        });
        builder.Services.AddSingleton<JwtServices>();
        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapControllers();
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.Run();
    }
}
