## Build 2.1.4
_Release notes - IdentityServer4.NHibernate - Version 2.1.4_

__New features__
- No new features

__Resolved issues__
- [#15](https://github.com/albertodall/IdentityServer4.NHibernate/issues/15) - SqlException: Incorrect syntax near the keyword 'Key'. ([@Hiller](https://github.com/Hiller)) 

## Build 2.1.3
_Release notes - IdentityServer4.NHibernate - Version 2.1.3_

__New features__
- Native support for [PostgreSQL](https://www.postgresql.org) and [MySQL](https://dev.mysql.com) databases.

__Resolved issues__
- [#13](https://github.com/albertodall/IdentityServer4.NHibernate/issues/13) - MySql support ([@pigsi](https://github.com/pigsi)) 

## Build 2.1.2
_Release notes - IdentityServer4.NHibernate - Version 2.1.2_

__New features__
- None

__Resolved Issues__
- [#11](https://github.com/albertodall/IdentityServer4.NHibernate/issues/11) - Session is closed exception ([@lcetinapta](https://github.com/lcetinapta)) 

## Build 2.1.1
_Release notes - IdentityServer4.NHibernate - Version 2.1.1_

__New features__
- None

__Resolved Issues__
- [#8](https://github.com/albertodall/IdentityServer4.NHibernate/issues/8) - Invalid DI configuration ([@mtivan](https://github.com/mtivan)) 

## Build 2.1.0
_Release notes - IdentityServer4.NHibernate - Version 2.1.0_

__New features__
- Support for IdentityServer4 version 3.1
- Azure DevOps Multi-stage Pipeline deployment

==breaking change==
This version supports **only** `netcoreapp31` targeted projects. If your project doesn't target this .NET Core version, you can:
- Use version [2.0.x](https://www.nuget.org/packages/IdentityServer4.Contrib.NHibernate/2.0.0) if your project targets `netcoreapp3`.
- Use version [1.1.x](https://www.nuget.org/packages/IdentityServer4.Contrib.NHibernate/1.1.0) for other project targets.

__Resolved issues__
- No issues
