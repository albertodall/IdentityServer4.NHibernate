namespace IdentityServer4.NHibernate.Database
{
    using global::NHibernate.Bytecode;
    using global::NHibernate.Cfg;
    using global::NHibernate.Dialect;
    using global::NHibernate.Driver;

    public static class Databases
    {
        public static Configuration SqlServer2008()
        {
            var cfg = new Configuration();
            cfg.Proxy(p => p.ProxyFactoryFactory<DefaultProxyFactoryFactory>());
            cfg.DataBaseIntegration(db =>
            {
                db.Dialect<MsSql2008Dialect>();
                db.Driver<Sql2008ClientDriver>();
                db.BatchSize = 100;
                db.LogFormattedSql = true;
                db.PrepareCommands = true;
            });
            return cfg;
        }

        public static Configuration SqlServer2012()
        {
            var cfg = new Configuration();
            cfg.Proxy(p => p.ProxyFactoryFactory<DefaultProxyFactoryFactory>());
            cfg.DataBaseIntegration(db =>
            {
                db.Dialect<MsSql2012Dialect>();
                db.Driver<Sql2008ClientDriver>();
                db.BatchSize = 100;
                db.LogFormattedSql = true;
                db.PrepareCommands = true;
            });
            return cfg;
        }
    }
}
