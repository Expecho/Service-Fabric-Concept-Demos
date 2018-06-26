using Microsoft.ServiceFabric.Actors;
using System;
using System.Threading.Tasks;

namespace MyActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IMyActor : IActor, IActorEventPublisher<IWakeupCallEvents>
    {
        Task CreateWakeupCallAsync(string message, TimeSpan dueTime, TimeSpan snoozeTime);
        Task DismissWakeupCallAsync();
    }
}
