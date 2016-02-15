using System;
using System.Linq;
using Exceptionless;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Exceptionless {
    /// <summary>
    /// Exceptionless Sink
    /// </summary>
    public class ExceptionlessSink : ILogEventSink {
        private readonly Func<EventBuilder, EventBuilder> _additionalOperation;
        private readonly bool _includeProperties;

        private readonly ExceptionlessClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionlessSink"/> class.
        /// </summary>
        /// <param name="apiKey">
        /// Optional API key that will be used when sending events to the server.
        /// </param>
        /// <param name="serverUrl">
        /// Optional URL of the server events will be sent to.
        /// </param>
        /// <param name="additionalOperation">
        /// Optional operation to run against the Error Builder before submitting to Exceptionless
        /// </param>
        /// <param name="includeProperties">
        /// If false then the Serilog properties will not be submitted to Exceptionless
        /// </param>
        public ExceptionlessSink(
            string apiKey = null,
            string serverUrl = null,
            Func<EventBuilder, EventBuilder> additionalOperation = null,
            bool includeProperties = true
        ) : this (
            new ExceptionlessClient(cfg => {
                if (!String.IsNullOrEmpty(apiKey)) {
                    cfg.ApiKey = apiKey;
                }
                if (!String.IsNullOrEmpty(apiKey)) {
                    cfg.ServerUrl = serverUrl;
                }
            }),
            additionalOperation,
            includeProperties
        ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionlessSink"/> class.
        /// </summary>
        /// <param name="client">
        /// The instance of <see cref="ExceptionlessClient"/> to use.
        /// </param>
        /// <param name="serverUrl">
        /// Optional URL of the server events will be sent to.
        /// </param>
        /// <param name="additionalOperation">
        /// Optional operation to run against the Error Builder before submitting to Exceptionless
        /// </param>
        /// <param name="includeProperties">
        /// If false then the Serilog properties will not be submitted to Exceptionless
        /// </param>
        public ExceptionlessSink(ExceptionlessClient client, Func<EventBuilder, EventBuilder> additionalOperation = null, bool includeProperties = true) {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            _client = client;
            _additionalOperation = additionalOperation;
            _includeProperties = includeProperties;
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