using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Data.Notifications;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using MyActor.Interfaces;

namespace EventHandlerService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class EventHandlerService : StatefulService, IEventHandlerService
    {
        public EventHandlerService(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(this.CreateServiceRemotingListener) };
        }

        public async Task<string> CreateWakeupCallAsync(string message, int minutes, int snoozeTime)
        {
            var actorId = Guid.NewGuid();
            var proxy = ActorProxy.Create<IMyActor>(new ActorId(actorId));
            await proxy.CreateWakeupCallAsync(
                message,
                TimeSpan.FromMinutes(minutes),
                TimeSpan.FromSeconds(snoozeTime));

            var state = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, string>>("state");
            using (var tx = StateManager.CreateTransaction())
            {
                await state.AddAsync(tx, actorId, message);
                await tx.CommitAsync();
            }

            ServiceEventSource.Current.ServiceMessage(Context, $"Reminder {message} created for actor {actorId}");

            return "Reminder created";
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var state = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, string>>("state");
            state.DictionaryChanged += StateOnDictionaryChanged;
        }

        private async void StateOnDictionaryChanged(object sender, NotifyDictionaryChangedEventArgs<Guid, string> notifyDictionaryChangedEventArgs)
        {
            if(notifyDictionaryChangedEventArgs.Action != NotifyDictionaryChangedAction.Add)
                return;

            var addEvent = (NotifyDictionaryItemAddedEventArgs<Guid, string>)notifyDictionaryChangedEventArgs;

            var proxy = ActorProxy.Create<IMyActor>(new ActorId(addEvent.Key));
            await proxy.SubscribeAsync<IWakeupCallEvents>(new WakeupCallEventsHandler());

            ServiceEventSource.Current.ServiceMessage(Context, $"Subscribed to event {addEvent.Value} for actor {addEvent.Key}");
        }
    }
}
