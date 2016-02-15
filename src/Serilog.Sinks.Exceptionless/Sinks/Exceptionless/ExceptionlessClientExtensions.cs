using System;
using Exceptionless;
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

        private static string GetSource(this LogEvent log) {
            LogEventPropertyValue value;
            if (log.Properties.TryGetValue(Constants.SourceContextPropertyName, out value) && value != null)
                return value.ToString();

            return null;
        }

        private static string GetLevel(this LogEvent log) {
            switch (log.Level) {
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