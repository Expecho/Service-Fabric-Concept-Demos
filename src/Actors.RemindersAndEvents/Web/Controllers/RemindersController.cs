using System;
using System.Fabric;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using MyActor.Interfaces;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class RemindersController : Controller
    {
        private readonly StatelessServiceContext context;

        public RemindersController(StatelessServiceContext context)
        {
            this.context = context;
        }

        // http://localhost:8251/api/Reminders/?message=Hello World&minutes=1&snoozeTime=500000
        /// <summary>
        /// Register a reminder using a new actor
        /// </summary>
        /// <param name="message">A message passed as state to the reminder so it can be read when the reminder is due</param>
        /// <param name="minutes">The period in minutes after wich the reminder is due</param>
        /// <param name="snoozeTime">If the reminder is not unregistered after dueTime it will continue to activate after every specified interval in seconds</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Get(string message, int minutes, int snoozeTime)
        {
            var actorId = ActorId.CreateRandom();
            var proxy = ActorProxy.Create<IMyActor>(actorId);
            await proxy.CreateWakeupCallAsync(
                message,
                TimeSpan.FromMinutes(minutes),
                TimeSpan.FromSeconds(snoozeTime));

            // Subscibe to the actor event that is raised when the reminder is received
            await proxy.SubscribeAsync<IWakeupCallEvents>(new WakeupCallEventsHandler());

            ServiceEventSource.Current.ServiceMessage(context, $"Reminder with message {message} created for actor {actorId.GetLongId()}");

            return "Reminder created";
        }
    }
}
