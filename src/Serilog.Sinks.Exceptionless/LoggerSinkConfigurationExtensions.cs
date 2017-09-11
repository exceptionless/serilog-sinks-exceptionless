using System;
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
        /// <param name="serverUrl">Optional URL of the server events will be sent to.</param>
        /// <param name="additionalOperation">Any additional operation to run against the build exceptions</param>
        /// <param name="includeProperties">If false it suppressed sending the Serilog properties to Exceptionless</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <returns>The <see cref="LoggerConfiguration"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static LoggerConfiguration Exceptionless(
            this LoggerSinkConfiguration loggerConfiguration,
            string apiKey,
            string serverUrl = null,
            Func<EventBuilder, EventBuilder> additionalOperation = null,
            bool includeProperties = true,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum
        )
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));
            if (apiKey == null)
                throw new ArgumentNullException(nameof(apiKey));

            return loggerConfiguration.Sink(new ExceptionlessSink(apiKey, serverUrl, additionalOperation, includeProperties), restrictedToMinimumLevel);
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

            return loggerConfiguration.Sink(new ExceptionlessSink(additionalOperation, includeProperties, client), restrictedToMinimumLevel);
        }
    }
}