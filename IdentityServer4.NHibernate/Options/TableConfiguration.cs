namespace IdentityServer4.NHibernate.Options
{
    /// <summary>
    /// Class to control a table's name and schema.
    /// </summary>
    public class TableConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableConfiguration"/> class.
        /// </summary>
        /// <param name="name">The table name.</param>
        public TableConfiguration(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableConfiguration"/> class.
        /// </summary>
        /// <param name="name">The table name.</param>
        /// <param name="schema">The table's schema name.</param>
        public TableConfiguration(string name, string schema)
        {
            Name = name;
            Schema = schema;
        }

        /// <summary>
        /// Gets or sets the table's name.
        /// </summary>
        /// <value>
        /// The table name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the table's schema name.
        /// </summary>
        /// <value>
        /// The schema name.
        /// </value>
        public string Schema { get; set; }
    }
}