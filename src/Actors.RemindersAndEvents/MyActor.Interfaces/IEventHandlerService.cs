using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace MyActor.Interfaces
{
    public interface IEventHandlerService : IService
    {
        Task<string> CreateWakeupCallAsync(string message, int minutes, int snoozeTime);
    }
}