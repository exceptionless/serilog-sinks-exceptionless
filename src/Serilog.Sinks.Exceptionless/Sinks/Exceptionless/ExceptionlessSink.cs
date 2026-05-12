using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exceptionless;
using Exceptionless.Dependency;
using Exceptionless.Logging;
using Exceptionless.Models;
using Exceptionless.Models.Data;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Exceptionless
{
    /// <summary>
    /// Exceptionless Sink
    /// </summary>
    public class ExceptionlessSink : IBatchedLogEventSink, IDisposable
#if NET8_0_OR_GREATER
        , IAsyncDisposable
#endif
    {
        private readonly string[] _defaultTags;
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
        /// <param name="defaultTags">
        /// Default tags to be added to every log event.
        /// </param>
        /// <param name="additionalOperation">
        /// Optional operation to run against the Error Builder before submitting to Exceptionless
        /// </param>
        /// <param name="includeProperties">
        /// If false then the Serilog properties will not be submitted to Exceptionless
        /// </param>
        /// <param name="restrictedToMinimumLevel">
        /// The minimum log event level required in order to write an event to the sink.
        /// </param>
        public ExceptionlessSink(
            string apiKey,
            string serverUrl = null,
            string[] defaultTags = null,
            Func<EventBuilder, EventBuilder> additionalOperation = null,
            bool includeProperties = true,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum
        )
        {
            if (apiKey == null)
                throw new ArgumentNullException(nameof(apiKey));

            _client = new ExceptionlessClient(config =>
            {
                if (!String.IsNullOrEmpty(apiKey) && apiKey != "API_KEY_HERE")
                    config.ApiKey = apiKey;

                if (!String.IsNullOrEmpty(serverUrl))
                    config.ServerUrl = serverUrl;

                config.UseInMemoryStorage();
                config.UseLogger(new SelfLogLogger());
                config.SetDefaultMinLogLevel(restrictedToMinimumLevel.GetLevel());
            });

            _defaultTags = defaultTags;
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
        /// <param name="restrictedToMinimumLevel">
        /// The minimum log event level required in order to write an event to the sink.
        /// </param>
        public ExceptionlessSink(
            Func<EventBuilder, EventBuilder> additionalOperation = null,
            bool includeProperties = true,
            ExceptionlessClient client = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum
        )
        {
            _additionalOperation = additionalOperation;
            _includeProperties = includeProperties;
            _client = client ?? ExceptionlessClient.Default;

            if (_client.Configuration.Resolver.HasDefaultRegistration<IExceptionlessLog, NullExceptionlessLog>())
            {
                _client.Configuration.UseLogger(new SelfLogLogger());
            }

            _client.Configuration.SetDefaultMinLogLevel(restrictedToMinimumLevel.GetLevel());
        }

        public Task EmitBatchAsync(IReadOnlyCollection<LogEvent> batch)
        {
            if (batch == null || batch.Count == 0 || !_client.Configuration.IsValid)
                return Task.CompletedTask;

            foreach (var logEvent in batch)
            {
                var minLogLevel = _client.Configuration.Settings.GetMinLogLevel(logEvent.GetSource());
                if (logEvent.Level.GetLevel() < minLogLevel)
                    continue;

                var builder = _client.CreateFromLogEvent(logEvent).AddTags(_defaultTags);

                if (_includeProperties)
                {
                    foreach (var prop in logEvent.Properties)
                    {
                        switch (prop.Key)
                        {
                            case Constants.SourceContextPropertyName:
                                continue;
                            case Event.KnownDataKeys.UserInfo when prop.Value is StructureValue uis && String.Equals(nameof(UserInfo), uis.TypeTag):
                                var userInfo = uis.FlattenProperties() as Dictionary<string, object>;
                                if (userInfo is null)
                                    continue;

                                // UserDescription Data property is currently ignored.
                                userInfo.TryGetValue(nameof(UserInfo.Identity), out object identityValue);
                                userInfo.TryGetValue(nameof(UserInfo.Name), out object nameValue);
                                string identity = identityValue as string;
                                string name = nameValue as string;
                                if (!String.IsNullOrWhiteSpace(identity) || !String.IsNullOrWhiteSpace(name))
                                    builder.SetUserIdentity(identity, name);
                                break;
                            case Event.KnownDataKeys.UserDescription when prop.Value is StructureValue uds && String.Equals(nameof(UserDescription), uds.TypeTag):
                                var userDescription = uds.FlattenProperties() as Dictionary<string, object>;
                                if (userDescription is null)
                                    continue;

                                // UserDescription Data property is currently ignored.
                                userDescription.TryGetValue(nameof(UserDescription.EmailAddress), out object emailAddressValue);
                                userDescription.TryGetValue(nameof(UserDescription.Description), out object descriptionValue);
                                string emailAddress = emailAddressValue as string;
                                string description = descriptionValue as string;
                                if (!String.IsNullOrWhiteSpace(emailAddress) || !String.IsNullOrWhiteSpace(description))
                                    builder.SetUserDescription(emailAddress, description);
                                break;
                            case "Tags":
                                builder.AddTags(prop.Value.GetTags());
                                break;
                            default:
                                builder.SetProperty(prop.Key, prop.Value.FlattenProperties());
                                break;
                        }
                    }
                }

                _additionalOperation?.Invoke(builder);
                builder.Submit();
            }

            return Task.CompletedTask;
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }

        void IDisposable.Dispose()
        {
            _client?.ProcessQueueAsync().GetAwaiter().GetResult();
        }

#if NET8_0_OR_GREATER
        public async ValueTask DisposeAsync()
        {
            if (_client != null)
                await _client.ProcessQueueAsync().ConfigureAwait(false);
        }
#endif
    }
}
