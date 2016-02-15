using System; 
using Exceptionless;
using Serilog.Configuration;
using Serilog.Sinks.Exceptionless;

namespace Serilog {
    /// <summary>
    /// The logger configuration exception less extensions.
    /// </summary>
    public static class LoggerSinkConfigurationExtensions {
        /// <summary>
        /// Creates a new Exceptionless sink
        /// </summary>
        /// <param name="loggerConfiguration">
        /// The logger configuration.
        /// </param>
        /// <param name="apiKey">
        /// Optional API key that will be used when sending events to the server.
        /// </param>
        /// <param name="serverUrl">
        /// Optional URL of the server events will be sent to.
        /// </param>
        /// <param name="additionalOperation">
        /// Any additional operation to run against the build exceptions
        /// </param>
        /// <param name="includeProperties">
        /// If false it suppressed sending the Serilog properties to Exceptionless
        /// </param>
        /// <returns>
        /// The <see cref="LoggerConfiguration"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static LoggerConfiguration Exceptionless(
            this LoggerSinkConfiguration loggerConfiguration,
            string apiKey = null,
            string serverUrl = null,
            Func<EventBuilder, EventBuilder> additionalOperation = null,
            bool includeProperties = true
        ) {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));

            return loggerConfiguration.Sink(new ExceptionlessSink(apiKey, serverUrl, additionalOperation, includeProperties));
        }

        /// <summary>
        /// Creates a new Exceptionless sink with the specified <see cref="ExceptionlessClient"/>.
        /// </summary>
        /// <param name="loggerConfiguration">
        /// The logger configuration.
        /// </param>
        /// <param name="client">
        /// The instance of <see cref="ExceptionlessClient"/> to use.
        /// </param>
        /// <param name="additionalOperation">
        /// Any additional operation to run against the build exceptions
        /// </param>
        /// <param name="includeProperties">
        /// If false it suppressed sending the Serilog properties to Exceptionless
        /// </param>
        /// <returns>
        /// The <see cref="LoggerConfiguration"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static LoggerConfiguration Exceptionless(
            this LoggerSinkConfiguration loggerConfiguration,
            ExceptionlessClient client,
            Func<EventBuilder, EventBuilder> additionalOperation = null,
            bool includeProperties = true
        ) {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            return loggerConfiguration.Sink(new ExceptionlessSink(client, additionalOperation, includeProperties));
        }
    }
}