using System;
using Serilog.Configuration;

namespace Serilog.Sinks.ExceptionLess
{
    public static class LoggerConfigurationExceptionLessExtensions
    {
        public static LoggerConfiguration ExceptionLess(this LoggerSinkConfiguration loggerConfiguration, bool includeProperties = true)
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException("loggerConfiguration");

            return loggerConfiguration.Sink(new ExceptionLessSink(includeProperties));
        }
    }
}