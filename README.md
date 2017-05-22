# Service-Fabric-Actors-Playground
Playground for Service Fabric Actors touching Events &amp; Reminders

# Getting started
Deploy the application to an Azure Service Fabric cluster and point a browser to the following url:

http://localhost:8251/api/Reminders/?message=Hello World&minutes=1&snoozeTime=500000

This will create a reminder that fires after 1 minute and says 'Hello World'

This demo showcases how the actor is automatically reactived whenever a reminder is due.

(The actor is set to deactive after an inactive period of 20 seconds)
