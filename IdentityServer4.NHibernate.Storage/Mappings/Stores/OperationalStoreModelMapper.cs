using System;
using IdentityServer4.NHibernate.Options;
using NHibernate.Mapping.ByCode;

namespace IdentityServer4.NHibernate.Mappings.Stores
{
    internal class OperationalStoreModelMapper : ModelMapperBase
    {
        private readonly OperationalStoreOptions _options;

        public OperationalStoreModelMapper(OperationalStoreOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            BeforeMapClass += BeforeMapOperationalStoreClass;
        }

        private void BeforeMapOperationalStoreClass(IModelInspector modelInspector, Type type, IClassAttributesMapper classCustomizer)
        {
            var tableDef = GetTableDefinition(type.Name, _options);

            if (tableDef != null)
            {
                classCustomizer.Table(tableDef.Name);
                if (string.IsNullOrEmpty(tableDef.Schema))
                {
                    classCustomizer.Schema(_options.DefaultSchema);
                }
                else
                {
                    classCustomizer.Schema(tableDef.Schema);
                }
            }

            classCustomizer.DynamicUpdate(true);
        }
    }
}
