using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;

namespace DemoService.EventSourceReader
{
    /// <inheritdoc />
    /// <summary>
    /// Implementation of <see cref="T:System.Diagnostics.Tracing.EventListener" /> that writes evententries to the debug console
    /// </summary>
    internal class DemoEventSourceReader : EventListener
    {
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            Debug.WriteLine($"Payload for event {eventData.EventName} (id {eventData.EventId}, version {eventData.Version}) : {string.Join(", ", eventData.FlattenPayload().Select(pl => $"{pl.Key}: {pl.Value}"))}");
        }
    }
}