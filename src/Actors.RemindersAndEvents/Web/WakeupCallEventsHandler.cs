using Microsoft.ServiceFabric.Actors;
using MyActor.Interfaces;

namespace Web
{
    /// <summary>
    /// Handles events raised by an actor
    /// </summary>
    public class WakeupCallEventsHandler : IWakeupCallEvents
    {
        /// <summary>
        /// Handle WakeupCall event
        /// </summary>
        /// <param name="message">The message received from the actor reminder</param>
        /// <param name="actorId">The sending actor</param>
        public void WakeupCall(string message, ActorId actorId)
        {
            ServiceEventSource.Current.Message($"Received event {message} from {actorId}");
        }
    }
}