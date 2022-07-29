using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace FolhaDePontoTest
{
    public class SharedDatabaseFixture : IDisposable
    {
        private readonly SqliteConnection connection;
        public SharedDatabaseFixture()
        {
            this.connection = new SqliteConnection("DataSource=:memory:");
        }
        public void Dispose() => this.connection.Dispose();
        public FolhaDePontoContext CreateContext()
        {
            var result = new FolhaDePontoContext(new DbContextOptionsBuilder<FolhaDePontoContext>()
                .UseSqlite(this.connection)
                .Options);
            result.Database.EnsureCreated();
            this.connection.Open();
            return result;
        }
    }
}
