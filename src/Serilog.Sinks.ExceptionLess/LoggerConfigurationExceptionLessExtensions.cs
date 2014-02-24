using System;
using Exceptionless;
using Serilog.Configuration;

namespace Serilog.Sinks.ExceptionLess
{
    public static class LoggerConfigurationExceptionLessExtensions
    {
        public static LoggerConfiguration ExceptionLess(this LoggerSinkConfiguration loggerConfiguration, Func<ErrorBuilder, ErrorBuilder> additionalOperation = null, bool includeProperties = true)
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException("loggerConfiguration");

            return loggerConfiguration.Sink(new ExceptionLessSink(additionalOperation, includeProperties));
        }
    }
}