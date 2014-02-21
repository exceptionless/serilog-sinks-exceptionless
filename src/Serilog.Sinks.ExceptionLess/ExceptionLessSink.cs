using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exceptionless;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.ExceptionLess
{
    public class ExceptionLessSink : ILogEventSink
    {
        private readonly bool _includeProperties;

        public ExceptionLessSink(bool includeProperties = true)
        {
            _includeProperties = includeProperties;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null)
                throw new ArgumentNullException("logEvent");

            if (logEvent.Exception == null)
                return;

            ErrorBuilder errorBuilder = logEvent.Exception.ToExceptionless();

            if (logEvent.Level == LogEventLevel.Fatal)
                errorBuilder.MarkAsCritical();

            if (_includeProperties && logEvent.Properties != null && logEvent.Properties.Count != 0)
            {
                errorBuilder.AddObject(logEvent.Properties);
            }

            errorBuilder.Submit();
        }
    }
}
