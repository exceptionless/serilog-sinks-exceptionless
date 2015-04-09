using System;
using System.Linq;
using Exceptionless;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks
{
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
        public ExceptionLessSink(Func<ErrorBuilder, ErrorBuilder> additionalOperation = null, bool includeProperties = true)
        {
            _additionalOperation = additionalOperation;
            _includeProperties = includeProperties;
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

            if (_includeProperties && logEvent.Properties != null && logEvent.Properties.Count != 0)
            {
                foreach (var property in logEvent.Properties)
                {
                    errorBuilder.AddObject(FlattenProperties(property.Value), property.Key);
                }
            }

            if (_additionalOperation != null)
            {
                _additionalOperation(errorBuilder);
            }

            errorBuilder.Submit();
        }

        /// <summary>
        /// Removes the structure of <see cref="LogEventPropertyValue"/> implementations introduced
        /// by Serilog and brings properties closer to the structure of the original object.
        /// This enables Exceptionless to display the properties in a nicer way.
        /// </summary>
        private static object FlattenProperties(LogEventPropertyValue value)
        {
            var scalar = value as ScalarValue;
            if (scalar != null)
            {
                return scalar.Value;
            }

            var sequence = value as SequenceValue;
            if (sequence != null)
            {
                return sequence.Elements
                    .Select(FlattenProperties);
            }

            var structure = value as StructureValue;
            if (structure != null)
            {
                return structure.Properties
                    .ToDictionary(p => p.Name, p => FlattenProperties(p.Value));
            }

            var dictionary = value as DictionaryValue;
            if (dictionary != null)
            {
                return dictionary.Elements
                    .ToDictionary(p => p.Key.Value, p => FlattenProperties(p.Value));
            }

            return value;
        }
    }
}