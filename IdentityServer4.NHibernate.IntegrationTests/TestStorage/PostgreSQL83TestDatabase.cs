using Npgsql;

namespace IdentityServer4.NHibernate.IntegrationTests.TestStorage
{
    internal class PostgreSQL83TestDatabase : TestDatabase
    {
        private readonly string _connectionString;
        private readonly string _databaseName;

        public PostgreSQL83TestDatabase(string connectionString, string databaseName, global::NHibernate.Cfg.Configuration config) 
            : base(config)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
        }

        public override void Create()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                using (var command = new NpgsqlCommand(@$"CREATE DATABASE {_databaseName} WITH OWNER=postgres ENCODING='UTF8' CONNECTION LIMIT=-1", conn))
                {
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }
                base.Create();
            }
        }

        public override void Drop()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                using (var command = new NpgsqlCommand(@$"DROP DATABASE IF EXISTS {_databaseName}", conn))
                {
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
    }
}
