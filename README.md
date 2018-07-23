# Service-Fabric-Concept-Demos
Repository demonstrating various Service Fabric concepts using simple demo applications that can easily run on a local cluster.

## [Service Fabric Actors Events & Reminders](https://github.com/Expecho/Service-Fabric-Concept-Demos/tree/master/src/Actors.RemindersAndEvents)

This sample contains an Azure Service Fabric application that functions as a demonstration of Service Fabric Actors touching Events & Reminders

## [Reliable Services Configuration Changes](https://github.com/Expecho/Service-Fabric-Concept-Demos/tree/master/src/Application.Configuration)

Service Fabric uses Configuration Packages to bundle a configuration with service instances. Those can be versioned and deployed independently of the service code. This application demonstrates how a service can react on configuration changes. It shows that the service instance is not restarted but instead changes are picked up at runtime.

## [Service Fabric ETW Logging](https://github.com/Expecho/Service-Fabric-Concept-Demos/tree/master/src/ServiceFabric.EventSource)

Demonstration of how to replace the default EventSource implementation of an EventSource in Service Fabric using rich payloads. This also includes the use of the `EventCounter` class to log metrics.
