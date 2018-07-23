using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace DemoService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class DemoService : StatelessService
    {
        public DemoService(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            long iterations = 0;

            var sw = new Stopwatch();
            var random = new Random();

            while (true)
            {
                sw.Restart();

                cancellationToken.ThrowIfCancellationRequested();

                // Log the iteration data using a complex type for the context and the data
                ServiceEventSource.Current.IterationUpdated(
                    Context.ToLogContext(), 
                    new IterationData
                    {
                        Iteration = ++iterations
                    });

                await Task.Delay(TimeSpan.FromMilliseconds(random.Next(50, 1000)), cancellationToken);

                // Log the performance of this iteration
                ServiceEventSource.Current.WriteMetric(sw.ElapsedMilliseconds);

                if (cancellationToken.IsCancellationRequested)
                    break;
            }
        }
    }
}
