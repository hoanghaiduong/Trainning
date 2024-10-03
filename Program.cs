using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Trainning.Common.Exceptions;
using Trainning.Common.Extensions;
using Trainning.Common.Filters;
using Trainning.Data;
using Trainning.Interfaces;
using Trainning.Models;
using Trainning.Repositories;
using Trainning.Services;


namespace Trainning;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
        // Đọc Issuer và Audience từ appsettings.json
        var issuer = builder.Configuration["AppSettings:Issuer"];
        var audience = builder.Configuration["AppSettings:Audience"];
        // Đọc cấu hình secret keys từ appsettings.json
        var access_secret = builder.Configuration["AppSettings:AccessTokenSecret"];
        var refresh_secret = builder.Configuration["AppSettings:RefreshTokenSecret"];
        builder.Services.AddDbContext<ApplicationDbContext>(o =>
        {
            o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        }
        );
        builder.Services.AddExceptionHandler<ExceptionHandler>();
        builder.Services.AddScoped<ApplicationDbContextInitialize>();
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
        builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        builder.Services.AddScoped<IFileUploadService, FileUploadService>();
        builder.Services.AddSingleton<JwtService>();
        //  // Đăng ký SQLHelper
        // builder.Services.AddScoped<SQLHelperNoContext>(provider =>
        // {
        //     var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        //     return new SQLHelperNoContext(connectionString);
        // });
        builder.Services.AddSwaggerGen((configure) =>
              {
                  configure.SchemaFilter<EnumSchemaFilter>();
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


        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            options.JsonSerializerOptions.WriteIndented = true;
        });

        var app = builder.Build();
        await app.AddInitialDatabaseAsync();
        app.UseStaticFiles();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapControllers();
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.AddWebService();

        await app.RunAsync();
    }
}
