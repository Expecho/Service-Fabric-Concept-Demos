# Description

Demonstration of how to replace the default EventSource implementation of an EventSource in Service Fabric using rich payloads.

Sections:
- [Rich Payload](https://github.com/Expecho/ServiceFabric-EventSource-Logging/tree/master#introducing-rich-payloads)
- [EventCounters](https://github.com/Expecho/ServiceFabric-EventSource-Logging#eventcounters)

## Logging in Service Fabric

The default EventSource implementation that comes with the templates for a reliable services and actors is too basic, which can lead to wrong extensions. The power of using ETW lies in the capablities to provide [structured logging](https://stackify.com/what-is-structured-logging-and-why-developers-need-it/). In my opinion using the templated `EventSource` does not use this to its full advantage since only the service context information is added as seperate payload values.  But the most important issue is, that *your* information you want to log ends up in a simple string:

```csharp
        [NonEvent]
        public void ServiceMessage(StatelessServiceContext serviceContext, string message, params object[] args)
        {
            if (IsEnabled())
            {
                string finalMessage = string.Format(message, args);
                ServiceMessage(
                    serviceContext.ServiceName.ToString(),
                    serviceContext.ServiceTypeName,
                    serviceContext.InstanceId,
                    serviceContext.PartitionId,
                    serviceContext.CodePackageActivationContext.ApplicationName,
                    serviceContext.CodePackageActivationContext.ApplicationTypeName,
                    serviceContext.NodeContext.NodeName,
                    finalMessage);
            }
        }
        
        private const int ServiceMessageEventId = 2;
        [Event(ServiceMessageEventId, Level = EventLevel.Informational, Message = "{7}")]
        private void ServiceMessage(
            string serviceName,
            string serviceTypeName,
            long replicaOrInstanceId,
            Guid partitionId,
            string applicationName,
            string applicationTypeName,
            string nodeName,
            string message)
        {
            WriteEvent(ServiceMessageEventId, serviceName, serviceTypeName, replicaOrInstanceId, partitionId, applicationName, applicationTypeName, nodeName, message);
        }
```        

If you want to create another event you will have to copy these methods and replace the `message` parameter with your own set.

### Introducing rich payloads

Starting with .Net 4.6 (and .Net Core) we can use complex types as arguments to an EventSource Write method as well. In order to be able to support this a complex type needs to be decorated with the `EventData` attribute.
This means that we can write some clean, simple methods to log complex types ...

```csharp
[Event((int)Events.IterationUpdated, Level = EventLevel.Informational, Keywords = Keywords.ServiceExecution)]
public void IterationUpdated(LogContext context, IterationData iterationData)
{
    WriteEvent((int)Events.IterationUpdated, context, iterationData);
}
```

```csharp
// Log the iteration data using a complex type for the context and the data
ServiceEventSource.Current.IterationUpdated(
    Context.ToLogContext(), 
    new IterationData
    {
        Iteration = ++iterations
    });
```

... and still get all the details. Arguments that are instances of a class marked with the `[EventData]` attribute will be stored as a json array of key/value pairs:

```json
{
  "Timestamp": "2018-06-28T13:50:20.135291+02:00",
  "ProviderName": "MyCompany-DynamicEventSourceDemo-DemoService",
  "Id": 699,
  "Message": null,
  "ProcessId": 12456,
  "Level": "Informational",
  "Keywords": "0x0000F00000000001",
  "EventName": "IterationUpdated",
  "ActivityID": "00000033-0001-0000-a830-0000ffdcd7b5",
  "RelatedActivityID": null,
  "Payload": {
    "context": "[{\"Key\":\"ServiceTypeName\",\"Value\":\"DemoServiceType\"},{\"Key\":\"ServiceName\",\"Value\":\"/DynamicEventSourceDemo/DemoService\"},{\"Key\":\"ApplicationTypeName\",\"Value\":\"DynamicEventSourceDemoType\"},{\"Key\":\"ApplicationName\",\"Value\":\"fabric:/DynamicEventSourceDemo\"},{\"Key\":\"NodeName\",\"Value\":\"_Node_0\"},{\"Key\":\"CodePackageVersion\",\"Value\":\"1.0.0\"},{\"Key\":\"ReplicaOrInstanceId\",\"Value\":131746601862682860},{\"Key\":\"PartitionId\",\"Value\":\"15af1ec4-a57b-45b3-afe0-e7f4a0930c26\"}]",
    "iterationData": "[{\"Key\":\"Iteration\",\"Value\":42},{\"Key\":\"TimeStamp\",\"Value\":\"\/Date(1530193820133)\/\"}]"
  }
}
```
As shown the Service Fabric ServiceContext is stored like an array in the payload:

```json
[
    {
        "Key": "ServiceTypeName",
        "Value": "DemoServiceType"
    },
    {
        "Key": "ServiceName",
        "Value": "/DynamicEventSourceDemo/DemoService"
    },
    {
        "Key": "ApplicationTypeName",
        "Value": "DynamicEventSourceDemoType"
    },
    {
        "Key": "ApplicationName",
        "Value": "fabric:/DynamicEventSourceDemo"
    },
    {
        "Key": "NodeName",
        "Value": "_Node_0"
    },
    {
        "Key": "CodePackageVersion",
        "Value": "1.0.0"
    },
    {
        "Key": "ReplicaOrInstanceId",
        "Value": 131746601862682860
    },
    {
        "Key": "PartitionId",
        "Value": "15af1ec4-a57b-45b3-afe0-e7f4a0930c26"
    }
]
```

The same applies to the custom type `IterationData`

```json
[
    {
        "Key": "Iteration",
        "Value": 42
    },
    {
        "Key": "TimeStamp",
        "Value": "/Date(1530193820133)/"
    }
]
```

## EventCounters

[EventCounter](https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.Tracing/documentation/EventCounterTutorial.md) is a mechanism for measuring things like performance or occurences for very frequent events.
In the DemoService an EventCounter is used to log the performance of an iteration.
Measurement data of EventCounters is stored as a json array of key/value pairs as well:

```json
{
  "Timestamp": "2018-06-28T13:50:20.0442854+02:00",
  "ProviderName": "MyCompany-DynamicEventSourceDemo-DemoService",
  "Id": 698,
  "Message": null,
  "ProcessId": 12456,
  "Level": "Always",
  "Keywords": "0x0000000000000000",
  "EventName": "EventCounters",
  "ActivityID": null,
  "RelatedActivityID": null,
  "Payload": {
    "Payload": "[{\"Key\":\"Name\",\"Value\":\"performanceCounter\"},{\"Key\":\"Mean\",\"Value\":319},{\"Key\":\"StandardDeviation\",\"Value\":106.62394},{\"Key\":\"Count\",\"Value\":3},{\"Key\":\"Min\",\"Value\":220},{\"Key\":\"Max\",\"Value\":467},{\"Key\":\"IntervalSec\",\"Value\":0.9950398}]"
  }
}
```

```json
[
    {
        "Key": "Name",
        "Value": "performanceCounter"
    },
    {
        "Key": "Mean",
        "Value": 319
    },
    {
        "Key": "StandardDeviation",
        "Value": 106.62394
    },
    {
        "Key": "Count",
        "Value": 3
    },
    {
        "Key": "Min",
        "Value": 220
    },
    {
        "Key": "Max",
        "Value": 467
    },
    {
        "Key": "IntervalSec",
        "Value": 0.9950398
    }
]
```

An EventCounter is created during the construction of the EventSource:

```csharp
performanceCounter = new EventCounter(nameof(performanceCounter), this);
```

and is written to using `WriteMetric`:

```csharp
[NonEvent]
internal void WriteMetric(long elapsedMilliseconds)
{
    performanceCounter.WriteMetric(elapsedMilliseconds);
}
```

To use this counter in the application simply call the EventSource method several times:

```csharp
// Log the performance of this iteration
ServiceEventSource.Current.WriteMetric(sw.ElapsedMilliseconds);
```

### Reading EventCounter data

`EventCounter` data can be read using an `EventListener` just like any other `EventSource` event

```csharp
var reader = new DemoEventSourceReader();
var arguments = new Dictionary<string, string>
{
    {"EventCounterIntervalSec", "1"}
};
reader.EnableEvents(ServiceEventSource.Current, EventLevel.LogAlways, EventKeywords.All, arguments);
```

During the interval counter data is aggregated.