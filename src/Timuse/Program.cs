using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using Timuse;
using Timuse.Entity;
using Timuse.Host;

var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
var dbPath = Path.Combine(appData, "Timuse", "Timuse.db");

var host = WpfHost.Build<App, MainWindow>(args, hostBuilder =>
{
    hostBuilder.ConfigureServices((context, services) =>
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });
    });
});

using var scope = host.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
if (dbContext.Database.GetPendingMigrations().Any())
{
    dbContext.Database.Migrate();
}

host.Run();
