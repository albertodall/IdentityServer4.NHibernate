# IdentityServer4.NHibernate
IdentityServer4.NHibernate is a persistence layer for IdentityServer 4 configuration data that uses [NHibernate](https://github.com/nhibernate/nhibernate-core) to access the storage layer.
It's heavily based on the [Entity Framework Provider](https://github.com/IdentityServer/IdentityServer4.EntityFramework), in order to implement all the features required by IdentityServer.

I decided to not use [FluentNHibernate](https://github.com/nhibernate/fluent-nhibernate) for mappings, so to have the least number of dependencies.
All mappings are defined using the integrated _Mapping By Code_ feature and the _Loquacious API_.

# Current status
### NuGet
[![NuGet](https://img.shields.io/nuget/v/IdentityServer4.Contrib.NHibernate.svg)](https://www.nuget.org/packages/IdentityServer4.Contrib.NHibernate/)


# Configuration
To configure the provider, simply add it to the IdentityServer configuration in the `Startup` class' `ConfigureServices()` method.

```c#
services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddNHibernateStores(
        Databases.SqlServer2012()
            .UsingConnectionString(Configuration["ConnectionStrings:Default"])
            .EnableSqlStatementsLogging(),
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
2. Show all the generated SQL statements in the console (`EnableSqlStatementsLogging()`).
3. Put all the configuration store objects and operational store objects in the `dbo` schema.


# Supported Databases

Currently, the provider directly supports the following databases:

- SQL Server 2008
- SQL Server 2012 or later.
- [SQLite](https://www.sqlite.org).
- [SQLite](https://www.sqlite.org) in-memory (not suitable for production).
- [PostgreSQL](https://www.postgresql.org)
- [MySQL](https://dev.mysql.com)

It's obviously possible to use every database supported by NHibernate.

Remember to add the required libraries to your IdentityServer project, in order to support the database you're going to use:

- For [SQL Server](https://www.nuget.org/packages/System.Data.SqlClient): `Install-Package System.Data.SqlClient`
- For [SQLite](https://www.nuget.org/packages/System.Data.SQLite.Core): `Install-Package System.Data.SQLite.Core`
- For [PostgreSQL](https://www.nuget.org/packages/Npgsql): `Install-Package Npgsql`
- For [MySQL](https://www.nuget.org/packages/MySql.Data): `Install-Package MySql.Data`

# Database Schema Creation
In the package's _Scripts_ folder you will find the schema creation scripts for every supported database.
You can use these scripts to create the database objects in the database you're going to use. 
Before executing, remember to modify them accordingly to your database schema.

# Additional configuration options
The `ConfigurationStoreOptions` class has an additional `EnableConfigurationStoreCache` option that enables the default cache for the configuration store.

# Known Issues
1. Like the official Entity Framework provider, also this one splits the storage in two logical stores:

    - _Configuration Store_
    - _Persisted Grant Store_

    The difference here is that the Entity Framework provider configures two `DbContext` instances, one for each store, so theoretically, you could put the each store in a dedicated database;
    with this provider, both stores are managed by the same NHibernate *SessionFactory*, so they have to be created in the same database. It's possible to put them in different database schemas, but the database has to be the same.

2. SQLite in-memory databases are "_per-connection_", so different NHibernate sessions use different databases.
That's why it's not recommended to use this provider in production with an in-memory SQLite backing store.

# Contributors
I wish to thank all the contributors to this project:

- [Ivan](https://github.com/mtivan)

# Acknowledgements
This package has been built using these awesome Open Source projects:

- [.NET Core](https://github.com/dotnet)
- [AutoMapper](https://github.com/AutoMapper)
- [FluentAssertions](https://github.com/fluentassertions)
- [GitVersion](https://github.com/GitTools/GitVersion)
- [Moq](https://github.com/moq)
- [NHibernate](https://github.com/NHibernate)
- [XUnit](https://github.com/XUnit)

And obviously, [IdentityServer](https://github.com/IdentityServer). :-)

Thanks everybody for the great work!
