using System;
using System.Fabric;
using MyActor.Interfaces;

namespace EventHandlerService
{
    public class WakeupCallEventsHandler : IWakeupCallEvents
    {
        private readonly StatefulServiceContext context;

        public WakeupCallEventsHandler(StatefulServiceContext context)
        {
            this.context = context;
        }

        public void WakeupCall(string message, Guid actorId)
        {
            ServiceEventSource.Current.ServiceMessage(context, $"Received event {message} from {actorId}");
        }
    }
}