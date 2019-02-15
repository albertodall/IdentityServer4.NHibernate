using IdentityServer4.NHibernate.Options;
using NHibernate.Mapping.ByCode;

namespace IdentityServer4.NHibernate.Extensions
{
    internal static class ClassAttributesMapperExtensions
    {
        public static IClassAttributesMapper MapToTable<TOptions>(this IClassAttributesMapper classCustomizer, TableDefinition tableDefinition, TOptions options)
            where TOptions: StoreOptionsBase
        {
            classCustomizer.Table(tableDefinition.Name);
            if (string.IsNullOrEmpty(tableDefinition.Schema))
            {
                classCustomizer.Schema(options.DefaultSchema);
            }
            else
            {
                classCustomizer.Schema(tableDefinition.Schema);
            }

            return classCustomizer;
        }
    }
}
