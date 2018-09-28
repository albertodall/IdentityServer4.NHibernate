using System;
using IdentityServer4.NHibernate.Options;
using NHibernate.Mapping.ByCode;

namespace IdentityServer4.NHibernate.Mappings.Storage
{
    internal class OperationalStoreModelMapper : ModelMapper
    {
        private readonly OperationalStoreOptions _options;

        public OperationalStoreModelMapper(OperationalStoreOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            BeforeMapClass += BeforeMapOperationalStoreClass;
        }

        private void BeforeMapOperationalStoreClass(IModelInspector modelInspector, Type type, IClassAttributesMapper classCustomizer)
        {
            classCustomizer.Table(_options.DefaultSchema);
            classCustomizer.DynamicUpdate(true);
        }
    }
}
