namespace IdentityServer4.NHibernate.Mappings.Storage
{
    using IdentityServer4.NHibernate.Entities;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    internal class ClientMap : ClassMapping<Client>
    {
        public ClientMap()
        {
            Schema("dbo");
            Table("Clients");

            Id(p => p.ID, map => map.Generator(Generators.Native));

            Property(p => p.ClientId, map => 
            {
                map.NotNullable(true);
                map.Length(200);
                map.UniqueKey("UK_ClientId");
            });

            Property(p => p.ProtocolType, map => 
            {
                map.Length(200);
                map.NotNullable(true);
            });

            Property(p => p.ClientName, map => map.Length(200));
            Property(p => p.ClientUri, map => map.Length(200));         
            Property(p => p.LogoUri, map =>map.Length(2000));
            Property(p => p.Description, map=> map.Length(1000));           
            Property(p => p.FrontChannelLogoutUri, map => map.Length(2000));
            Property(p => p.BackChannelLogoutUri, map => map.Length(2000));
            Property(p => p.ClientClaimsPrefix, map => map.Length(200));
            Property(p => p.PairWiseSubjectSalt, map => map.Length(200));

            Set(p => p.AllowedGrantTypes, map => 
            {
                map.Schema("dbo");
                map.Table("AllowedGrantTypes");
                map.Access(Accessor.Field | Accessor.NoSetter | Accessor.ReadOnly);
                map.Fetch(CollectionFetchMode.Join);
                map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                map.Lazy(CollectionLazy.NoLazy);
            }, r => 
            {
                r.OneToMany(m => m.Class(typeof(Client)));
            });


        }
    }
}
