namespace IdentityServer4.NHibernate.Options
{
    using System;
    using global::NHibernate.Cfg;

    public class OperationalStoreOptions
    {
        /// <summary>
        /// Callback to configure the SessionFactory.
        /// </summary>
        /// <value>
        /// Session factory for the operational store.
        /// </value>
        public Func<Configuration> Database { get; set; }

        /// <summary>
        /// Gets or sets the default schema for database objects.
        /// </summary>
        /// <value>
        /// The default schema.
        /// </value>
        public string DefaultSchema { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether stale entries will be automatically cleaned up from the database.
        /// This is implemented by perodically connecting to the database (according to the TokenCleanupInterval) from the hosting application.
        /// Defaults to false.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable token cleanup]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableTokenCleanup { get; set; } = false;

        /// <summary>
        /// Gets or sets the token cleanup interval (in seconds). The default is 3600 (1 hour).
        /// </summary>
        /// <value>
        /// The token cleanup interval.
        /// </value>
        public int TokenCleanupInterval { get; set; } = 3600;

        /// <summary>
        /// Gets or sets the number of records to remove at a time. Defaults to 100.
        /// </summary>
        /// <value>
        /// The size of the token cleanup batch.
        /// </value>
        public int TokenCleanupBatchSize { get; set; } = 100;
    }
}
