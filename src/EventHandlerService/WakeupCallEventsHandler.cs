using System;
using MyActor.Interfaces;

namespace EventHandlerService
{
    public class WakeupCallEventsHandler : IWakeupCallEvents
    {
        public void WakeupCall(string message, Guid actorId)
        {
            ServiceEventSource.Current.Message($"Received event {message} from {actorId}");
        }
    }
}