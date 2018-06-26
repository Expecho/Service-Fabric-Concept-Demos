# Overview

Service Fabric uses [Configuration Packages](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-application-and-service-manifests) to bundle a configuration with service instances. Those can be versioned and deployed independently of the service code.

from [the docs](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-application-and-service-manifests):

> ConfigPackage declares a folder, named by the Name attribute, that contains a Settings.xml file. The settings file contains sections of user-defined, key-value pair settings that the process reads back at run time. During an upgrade, if only the ConfigPackage version has changed, then the running process is not restarted. Instead, a callback notifies the process that configuration settings have changed so they can be reloaded dynamically. Here is an example Settings.xml file

This application demonstrates how a service can react on configuration changes. It shows that the service instance is not restarted but instead changes are picked up at runtime.

## How to use

### Initial deployment

When the application is deployed and DemoService is instantiated the configuration is read from [Settings.xml](https://github.com/Expecho/Service-Fabric-Concept-Demos/blob/master/src/Application.Configuration/DemoService/PackageRoot/Config/Settings.xml):

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Settings xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.microsoft.com/2011/01/fabric">
  <!-- Add your custom configuration sections and parameters here -->
  <Section Name="MyConfigSection">
    <Parameter Name="MyParameter" Value="FirstValue" />
  </Section>
</Settings>
```

As shown in [the logs](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-diagnostics-how-to-monitor-and-diagnose-services-locally#view-service-fabric-system-events-in-visual-studio) the `RunAsync` method is called and the actual configuration is logged:

| Time | Message|
-------|--------|
| 16:45:33 | RunAsync has successfully completed for a stateless service instance.  Application Type Name: Application.ConfigurationType, Application Name: fabric:/Application.Configuration, Service Type Name: DemoServiceType, Service Name: fabric:/Application.Configuration/DemoService, Partition Id: ed8eda41-c7eb-4a55-9eff-ea5ee53df26e, Instance Id: 131744114564715568, WasCancelled: False |
| 16:45:23 | values at startup: None - Config: MyParameter: FirstValue |
| 16:44:24 | RunAsync has been invoked for a stateless service instance.  Application Type Name: Application.ConfigurationType, Application Name: fabric:/Application.Configuration, Service Type Name: DemoServiceType, Service Name: fabric:/Application.Configuration/DemoService, Partition Id: ed8eda41-c7eb-4a55-9eff-ea5ee53df26e, Instance Id: 131744114564715568 |
| 16:44:23 | Service host process 27012 registered service type DemoService |

### Upgrading the configuration

Change the [Settings.xml](https://github.com/Expecho/Service-Fabric-Concept-Demos/blob/master/src/Application.Configuration/DemoService/PackageRoot/Config/Settings.xml) file and change the value of the parameter, for example 

```xml
<Section Name="MyConfigSection">
    <Parameter Name="MyParameter" Value="SecondValue" />
</Section>
```

Next, modify the [ServiceManifest.xml](https://github.com/Expecho/Service-Fabric-Concept-Demos/blob/master/src/Application.Configuration/DemoService/PackageRoot/ServiceManifest.xml) file and uncomment the second ConfigPackage element to include another configuration section:

```xml
  <!-- Config package is the contents of the Config directoy under PackageRoot that contains an 
       independently-updateable and versioned set of custom configuration settings for your service. -->
  <ConfigPackage Name="Config" Version="1.0.0" />
  <ConfigPackage Name="OtherConfig" Version="1.0.0" />
```

### Deploy the configuration changes

Next, deploy the application again and change the versions using the [Manifest Versions] button like this:

![Edit Versions](blobs/upgrade-application-settings.png?raw=true)

After deployment observe that the `RunAsync` is not callled. The service instance is not restarted during this upgrade. Instead, the following events are fired and handled:

```csharp
public DemoService(StatelessServiceContext context)
    : base(context)
{
    // Register for configuration changes
    Context.CodePackageActivationContext.ConfigurationPackageModifiedEvent += ConfigurationPackageModifiedEvent;
    Context.CodePackageActivationContext.ConfigurationPackageAddedEvent += ConfigurationPackageAddedEvent;
}
```

The configuration is logged and will show:

| Time | Message|
-------|--------|
| 16:48:25 | Added - OtherConfig: MyParameter: FirstValue |
| 16:48:24 | values at startup: None - Config: MyParameter: FirstValue |
| 16:48:24 | current: Modified - Config: MyParameter: SecondValue |
| 16:48:23 | previous: Modified - Config: MyParameter: FirstValue |

Notice the changed parameter values of the upgraded "Config" package and the new values for the "OtherConfig" package.
