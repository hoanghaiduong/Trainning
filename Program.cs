using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        builder.Services.AddDbContext<ApplicationDbContext>(o =>
        {
            o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        }
        );
        builder.Services.AddExceptionHandler<ExceptionHandler>();
        builder.Services.AddScoped<ApplicationDbContextInitialize>();
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        builder.Services.AddScoped<IFileUploadService, FileUploadService>();
        //  // Đăng ký SQLHelper
        // builder.Services.AddScoped<SQLHelperNoContext>(provider =>
        // {
        //     var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        //     return new SQLHelperNoContext(connectionString);
        // });

        builder.Services.AddSwaggerGen(c =>
        {
            // Enable enum string representation
            c.SchemaFilter<EnumSchemaFilter>();
        });
        // builder.Services.AddControllers().AddNewtonsoftJson(options =>
        // {
        //     options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        // });
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
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
