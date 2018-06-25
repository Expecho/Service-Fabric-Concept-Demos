# Overview

from [the docs](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-application-and-service-manifests):

> ConfigPackage declares a folder, named by the Name attribute, that contains a Settings.xml file. The settings file contains sections of user-defined, key-value pair settings that the process reads back at run time. During an upgrade, if only the ConfigPackage version has changed, then the running process is not restarted. Instead, a callback notifies the process that configuration settings have changed so they can be reloaded dynamically. Here is an example Settings.xml file

This application demonstrates how a service can react on configuration changes. It shows that the service instance is not restarted but instead changes are picked up at runtime.

## How to use

TODO