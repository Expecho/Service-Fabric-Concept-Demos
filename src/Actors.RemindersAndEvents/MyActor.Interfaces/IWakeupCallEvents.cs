using System;
using Microsoft.ServiceFabric.Actors;

namespace MyActor.Interfaces
{
    /// <summary>
    /// Handles events raised by an actor
    /// </summary>
    public interface IWakeupCallEvents : IActorEvents
    {
        /// <summary>
        /// Handle WakeupCall event
        /// </summary>
        /// <param name="message">The message received from the actor reminder</param>
        /// <param name="actorId">The sending actor</param>
        void WakeupCall(string message, ActorId actorId);
    }
}