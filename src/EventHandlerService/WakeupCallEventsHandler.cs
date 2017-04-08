using System;
using MyActor.Interfaces;

namespace EventHandlerService
{
    public class WakeupCallEventsHandler : IWakeupCallEvents
    {
        public void WakeupCall(string message)
        {
            Console.WriteLine(message);
        }
    }
}