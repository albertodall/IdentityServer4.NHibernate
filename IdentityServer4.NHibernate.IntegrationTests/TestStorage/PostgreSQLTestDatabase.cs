using Npgsql;

namespace IdentityServer4.NHibernate.IntegrationTests.TestStorage
{
    internal class PostgreSQLTestDatabase : TestDatabase
    {
        public PostgreSQLTestDatabase(string connectionString, string databaseName, global::NHibernate.Cfg.Configuration config) 
            : base(connectionString, databaseName, config)
        { }

        public override void CreateEmptyDatabase()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                using (var command = new NpgsqlCommand($@"
CREATE DATABASE ""{DatabaseName}"" WITH OWNER=""postgres"" ENCODING='UTF8' CONNECTION LIMIT=-1;", conn))
                {
                    conn.Open();
                    command.ExecuteNonQuery(); 
                    conn.Close();
                }
            }
        }

        public override void DropIfExists()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                using (var command = new NpgsqlCommand($@"
DROP DATABASE IF EXISTS ""{DatabaseName}"" WITH (FORCE);", conn))
                {
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
    }
}
