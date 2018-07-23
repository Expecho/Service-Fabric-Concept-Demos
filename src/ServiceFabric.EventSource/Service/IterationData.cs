using System;
using System.Diagnostics.Tracing;

namespace DemoService
{
    /// <summary>
    /// Data passed to the <see cref="ServiceEventSource"/> as evententry payload
    /// </summary>
    /// <remarks>Using the <see cref="EventDataAttribute"/> we can mark a class so it
    /// can be used as an argument to the WriteEvent method of an <see cref="EventSource"/>.
    /// Withouth it ony simple types can be used as arguments</remarks>
    [EventData]
    public class IterationData
    {
        public long Iteration { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.Now;

        /// <summary>
        /// By using the <see cref="EventIgnoreAttribute"/> properties can be excluded from logging
        /// </summary>
        [EventIgnore]
        public string Ignoredproperty { get; set; }
    }
}