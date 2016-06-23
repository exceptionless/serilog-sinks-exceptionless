using System;
using Exceptionless;
using Exceptionless.Logging;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Exceptionless {
    public static class ExceptionlessClientExtensions {
        public static EventBuilder CreateFromLogEvent(this ExceptionlessClient client, LogEvent log) {
            var builder = log.Exception != null 
                ? client.CreateException(log.Exception)
                : client.CreateLog(log.GetSource(), log.RenderMessage(), log.GetLevel());

            builder.Target.Date = log.Timestamp;
            if (log.Level == LogEventLevel.Fatal)
                builder.MarkAsCritical();

            if (!String.IsNullOrWhiteSpace(log.RenderMessage()))
                builder.SetMessage(log.RenderMessage());

            return builder;
        }

        public static void SubmitFromLogEvent(this ExceptionlessClient client, LogEvent log) {
            CreateFromLogEvent(client, log).Submit();
        }

        internal static string GetSource(this LogEvent log) {
            LogEventPropertyValue value;
            if (log.Properties.TryGetValue(Constants.SourceContextPropertyName, out value) && value != null)
                return value.ToString();

            return null;
        }

        internal static LogLevel GetLevel(this LogEvent log) {
            switch (log.Level) {
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