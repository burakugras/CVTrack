using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CVTrack.Persistence.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 1) Persistence klasör yolunu al
            var persistencePath = Directory.GetCurrentDirectory(); 
            //    .../src/CVTrack.Persistence

            // 2) API projesinin yolunu hesapla
            var apiProjectPath = Path.GetFullPath(Path.Combine(persistencePath, "..", "CVTrack.Api"));

            // 3) API klasörünü baz alarak konfigürasyonu oluştur
            var config = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 4) Connection string’i alıp DbContextOptionsBuilder’ı oluştur
            var cs = config.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(cs);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
