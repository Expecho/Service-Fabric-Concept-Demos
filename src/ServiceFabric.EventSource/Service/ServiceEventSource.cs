using System.Diagnostics.Tracing;

namespace DemoService
{
    [EventSource(Name = "MyCompany-DynamicEventSourceDemo-DemoService")]
    internal sealed class ServiceEventSource : EventSource
    {
        public static readonly ServiceEventSource Current = new ServiceEventSource();
        private readonly EventCounter performanceCounter;

        public static class Keywords
        {
            public const EventKeywords ServiceExecution = (EventKeywords)1;
            public const EventKeywords ServiceManagement = (EventKeywords)2;
        }

        public enum Events
        {
            ServiceTypeRegisteredEventId = 1,
            ServiceHostInitializationFailedEventId = 2,
            IterationUpdated = 3
        }

        // Instance constructor is private to enforce singleton semantics
        private ServiceEventSource() : base(EventSourceSettings.EtwSelfDescribingEventFormat)
        {
            performanceCounter = new EventCounter(nameof(performanceCounter), this);
        }

        [Event((int)Events.ServiceTypeRegisteredEventId, Level = EventLevel.Informational, Message = "Service host process {0} registered service type {1}", Keywords = Keywords.ServiceManagement)]
        public void ServiceTypeRegistered(int hostProcessId, string serviceType)
        {
            WriteEvent((int)Events.ServiceTypeRegisteredEventId, hostProcessId, serviceType);
        }

        [Event((int)Events.ServiceHostInitializationFailedEventId, Level = EventLevel.Error, Message = "Service host initialization failed", Keywords = Keywords.ServiceManagement)]
        public void ServiceHostInitializationFailed(string exception)
        {
            WriteEvent((int)Events.ServiceHostInitializationFailedEventId, exception);
        }

        [Event((int)Events.IterationUpdated, Level = EventLevel.Informational, Keywords = Keywords.ServiceExecution)]
        public void IterationUpdated(LogContext context, IterationData iterationData)
        {
            WriteEvent((int)Events.IterationUpdated, context, iterationData);
        }

        /// <summary>
        /// Performance counters are a relative new concept that can be used to create in-process
        /// performance counters
        /// </summary>
        /// <param name="elapsedMilliseconds">The time taken for the iteration</param>
        [NonEvent]
        internal void WriteMetric(long elapsedMilliseconds)
        {
            performanceCounter.WriteMetric(elapsedMilliseconds);
        }
    }
}
