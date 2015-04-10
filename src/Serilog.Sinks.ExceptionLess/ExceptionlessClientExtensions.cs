using System;
using Exceptionless;
using Serilog.Events;

namespace Serilog {
    public static class ExceptionlessClientExtensions {
        public static EventBuilder CreateFromLogEvent(this ExceptionlessClient client, LogEvent log) {
            var builder = log.Exception != null 
                ? client.CreateException(log.Exception)
                : client.CreateLog(null, log.RenderMessage(), GetLogLevel(log.Level)); // TODO: How can we get the log source?

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

        private static string GetLogLevel(LogEventLevel level) {
            switch (level) {
                case LogEventLevel.Verbose:
                    return "Trace";
                case LogEventLevel.Debug:
                    return "Debug";
                case LogEventLevel.Information:
                    return "Info";
                case LogEventLevel.Warning:
                    return "Warn";
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    return "Error";
            }

            return null;
        }
    }
}