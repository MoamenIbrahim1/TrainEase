using Microsoft.AspNetCore.Identity;
using TrainEase.APIs.Extensions;
using TrainEase.Core.Application;
using TrainEase.Infrastructure.Presistance;
using TrainEase.Infrastructure.Presistance.Seeding;
namespace TrainEase.APIs
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddPersistenceServices(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                 "http://127.0.0.1:5500",      // Frontend
                 "http://192.168.47.1:5000"    // Mobile
                    )
                           .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });


            var app = builder.Build();
            // Seed Roles
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await IdentitySeeder.SeedRolesAsync(roleManager);
                }
                catch (Exception ex)
                {
                    // هيطبع الخطأ في الـ Console بس مش هيوقع البرنامج
                    Console.WriteLine($"Error during seeding: {ex.Message}");
                }
            }

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrainEase API V1");

                c.RoutePrefix = string.Empty;
            });

            //}

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
