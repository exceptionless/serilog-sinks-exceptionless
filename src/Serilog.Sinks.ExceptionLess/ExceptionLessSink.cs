using System;
using System.Linq;
using Exceptionless;
using Serilog.Core;
using Serilog.Events;

namespace Serilog {
    /// <summary>
    /// ExceptionLess Sink
    /// </summary>
    public class ExceptionLessSink : ILogEventSink {
        private readonly Func<EventBuilder, EventBuilder> _additionalOperation;
        private readonly bool _includeProperties;

        private ExceptionlessClient _client = ExceptionlessClient.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionLessSink"/> class.
        /// </summary>
        /// <param name="additionalOperation">
        /// Optional operation to run against the Error Builder before submitting to ExceptionLess
        /// </param>
        /// <param name="includeProperties">
        /// If false then the seri log properties will not be submitted to ExceptionLess
        /// </param>
        public ExceptionLessSink(Func<EventBuilder, EventBuilder> additionalOperation = null, bool includeProperties = true) {
            _additionalOperation = additionalOperation;
            _includeProperties = includeProperties;
        
            // TODO: Read this from configuration.
            //if (String.IsNullOrEmpty(ApiKey) && String.IsNullOrEmpty(ServerUrl))
            //    return;

            //_client = new ExceptionlessClient(config => {
            //    if (!String.IsNullOrEmpty(ApiKey))
            //        config.ApiKey = ApiKey;
            //    if (!String.IsNullOrEmpty(ServerUrl))
            //        config.ServerUrl = ServerUrl;
            //    config.UseInMemoryStorage();
            //});
        }

        public void Emit(LogEvent logEvent) {
            if (logEvent == null || !_client.Configuration.IsValid)
                return;

            var builder = _client.CreateFromLogEvent(logEvent);

            if (_includeProperties && logEvent.Properties != null) {
                foreach (var property in logEvent.Properties)
                    builder.SetProperty(property.Key, FlattenProperties(property.Value));
            }

            if (_additionalOperation != null)
                _additionalOperation(builder);

            builder.Submit();
        }

        /// <summary>
        /// Removes the structure of <see cref="LogEventPropertyValue"/> implementations introduced
        /// by Serilog and brings properties closer to the structure of the original object.
        /// This enables Exceptionless to display the properties in a nicer way.
        /// </summary>
        private static object FlattenProperties(LogEventPropertyValue value) {
            var scalar = value as ScalarValue;
            if (scalar != null)
                return scalar.Value;

            var sequence = value as SequenceValue;
            if (sequence != null)
                return sequence.Elements.Select(FlattenProperties);

            var structure = value as StructureValue;
            if (structure != null)
                return structure.Properties.ToDictionary(p => p.Name, p => FlattenProperties(p.Value));

            var dictionary = value as DictionaryValue;
            if (dictionary != null)
                return dictionary.Elements.ToDictionary(p => p.Key.Value, p => FlattenProperties(p.Value));

            return value;
        }
    }
}