using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace DemoService.EventSourceReader
{
    /// <summary>
    /// Extensions methods for <see cref="EventWrittenEventArgs"/>
    /// </summary>
    public static class EventWrittenEventArgsExtensions
    {
        /// <summary>
        /// Flattens the payload into one dimensional dictionary.
        /// </summary>
        /// <param name="eventData">The <see cref="EventWrittenEventArgs"/> to get the flattened payload for</param>
        /// <returns>One dimensional dictionary with all payload properties</returns>
        public static IDictionary<string, object> FlattenPayload(this EventWrittenEventArgs eventData)
        {
            IDictionary<string, object> flattenedPayload = new Dictionary<string, object>();

            for (var i = 0; i < eventData.Payload.Count; i++)
            {
                if (!(eventData.Payload[i] is IDictionary<string, object> nestedPayload))
                    flattenedPayload.Add(eventData.PayloadNames[i], eventData.Payload[i]);
                else
                    foreach (var item in ExtractNestedPayload(eventData.PayloadNames[i], nestedPayload))
                    {
                        flattenedPayload.Add(item.Key, item.Value);
                    }
            }

            return flattenedPayload;
        }

        private static IDictionary<string, object> ExtractNestedPayload(string name, IDictionary<string, object> payload)
        {
            IDictionary<string, object> flattenedPayload = new Dictionary<string, object>();

            foreach (var item in payload)
            {
                if (!(item.Value is IDictionary<string, object> nestedPayload))
                    flattenedPayload.Add($"{name}.{item.Key}", item.Value);
                else
                    foreach (var nestedItem in ExtractNestedPayload($"{name}.{item.Key}", nestedPayload))
                    {
                        flattenedPayload.Add(nestedItem.Key, nestedItem.Value);
                    }
            }

            return flattenedPayload;
        }
    }
}