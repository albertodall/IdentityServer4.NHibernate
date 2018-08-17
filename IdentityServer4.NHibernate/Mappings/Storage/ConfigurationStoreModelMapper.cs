namespace IdentityServer4.NHibernate.Mappings.Storage
{
    using System;
    using Options;
    using global::NHibernate.Mapping.ByCode;

    internal class ConfigurationStoreModelMapper : ModelMapper
    {
        private readonly ConfigurationStoreOptions _options;

        public ConfigurationStoreModelMapper(ConfigurationStoreOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            BeforeMapClass += ConfigurationStoreModelMapper_BeforeMapClass;
        }

        private void ConfigurationStoreModelMapper_BeforeMapClass(IModelInspector modelInspector, Type type, IClassAttributesMapper classCustomizer)
        {   
            var tableDef = GetTableDefinition(type.Name);
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
        }

        private TableDefinition GetTableDefinition(string tableObjectName)
        {
            var prop = typeof(ConfigurationStoreOptions).GetProperty(tableObjectName);
            if (prop != null)
            {
                return prop.GetValue(_options, null) as TableDefinition;
            }
            return null;
        }
    }
}
