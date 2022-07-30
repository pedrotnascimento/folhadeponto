using Microsoft.EntityFrameworkCore;
using Repository.Tables;

namespace Repository
{

    public class FolhaDePontoContext : DbContext
    {
        //public DbSet<User> Users { get; set; }
        public DbSet<TimeAllocation> TimeAllocations { get; set; }
        public DbSet<TimeMoment> TimeMoments { get; set; }

        public string DbPath { get; }

        public FolhaDePontoContext(DbContextOptions<FolhaDePontoContext> options)
            : base(options)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "FolhaDePonto.db");
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseSqlite($"Data Source=:memory:");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TimeMoment>();
            modelBuilder.Entity<TimeAllocation>();

        }

    }
}