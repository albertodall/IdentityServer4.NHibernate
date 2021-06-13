using MySql.Data.MySqlClient;

namespace IdentityServer4.NHibernate.IntegrationTests.TestStorage
{
    internal class MySqlTestDatabase : TestDatabase
    {
        public MySqlTestDatabase(string connectionString, string databaseName, global::NHibernate.Cfg.Configuration config) 
            : base(connectionString, databaseName, config)
        { }

        public override void CreateEmptyDatabase()
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                using (var command = new MySqlCommand($@"CREATE DATABASE {DatabaseName};", conn))
                {
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public override void DropIfExists()
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                using (var command = new MySqlCommand($@"DROP DATABASE IF EXISTS {DatabaseName};", conn))
                {
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
    }
}
