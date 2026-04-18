using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TrainEase.APIs.Extensions;
using TrainEase.Core.Application;
using TrainEase.Infrastructure.Presistance;
using TrainEase.Infrastructure.Presistance._Data;
using TrainEase.Infrastructure.Presistance.Seeding;

namespace TrainEase.APIs
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "TrainEase API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "ادخل الـ Token فقط هنا"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] {}
                    }
                });
            });
            ;
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddPersistenceServices(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                        "http://127.0.0.1:5500",
                        "http://192.168.47.1:5000",
                        "https://train-ease.netlify.app"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await IdentitySeeder.SeedRolesAsync(roleManager);

                    var context = services.GetRequiredService<AppDbContext>();
                    await context.Database.MigrateAsync();
                    //await SeedData.SeedAsync(context);

                    Console.WriteLine("--> Success: Database Migrated & Seeded!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during seeding: {ex.Message}");
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred during migration/seeding.");
                    Console.WriteLine($"--> Startup Error: {ex.Message}");
                }
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrainEase API V1");
                c.RoutePrefix = string.Empty;
            });

            // يخلي /swagger و /swagger/index.html يشتغلوا برضه
            app.MapGet("/swagger", context =>
            {
                context.Response.Redirect("/");
                return Task.CompletedTask;
            });

            app.MapGet("/swagger/index.html", context =>
            {
                context.Response.Redirect("/");
                return Task.CompletedTask;
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}