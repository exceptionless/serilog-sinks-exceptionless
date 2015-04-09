using System;
using Exceptionless;
using Serilog.Configuration;

namespace Serilog.Sinks
{
    /// <summary>
    /// The logger configuration exception less extensions.
    /// </summary>
    public static class LoggerConfigurationExceptionLessExtensions
    {
        /// <summary>
        /// Creates a new ExceptionLess sink
        /// </summary>
        /// <param name="loggerConfiguration">
        /// The logger configuration.
        /// </param>
        /// <param name="additionalOperation">
        /// Any additional operation to run against the build exceptions
        /// </param>
        /// <param name="includeProperties">
        /// If false it suppressed sending the Serilog properties to ExceptionLess
        /// </param>
        /// <returns>
        /// The <see cref="LoggerConfiguration"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static LoggerConfiguration ExceptionLess(this LoggerSinkConfiguration loggerConfiguration, Func<ErrorBuilder, ErrorBuilder> additionalOperation = null, bool includeProperties = true)
        {
            if (loggerConfiguration == null)
            {
                throw new ArgumentNullException("loggerConfiguration");
            }

            return loggerConfiguration.Sink(new ExceptionLessSink(additionalOperation, includeProperties));
        }
    }
}