# Overview
This repository contains an Azure Service Fabric application that functions as a  playground for Service Fabric Actors touching Events &amp; Reminders

The application consist of a stateless services hosting an ASP.Net Core application that acts as an Web Api and an actor service that is being called from the Web Api.

![Application Overview](blobs/overview.PNG?raw=true)

# How it works
The actor exposes a method that allows a reminder to be set. It also exposes an event that can be subscribed to. When the reminder is due the event will be raised. Any subscribers will so be alerted.

By calling the web api a reminder is created by the actor. As a second action an event subscription is made by the web api to the exposed actor event.

The actor is set to deactive after an inactive period of 20 seconds so we can observe that the actor will be automatically reactived whenever a reminder is due.

# Getting started
Start a debug session with the application to a local Azure Service Fabric development cluster and point a browser to the following url after the project has been deployed and all services are up and running:

http://localhost:8251/api/Reminders/?message=HelloWorld&minutes=1&snoozeTime=500000

In Visual Studio open the Diagnostic Events window (In the menu View -> Other Windows -> Diagnostic Events) and observe the flow of the application as shown in the picture below (The flow is displayed from bottom to top)

![Application Overview](blobs/asf-actors-output.PNG?raw=true)
