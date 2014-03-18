namespace Serilog.Sinks.ExceptionLess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core;
    using Events;
    using Exceptionless;

    /// <summary>
    /// ExceptionLess Sink
    /// </summary>
    public class ExceptionLessSink : ILogEventSink
    {
        /// <summary>
        /// Additional operation to execute against the <c>ErrorBuilder</c> object prior to submitting to ExceptionLess
        /// </summary>
        private readonly Func<ErrorBuilder, ErrorBuilder> _additionalOperation;

        /// <summary>
        /// If false then the seri log properties will not be submitted to ExceptionLess
        /// </summary>
        private readonly bool _includeProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionLessSink"/> class.
        /// </summary>
        /// <param name="additionalOperation">
        /// Optional operation to run against the Error Builder before submitting to ExceptionLess
        /// </param>
        /// <param name="includeProperties">
        /// If false then the seri log properties will not be submitted to ExceptionLess
        /// </param>
        public ExceptionLessSink(Func<ErrorBuilder, ErrorBuilder> additionalOperation = null, bool includeProperties= true)
        {
            this._additionalOperation = additionalOperation;
            this._includeProperties = includeProperties;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException("logEvent");
            }

            if (logEvent.Exception == null)
            {
                return;
            }

            ErrorBuilder errorBuilder = logEvent.Exception.ToExceptionless();
            if (logEvent.Level == LogEventLevel.Fatal)
            {
                errorBuilder.MarkAsCritical();
            }

            errorBuilder.AddObject(logEvent.RenderMessage(), "Log Message");

            if (this._includeProperties && logEvent.Properties != null && logEvent.Properties.Count != 0)
            {
                foreach (var property in logEvent.Properties)
                {
                    errorBuilder.AddObject(property.Value, property.Key);
                }                
            }

            if (this._additionalOperation != null)
            {
                this._additionalOperation(errorBuilder);
            }

            errorBuilder.Submit();
        }
    }
}
