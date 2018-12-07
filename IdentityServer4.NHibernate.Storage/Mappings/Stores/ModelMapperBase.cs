using IdentityServer4.NHibernate.Options;
using NHibernate.Mapping.ByCode;

namespace IdentityServer4.NHibernate.Mappings.Stores
{
    internal abstract class ModelMapperBase : ModelMapper
    {
        protected TableDefinition GetTableDefinition<TOptions>(string tableObjectName, TOptions options)
            where TOptions : StoreOptionsBase
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
