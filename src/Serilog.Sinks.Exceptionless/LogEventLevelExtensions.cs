using Exceptionless.Logging;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.Exceptionless
{
    internal static class LogEventLevelExtensions
    {
        public static LogLevel ToLogLevel(this LogEventLevel logEventLevel)
        {
            switch (logEventLevel)
            {
                case LogEventLevel.Verbose:
                    return LogLevel.Trace;
                case LogEventLevel.Debug:
                    return LogLevel.Debug;
                case LogEventLevel.Information:
                    return LogLevel.Info;
                case LogEventLevel.Warning:
                    return LogLevel.Warn;
                case LogEventLevel.Error:
                    return LogLevel.Error;
                case LogEventLevel.Fatal:
                    return LogLevel.Fatal;
                default:
                    return LogLevel.Other;
            }
        }
    }
}
