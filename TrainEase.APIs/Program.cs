using TrainEase.APIs.Extensions;
using TrainEase.Infrastructure.Presistance;
namespace TrainEase.APIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddPersistenceServices(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://127.0.0.1:5500")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });


            var app = builder.Build();

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
