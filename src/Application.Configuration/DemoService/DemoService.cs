using System;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

namespace DemoService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class DemoService : StatelessService
    {
        public DemoService(StatelessServiceContext context)
            : base(context)
        {
            // Register for configuration changes
            Context.CodePackageActivationContext.ConfigurationPackageModifiedEvent += ConfigurationPackageModifiedEvent;
            Context.CodePackageActivationContext.ConfigurationPackageAddedEvent += ConfigurationPackageAddedEvent;
            Context.CodePackageActivationContext.ConfigurationPackageRemovedEvent += ConfigurationPackageRemovedEvent;
        }

        /// <summary>
        /// Type of configuration change
        /// </summary>
        private enum ConfigurationPackageEvent
        {
            Added,
            Removed,
            Modified
        }

        private void ConfigurationPackageRemovedEvent(object sender, PackageRemovedEventArgs<ConfigurationPackage> e)
        {
            DumpConfiguration(ConfigurationPackageEvent.Removed, e.Package);
        }

        private void ConfigurationPackageAddedEvent(object sender, PackageAddedEventArgs<ConfigurationPackage> e)
        {
            DumpConfiguration(ConfigurationPackageEvent.Added, e.Package);
        }

        private void ConfigurationPackageModifiedEvent(object sender, PackageModifiedEventArgs<ConfigurationPackage> e)
        {
            DumpConfiguration(ConfigurationPackageEvent.Modified, e.OldPackage, "previous: ");
            DumpConfiguration(ConfigurationPackageEvent.Modified, e.NewPackage, "current: ");
        }

        /// <summary>
        /// On change of configuration, dump the current configuration (and where applicable, the previous configuration)
        /// </summary>
        /// <param name="configurationPackageEvent">Type of event</param>
        /// <param name="configurationPackage">The <see cref="ConfigurationPackage"/> that is changed</param>
        /// <param name="prefix">Optional logmessage prefix</param>
        /// <remarks>This would be the place to retrieve the configuration changes and apply them without having to restart the service</remarks>
        private void DumpConfiguration(ConfigurationPackageEvent configurationPackageEvent, ConfigurationPackage configurationPackage, string prefix = "")
        {
            var parameters = configurationPackage.Settings.Sections.SelectMany(s => s.Parameters.Select(p => $"{p.Name}: {p.Value}"));
            var parametersDescription = string.Join(Environment.NewLine, parameters);
            ServiceEventSource.Current.Message($"{prefix}{configurationPackageEvent} - {configurationPackage.Description.Name}: {parametersDescription}");
        }

        /// <summary>
        /// Runs once during the lifetime of the service instance
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            DumpConfiguration(ConfigurationPackageEvent.Modified, Context.CodePackageActivationContext.GetConfigurationPackageObject("Config"), "values at startup: ");
            return Task.CompletedTask;
        }
    }
}
