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
        [HttpGet]
        public async Task<string> Get(string message, int minutes, int snoozeTime)
        {
            var actorId = Guid.NewGuid();
            var proxy = ActorProxy.Create<IMyActor>(new ActorId(actorId));
            await proxy.CreateWakeupCallAsync(
                message,
                TimeSpan.FromMinutes(minutes),
                TimeSpan.FromSeconds(snoozeTime));

            await proxy.SubscribeAsync<IWakeupCallEvents>(new WakeupCallEventsHandler());

            ServiceEventSource.Current.ServiceMessage(context, $"Reminder {message} created for actor {actorId}");

            return "Reminder created";
        }
    }
}
