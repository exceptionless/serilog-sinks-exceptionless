using System;
using Exceptionless.Logging;
using Serilog.Debugging;

namespace Serilog.Sinks.Exceptionless {
    public class SelfLogLogger : IExceptionlessLog {
        public void Error(string message, string source = null, Exception exception = null) {
            SelfLog.WriteLine("Error: {0}, source: {1}, Exception: {2}", message, source, exception);
        }

        public void Info(string message, string source = null) {
            SelfLog.WriteLine("Info: {0}, source: {1}", message, source);
        }

        public void Debug(string message, string source = null) {
            SelfLog.WriteLine("Debug: {0}, source: {1}", message, source);
        }

        public void Warn(string message, string source = null) {
            SelfLog.WriteLine("Warn: {0}, source: {1}", message, source);
        }

        public void Trace(string message, string source = null) {
            SelfLog.WriteLine("Trace: {0}, source: {1}", message, source);
        }

        public void Flush() {
        }

        public LogLevel MinimumLogLevel { get; set; }
    }
}