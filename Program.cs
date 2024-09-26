using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Trainning.Data;
using Trainning.Helpers;
using Trainning.Interfaces;
using Trainning.Repositories;
using Trainning.Services;


namespace Trainning;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
         builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
         // Đăng ký SQLHelper
        builder.Services.AddScoped<SQLHelperNoContext>(provider =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            return new SQLHelperNoContext(connectionString);
        });


        // builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepositoryLocal<,>));

        builder.Services.AddScoped<BookService>();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        });

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
