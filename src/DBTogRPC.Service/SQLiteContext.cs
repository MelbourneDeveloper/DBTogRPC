
using Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DBTogRPCService
{
    public class SQLiteContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public SQLiteContext()
        {
            //TODO: Make async
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = new SqliteConnection($"Data Source=Test.db");
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = $"PRAGMA foreign_keys = ON;";
            command.ExecuteNonQuery();

            optionsBuilder.UseSqlite(connection);

            base.OnConfiguring(optionsBuilder);
        }
    }
}
