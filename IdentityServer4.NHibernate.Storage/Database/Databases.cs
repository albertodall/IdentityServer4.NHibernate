﻿using IdentityServer4.NHibernate.Extensions;
using NHibernate.Bytecode;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;

namespace IdentityServer4.NHibernate.Database
{
    using global::NHibernate.Cfg;

    /// <summary>
    /// NHibernate configurations for supported databases.
    /// </summary>
    public static class Databases
    {
        /// <summary>
        /// Database configuration for SQL Server 2008 as backing storage.
        /// </summary>
        public static Configuration SqlServer2008()
        {
            var cfg = new Configuration();
            cfg.Proxy(p => p.ProxyFactoryFactory<StaticProxyFactoryFactory>());
            cfg.DataBaseIntegration(db =>
            {
                db.Dialect<MsSql2008Dialect>();
                db.Driver<Sql2008ClientDriver>();
                db.BatchSize = 100;
                db.PrepareCommands = true;
            });

            return cfg;
        }

        /// <summary>
        /// Database configuration for SQL Server 2012+ as backing storage.
        /// </summary>
        public static Configuration SqlServer2012()
        {
            var cfg = new Configuration();
            cfg.Proxy(p => p.ProxyFactoryFactory<StaticProxyFactoryFactory>());
            cfg.DataBaseIntegration(db =>
            {
                db.Dialect<MsSql2012Dialect>();
                db.Driver<Sql2008ClientDriver>();
                db.BatchSize = 100;
                db.PrepareCommands = true;
            });

            return cfg;
        }

        /// <summary>
        /// Database configuration for SQLite as backing storage.
        /// </summary>
        public static Configuration SQLite()
        {
            var cfg = new Configuration();
            cfg.Proxy(p => p.ProxyFactoryFactory<StaticProxyFactoryFactory>());
            cfg.DataBaseIntegration(db =>
            {
                db.Dialect<SQLiteDialect>();
                db.Driver<SQLite20Driver>();
                db.BatchSize = 100;
                db.PrepareCommands = true;
            });

            return cfg;
        }

        /// <summary>
        /// Database configuration for in-memory SQLite as backing storage.
        /// </summary>
        public static Configuration SQLiteInMemory()
        {
            return SQLite()
                .UsingConnectionString("FullUri=file:memorydb.db?mode=memory&cache=shared")
                .SetProperty("connection.release_mode", "on_close");
        }

        /// <summary>
        /// Database configuration for PostgreSQL 8.3 as backing storage.
        /// </summary>
        public static Configuration PostgreSQL83()
        {
            var cfg = new Configuration();
            cfg.Proxy(p => p.ProxyFactoryFactory<StaticProxyFactoryFactory>());
            cfg.DataBaseIntegration(db =>
            {
                db.Dialect<PostgreSQL83Dialect>();
                db.Driver<NpgsqlDriver>();
                db.BatchSize = 100;
                db.PrepareCommands = true;
            });

            return cfg;
        }
    }
}
