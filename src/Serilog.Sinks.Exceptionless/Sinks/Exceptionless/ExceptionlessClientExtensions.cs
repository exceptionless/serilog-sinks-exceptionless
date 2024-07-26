using System;
using System.Collections.Generic;
using System.Linq;
using Exceptionless;
using Exceptionless.Logging;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Exceptionless
{
    public static class ExceptionlessClientExtensions
    {
        public static EventBuilder CreateFromLogEvent(this ExceptionlessClient client, LogEvent log)
        {
            string message = log.RenderMessage();

            var builder = log.Exception != null
                ? client.CreateException(log.Exception)
                : client.CreateLog(log.GetSource(), message, log.Level.GetLevel());

            builder.Target.Date = log.Timestamp;
            if (log.Level == LogEventLevel.Fatal)
                builder.MarkAsCritical();

            if (!String.IsNullOrWhiteSpace(message))
                builder.SetMessage(message);

            return builder;
        }

        public static void SubmitFromLogEvent(this ExceptionlessClient client, LogEvent log)
        {
            CreateFromLogEvent(client, log).Submit();
        }

        internal static string GetSource(this LogEvent log)
        {
            if (log.Properties.TryGetValue(Constants.SourceContextPropertyName, out LogEventPropertyValue value))
                return value.FlattenProperties()?.ToString();

            return null;
        }

        internal static string[] GetTags(this LogEventPropertyValue value)
        {
            var propertyCollection = value.FlattenProperties() as List<object>;
            if (propertyCollection == null)
                return Array.Empty<string>();

            return propertyCollection.Select(p => p.ToString()).ToArray();
        }

        internal static LogLevel GetLevel(this LogEventLevel log)
        {
            switch (log)
            {
                case LogEventLevel.Verbose:
                    return LogLevel.Trace;
                case LogEventLevel.Debug:
                    return LogLevel.Debug;
                case LogEventLevel.Information:
                    return LogLevel.Info;
                case LogEventLevel.Warning:
                    return LogLevel.Warn;
                case LogEventLevel.Error:
                    return LogLevel.Error;
                case LogEventLevel.Fatal:
                    return LogLevel.Fatal;
                default:
                    return LogLevel.Other;
            }
        }

        /// <summary>
        /// Removes the structure of <see cref="LogEventPropertyValue"/> implementations introduced
        /// by Serilog and brings properties closer to the structure of the original object.
        /// This enables Exceptionless to display the properties in a nicer way.
        /// </summary>
        internal static object FlattenProperties(this LogEventPropertyValue value)
        {
            if (value == null)
                return null;

            if (value is ScalarValue scalar)
                return scalar.Value;

            if (value is SequenceValue sequence)
            {
                var flattenedProperties = new List<object>(sequence.Elements.Count);
                foreach (var element in sequence.Elements)
                    flattenedProperties.Add(element.FlattenProperties());

                return flattenedProperties;
            }

            if (value is StructureValue structure)
            {
                var flattenedProperties = new Dictionary<string, object>(structure.Properties.Count);
                foreach (var property in structure.Properties)
                    flattenedProperties.Add(property.Name, property.Value.FlattenProperties());

                return flattenedProperties;
            }

            if (value is DictionaryValue dictionary)
            {
                var flattenedProperties = new Dictionary<object, object>(dictionary.Elements.Count);
                foreach (var element in dictionary.Elements)
                    flattenedProperties.Add(element.Key.Value, element.Value.FlattenProperties());

                return flattenedProperties;
            }

            return value;
        }
    }
}