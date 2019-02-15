using System;

namespace IdentityServer4.NHibernate.IntegrationTests.TestStorage
{
    using System.Data.SQLite;

    /// <summary>
    /// A file-based SQLite database used for testing.
    /// </summary>
    internal class SQLiteTestDatabase : TestDatabase
    {
        private readonly string _databaseFileName;

        public SQLiteTestDatabase(string databaseFileName, global::NHibernate.Cfg.Configuration config)
            : base(config)
        {
            _databaseFileName = databaseFileName ?? throw new ArgumentNullException(nameof(databaseFileName));
        }

        public override void Create()
        {
            SQLiteConnection.CreateFile(_databaseFileName);
            base.Create();
        }
    }
}
