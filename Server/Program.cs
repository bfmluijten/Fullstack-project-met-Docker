// Code hiervoor en hierna laten staan.
// De code hieronder is toevoeging. Zorg dat je deze ook hebt.

builder.Services.AddLogging(builder =>
    builder.AddDebug()
        .AddConsole()
        .SetMinimumLevel(LogLevel.Information));

// Prefer environment variable for connection string if present
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__{{JouwConnectieNaam}}")
    ?? builder.Configuration.GetConnectionString("{{JouwConnectieNaam}}");
Console.WriteLine($"Using connection string: {connectionString}");

builder.Services.AddDbContext<MyCarsDbContext>(options =>
  options.UseSqlServer(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Bij het opstarten zal hij de database migraties uitvoeren.
    var db = scope.ServiceProvider.GetRequiredService<MyCarsDbContext>();
    db.Database.Migrate();
}