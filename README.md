# IdentityServer4.NHibernate
IdentityServer4.NHibernate is a persistence layer for IdentityServer 4 configuration data that uses [NHibernate](https://github.com/nhibernate/nhibernate-core) to access the storage layer.
It's heavily based on the [Entity Framework Provider](https://github.com/IdentityServer/IdentityServer4.EntityFramework), in order to implement all the features required by IdentityServer.

# Current status
[![Build status](https://dev.azure.com/albertod/IdentityServer4.NHibernate/_apis/build/status/IdentityServer4.NHibernate-CI)](https://dev.azure.com/albertod/IdentityServer4.NHibernate/_build/latest?definitionId=3)

# Configuration
To configure the provider, simply add it to the IdentityServer configuration in the `Startup` class' `ConfigureServices()` method.

```c#
services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddNHibernateStores(
        Databases.SqlServer2012(showGeneratedSql: true)
            .UsingConnectionString(Configuration["ConnectionStrings:Default"]),
        cfgStore =>
        {
            cfgStore.DefaultSchema = "dbo";
        },
        opStore =>
        {
            opStore.DefaultSchema = "dbo";
        }
    )
```
In this example, we are configuring the NHibernate provider in order to:

1. Store the configuration data in a [SQL Server](https://www.microsoft.com/en-us/sql-server/) 2012 (or later) database, whose connection string is the one called `Default` in the `appsettings.json` file
2. Put all the configuration store objects and operational store objects in the `dbo` schema.
3. Show all the generated SQL statements in the console (`showGeneratedSql: true`).

# Supported Databases

Currently, the provider directly supports the following databases:

- SQL Server 2008
- SQL Server 2012 or later.
- [SQLite](https://www.sqlite.org).
- [SQLite](https://www.sqlite.org) in-memory (not suitable for production).

It's obviously possible to use every database supported by NHibernate.

Remember to add the required libraries to your IdentityServer project, in order to support the database you're going to use:

- For [SQL Server](https://www.nuget.org/packages/System.Data.SqlClient): `Install-Package System.Data.SqlClient`
- For [SQLite](https://www.nuget.org/packages/System.Data.SQLite.Core): `Install-Package System.Data.SQLite.Core`

# Database Schema Creation
In the package's _Content_ folder you will find the schema creation scripts for every supported database.
You can use these scripts to create the database objects in the database you're going to use. 
Before executing, remember to modify them accordingly to your database schema.

# Known Issues
1. As the Entity Framework provider, also this one "splits" the storage in two logical stores:

  - _Configuration Store_
  - _Persisted Grant Store_

    The difference here is that the Entity Framework provider configures two `DbContext` instances, one for each store, so theoretically, you could put the each store in a dedicated database;
    in this provider, both stores are managed by the same NHibernate SessionFactory, so they have to be created in the same database. It's possible to put them in different schemas, but the database has to be the same.

2. SQLite in-memory databases are "_per-connection_", so different NHibernate sessions use different databases.
That's why it's not recommennded to use this provider in production with an in-memory SQLite backing store.

