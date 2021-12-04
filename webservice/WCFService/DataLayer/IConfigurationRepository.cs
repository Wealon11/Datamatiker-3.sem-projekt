namespace WCFService.DataLayer
{
    public interface IConfigurationRepository
    {
        /// <summary>
        /// Looks up a configuration value by key in the data layer.
        /// </summary>
        /// <exception cref="System.ArgumentNullException" />
        /// <returns>Configuration value as string or null if not found.</returns>
        string Get(string key);

        /// <summary>
        /// Sets the configuration value for the given key in the data layer.
        /// </summary>
        /// <exception cref="System.ArgumentException" />
        void Set(string key, string value);
    }
}