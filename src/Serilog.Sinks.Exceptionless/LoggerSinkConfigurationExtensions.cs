using System;
using System.ComponentModel;
using Exceptionless;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.Exceptionless;

namespace Serilog
{
    /// <summary>
    /// The logger configuration exception less extensions.
    /// </summary>
    public static class LoggerSinkConfigurationExtensions
    {
        /// <summary>Creates a new Exceptionless sink with the specified <paramref name="apiKey"/>.</summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="apiKey">The API key that will be used when sending events to the server.</param>
        /// <param name="additionalOperation">Any additional operation to run against the build exceptions</param>
        /// <param name="includeProperties">If false it suppressed sending the Serilog properties to Exceptionless</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <returns>The <see cref="LoggerConfiguration"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static LoggerConfiguration Exceptionless(
            this LoggerSinkConfiguration loggerConfiguration,
            string apiKey,
            Func<EventBuilder, EventBuilder> additionalOperation = null,
            bool includeProperties = true,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum
        )
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));
            if (apiKey == null)
                throw new ArgumentNullException(nameof(apiKey));

            return loggerConfiguration.Sink(new ExceptionlessSink(apiKey, null, null, additionalOperation, includeProperties, restrictedToMinimumLevel, null), new BatchingOptions { BatchSizeLimit = 50 }, restrictedToMinimumLevel);
        }

        /// <summary>Creates a new Exceptionless sink with the specified <paramref name="apiKey"/>.</summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="apiKey">The API key that will be used when sending events to the server.</param>
        /// <param name="serverUrl">Optional URL of the server events will be sent to.</param>
        /// <param name="defaultTags">Default tags to be added to every log event.</param>
        /// <param name="additionalOperation">Any additional operation to run against the build exceptions</param>
        /// <param name="includeProperties">If false it suppressed sending the Serilog properties to Exceptionless</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="environment">
        /// Optional deployment environment for every event, such as production or staging. When not set, the client
        /// detects ASPNETCORE_ENVIRONMENT, DOTNET_ENVIRONMENT or AZURE_FUNCTIONS_ENVIRONMENT.
        /// </param>
        /// <returns>The <see cref="LoggerConfiguration"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static LoggerConfiguration Exceptionless(
            this LoggerSinkConfiguration loggerConfiguration,
            string apiKey,
            string serverUrl = null,
            string[] defaultTags = null,
            Func<EventBuilder, EventBuilder> additionalOperation = null,
            bool includeProperties = true,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string environment = null
        )
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));
            if (apiKey == null)
                throw new ArgumentNullException(nameof(apiKey));

            return loggerConfiguration.Sink(new ExceptionlessSink(apiKey, serverUrl, defaultTags, additionalOperation, includeProperties, restrictedToMinimumLevel, environment), new BatchingOptions { BatchSizeLimit = 50 }, restrictedToMinimumLevel);
        }

        /// <summary>Kept for binary compatibility; use the overload that accepts an environment.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static LoggerConfiguration Exceptionless(
            this LoggerSinkConfiguration loggerConfiguration,
            string apiKey,
            string serverUrl,
            string[] defaultTags,
            Func<EventBuilder, EventBuilder> additionalOperation,
            bool includeProperties,
            LogEventLevel restrictedToMinimumLevel
        )
        {
            return Exceptionless(loggerConfiguration, apiKey, serverUrl, defaultTags, additionalOperation, includeProperties, restrictedToMinimumLevel, null);
        }

        /// <summary>Creates a new Exceptionless sink.</summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="additionalOperation">Any additional operation to run against the build exceptions</param>
        /// <param name="includeProperties">If false it suppressed sending the Serilog properties to Exceptionless</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="client">Optional instance of <see cref="ExceptionlessClient"/> to use.</param>
        /// <returns>The <see cref="LoggerConfiguration"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static LoggerConfiguration Exceptionless(
            this LoggerSinkConfiguration loggerConfiguration,
            Func<EventBuilder, EventBuilder> additionalOperation = null,
            bool includeProperties = true,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            ExceptionlessClient client = null
        )
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));

            return loggerConfiguration.Sink(new ExceptionlessSink(additionalOperation, includeProperties, client, restrictedToMinimumLevel), new BatchingOptions { BatchSizeLimit = 50 }, restrictedToMinimumLevel);
        }
    }
}