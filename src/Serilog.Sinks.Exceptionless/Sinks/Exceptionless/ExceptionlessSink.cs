using System;
using Exceptionless;
using Exceptionless.Dependency;
using Exceptionless.Logging;
using Exceptionless.Models;
using Exceptionless.Models.Data;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Exceptionless {
    /// <summary>
    /// Exceptionless Sink
    /// </summary>
    public class ExceptionlessSink : ILogEventSink, IDisposable {
        private readonly Func<EventBuilder, EventBuilder> _additionalOperation;
        private readonly bool _includeProperties;

        private readonly ExceptionlessClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionlessSink"/> class.
        /// </summary>
        /// <param name="apiKey">
        /// The API key that will be used when sending events to the server.
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
            string apiKey,
            string serverUrl = null,
            Func<EventBuilder, EventBuilder> additionalOperation = null,
            bool includeProperties = true
        ) {
            if (apiKey == null)
                throw new ArgumentNullException(nameof(apiKey));

            _client = new ExceptionlessClient(config => {
                if (!String.IsNullOrEmpty(apiKey) && apiKey != "API_KEY_HERE")
                    config.ApiKey = apiKey;

                if (!String.IsNullOrEmpty(serverUrl))
                    config.ServerUrl = serverUrl;

                config.UseInMemoryStorage();
                config.UseLogger(new SelfLogLogger());
            });

            _additionalOperation = additionalOperation;
            _includeProperties = includeProperties;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionlessSink"/> class.
        /// </summary>
        /// <param name="additionalOperation">
        /// Optional operation to run against the Error Builder before submitting to Exceptionless
        /// </param>
        /// <param name="includeProperties">
        /// If false then the Serilog properties will not be submitted to Exceptionless
        /// </param>
        /// <param name="client">
        /// Optional instance of <see cref="ExceptionlessClient"/> to use.
        /// </param>
        public ExceptionlessSink(Func<EventBuilder, EventBuilder> additionalOperation = null, bool includeProperties = true, ExceptionlessClient client = null) {
            _additionalOperation = additionalOperation;
            _includeProperties = includeProperties;
            _client = client ?? ExceptionlessClient.Default;

            if (_client.Configuration.Resolver.HasDefaultRegistration<IExceptionlessLog, NullExceptionlessLog>()) {
                _client.Configuration.UseLogger(new SelfLogLogger());
            }
        }

        public void Emit(LogEvent logEvent) {
            if (logEvent == null || !_client.Configuration.IsValid)
                return;

            var minLogLevel = _client.Configuration.Settings.GetMinLogLevel(logEvent.GetSource());
            if (logEvent.GetLevel() < minLogLevel)
                return;

            var builder = _client.CreateFromLogEvent(logEvent);

            if (_includeProperties && logEvent.Properties != null) {
                foreach (var property in logEvent.Properties)
                {
                    if (property.Key == Constants.SourceContextPropertyName)
                    {
                        continue;
                    }
                    else if (property.Key == Event.KnownDataKeys.UserInfo)
                    {
                        if (property.Value is StructureValue structure && structure.TypeTag == nameof(UserInfo))
                        {
                            var userInfo = new UserInfo();
                            var identity = structure.Properties.SequenceToObjectValue<string>(nameof(userInfo.Identity), null);
                            var name = structure.Properties.SequenceToObjectValue<string>(nameof(userInfo.Name), null);
                            if (!string.IsNullOrWhiteSpace(identity) || !string.IsNullOrWhiteSpace(name))
                            {
                                builder.SetUserIdentity(identity, name);
                                continue;
                            }
                        }
                    }
                    else if (property.Key == Event.KnownDataKeys.UserDescription)
                    {
                        if (property.Value is StructureValue structure && structure.TypeTag == nameof(UserDescription))
                        {
                            var userDescription = new UserDescription();
                            var emailAddress = structure.Properties.SequenceToObjectValue<string>(nameof(userDescription.EmailAddress), null);
                            var description = structure.Properties.SequenceToObjectValue<string>(nameof(userDescription.Description), null);

                            if (!string.IsNullOrWhiteSpace(emailAddress) || !string.IsNullOrWhiteSpace(description))
                            {
                                builder.SetUserDescription(emailAddress, description);
                                continue;
                            }
                          
                        }
                    }

                    builder.SetProperty(property.Key, property.Value.FlattenProperties());
                }
            }

            _additionalOperation?.Invoke(builder);
            builder.Submit();
        }

        void IDisposable.Dispose() {
            _client?.ProcessQueue();
        }
    }
}