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

        /// <summary>
        /// Called before a method is invoked on the actor
        /// </summary>
        /// <param name="actorMethodContext"></param>
        /// <returns></returns>
        protected override Task OnPreActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            ActorEventSource.Current.ActorMessage(this, $"Actor {actorId} received call to {actorMethodContext.MethodName}.");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Called after a method is invoked on the actor
        /// </summary>
        /// <param name="actorMethodContext"></param>
        /// <returns></returns>
        protected override Task OnPostActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            ActorEventSource.Current.ActorMessage(this, $"Actor {actorId} completed call to {actorMethodContext.MethodName}.");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when an actor is deactived due to inactivity
        /// </summary>
        /// <returns></returns>
        protected override Task OnDeactivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"Actor {actorId} deactivated.");

            return Task.CompletedTask;
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"Actor {actorId} activated.");

            return Task.CompletedTask;
        }

        /// <summary>
        /// This method is part of the <see cref="IRemindable"/> interface and is called when te reminder is recieved
        /// </summary>
        /// <param name="reminderName">The name of the reminder</param>
        /// <param name="state">The state as set when the reminder was created</param>
        /// <param name="dueTime">The period after wich the reminder is due</param>
        /// <param name="snoozeTime">If the reminder is not unregistered after dueTime it will continue to activate after every specified time</param>
        /// <returns></returns>
        public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan snoozeTime)
        {
            ActorEventSource.Current.Message($"Actor {actorId} recieved reminder {reminderName} that will activate in {dueTime.TotalMinutes} minutes.");

            var ev = GetEvent<IWakeupCallEvents>();
            ev.WakeupCall(Encoding.ASCII.GetString(state), Id.GetGuidId());

            return Task.CompletedTask;
        }

        /// <summary>
        /// Register a reminder
        /// </summary>
        /// <param name="message">A message passed as state to the reminder so it can be read when the reminder is due</param>
        /// <param name="dueTime">The period after wich the reminder is due</param>
        /// <param name="snoozeTime">If the reminder is not unregistered after dueTime it will continue to activate after every specified time</param>
        /// <returns></returns>
        public async Task CreateWakeupCallAsync(string message, TimeSpan dueTime, TimeSpan snoozeTime)
        {
            await RegisterReminderAsync(ReminderName, 
                Encoding.ASCII.GetBytes(message),
                dueTime,
                snoozeTime);

            ActorEventSource.Current.Message($"Registered reminder {ReminderName} with message {message} for actor {Id.GetGuidId()}");
        }

        /// <summary>
        /// Unregister the reminder
        /// </summary>
        /// <returns></returns>
        public async Task DismissWakeupCallAsync()
        {
            await UnregisterReminderAsync(GetReminder(ReminderName));

            ActorEventSource.Current.Message($"Actor {actorId} unregistered reminder {ReminderName}.");
        }
    }
}
