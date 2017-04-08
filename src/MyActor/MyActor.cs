using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using MyActor.Interfaces;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MyActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class MyActor : Actor, IMyActor, IRemindable
    {
        private readonly ActorId actorId;
        private const string ReminderName = "WakeupCall";

        /// <summary>
        /// Initializes a new instance of MyActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public MyActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
            this.actorId = actorId;
        }

        protected override Task OnPostActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            ActorEventSource.Current.ActorMessage(this, $"Actor {actorId} completed call to {actorMethodContext.MethodName}.");

            return Task.CompletedTask;
        }

        protected override Task OnPreActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            ActorEventSource.Current.ActorMessage(this, $"Actor {actorId} received call to {actorMethodContext.MethodName}.");

            return Task.CompletedTask;
        }

        protected override Task OnDeactivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"Actor {actorId} deactivated.");

            return Task.CompletedTask;
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"Actor {actorId} activated.");

            return Task.CompletedTask;
        }

        public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            ActorEventSource.Current.Message($"Actor {actorId} recieved reminder {reminderName}.");

            var ev = GetEvent<IWakeupCallEvents>();
            ev.WakeupCall(Encoding.ASCII.GetString(state), Id.GetGuidId());

            return Task.CompletedTask;
        }

        public async Task CreateWakeupCallAsync(string message, TimeSpan dueTime, TimeSpan snoozeTime)
        {
            await RegisterReminderAsync(ReminderName, 
                Encoding.ASCII.GetBytes(message),
                dueTime,
                snoozeTime);

            ActorEventSource.Current.Message($"Subscribed to event {message} for actor {Id.GetGuidId()}");
        }

        public async Task DismissWakeupCallAsync(string message, TimeSpan dueTime, TimeSpan snoozeTime)
        {
            await UnregisterReminderAsync(GetReminder(ReminderName));

            ActorEventSource.Current.Message($"Actor {actorId} dismissed event {message}.");
        }
    }
}
