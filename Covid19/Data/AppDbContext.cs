using Covid19.Models;
using Microsoft.EntityFrameworkCore;

namespace Covid19.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }

        public DbSet<CovidClass> Covids { get; set; }
    }
}
