using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using MottuGestor.Domain.Entities;
using MottuGestor.Infrastructure.Context;
using MottuGestor.Infrastructure.Repositories;

namespace GestMottu.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables(); // lẽ ConnectionStrings:DefaultConnection do App Service

            builder.Services.AddControllers()
                .AddJsonOptions(o => { o.JsonSerializerOptions.WriteIndented = true; });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = builder.Configuration["Swagger:Title"] ?? "GestMottu API",
                    Description = "API RESTful para gestão de motos com Clean Architecture e DDD",
                    Contact = new OpenApiContact { Name = "Equipe MottuGestor", Email = "contato@mottu.com.br" }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    x.IncludeXmlComments(xmlPath);
                }
            });

            builder.Services.AddDbContext<GestMottuContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null) // resiliente a transientes
                )
            );

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IMotoRepository, MotoRepository>();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();
            app.MapControllers();
            
            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.Run();
        }
    }
}
