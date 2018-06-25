using System;
using Microsoft.ServiceFabric.Actors;

namespace MyActor.Interfaces
{
    public interface IWakeupCallEvents : IActorEvents
    {
        void WakeupCall(string message, Guid actorId);
    }
}