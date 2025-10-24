using Microsoft.EntityFrameworkCore;
using Server.Database;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Code hiervoor en hierna laten staan.
            // De code hieronder is toevoeging. Zorg dat je deze ook hebt.

            builder.Services.AddLogging(builder =>
                builder.AddDebug()
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Information));

            // Prefer environment variable for connection string if present
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__{{JouwConnectieNaam}}")
                ?? builder.Configuration.GetConnectionString("{{JouwConnectieNaam}}");
            connectionString = "Server=host.docker.internal,1433;Database=Hospital;User=sa;Password=C0mplexW@chtw00rd;TrustServerCertificate=True;Encrypt=True";
            Console.WriteLine($"Using connection string: {connectionString}");

            builder.Services.AddDbContext<HospitalDbContext>(options =>
              options.UseSqlServer(connectionString));

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                // Bij het opstarten zal hij de database migraties uitvoeren.
                var db = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
                db.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}