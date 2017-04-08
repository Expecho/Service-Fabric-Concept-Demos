using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using MyActor.Interfaces;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class RemindersController : Controller
    {
        // http://localhost:8251/api/Reminders/?message=Hello World&minutes=1&snoozeTime=500000
        [HttpGet]
        public Task<string> Get(string message, int minutes, int snoozeTime)
        {
            var proxy = ServiceProxy.Create<IEventHandlerService>(new Uri("fabric:/ServiceFabric.ActorsDemo/EventHandlerService"), new ServicePartitionKey(0));
            return proxy.CreateWakeupCallAsync(message, minutes, snoozeTime);
        }
    }
}
