﻿namespace IdentityServer4.NHibernate.Mappings.Storage
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    internal class PersistedGrantMap : ClassMapping<PersistedGrant>
    {
        public PersistedGrantMap()
        {
            Schema("dbo");
            Table("PersistedGrant");

            Id(p => p.ID, map => 
            {
                map.Column("Key");
                map.Generator(Generators.Assigned);
                map.Length(200);
            });

            Property(p => p.SubjectId, map => 
            {
                map.Length(200);
                map.Index("IX_Subject_Client_Type");
            });

            Property(p => p.ClientId, map => 
            {
                map.Length(200);
                map.NotNullable(true);
                map.Index("IX_Subject_Client_Type");
            });

            Property(p => p.Type, map =>
            {
                map.Length(50);
                map.NotNullable(true);
                map.Index("IX_Subject_Client_Type");
            });

            Property(p => p.CreationTime, map => map.NotNullable(true));

            Property(p => p.Data, map => map.NotNullable(true));
        }
    }
}