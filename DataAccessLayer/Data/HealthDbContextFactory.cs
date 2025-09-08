using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccessLayer.Data
{
    public class HealthDbContextFactory : IDesignTimeDbContextFactory<HealthDbContext>
    {
        public HealthDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HealthDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=NazliDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");

            return new HealthDbContext(optionsBuilder.Options);
        }
    }
}
