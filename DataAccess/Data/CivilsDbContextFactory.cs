using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Data
{
    public class CivilsDbContextFactory : IDesignTimeDbContextFactory<CivilsDbContext>
    {
        public CivilsDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(@"D:\CivilsAssistance\BackEnd\CivilsAssistance_API")
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<CivilsDbContext>();
            var connectionString = configuration.GetConnectionString("ConString");
            optionsBuilder.UseSqlServer(connectionString);

            return new CivilsDbContext(optionsBuilder.Options);
        }
    }
}