using IdentityServer4.NHibernate.Storage.Options;
using NHibernate.Mapping.ByCode;

namespace IdentityServer4.NHibernate.Storage.Mappings.Stores
{
    internal abstract class ModelMapperBase : ModelMapper
    {
        protected TableDefinition GetTableDefinition<TOptions>(string tableObjectName, TOptions options)
        {
            var prop = typeof(TOptions).GetProperty(tableObjectName);
            if (prop != null)
            {
                return prop.GetValue(options, null) as TableDefinition;
            }
            return null;
        }
    }
}
