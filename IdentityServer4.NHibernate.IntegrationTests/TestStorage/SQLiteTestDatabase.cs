using System;
using System.Data.SQLite;

namespace IdentityServer4.NHibernate.IntegrationTests.TestStorage
{
    /// <summary>
    /// A file-based SQLite database used for testing.
    /// </summary>
    internal class SQLiteTestDatabase : TestDatabase
    {
        private readonly string _databaseFileName;

        public SQLiteTestDatabase(string databaseFileName, global::NHibernate.Cfg.Configuration config)
            : base(databaseFileName, string.Empty, config)
        {
            _databaseFileName = databaseFileName ?? throw new ArgumentNullException(nameof(databaseFileName));
        }

        public override void CreateEmptyDatabase()
        {
            SQLiteConnection.CreateFile(_databaseFileName);
        }

        public override void Drop() { }
    }
}
