# Refinery Load balancing with Azure EventHubs

This is an example of using Azure EventHubs and it's kafka protocol as a mechanism for load balancing between independent refinery nodes.

In this scenario, there is no refinery cluster, just independent refinery nodes that use kafka partitions to ensure that spans for a trace all get sent to the same instance.

This repo uses Pulumi to build the eventhubs, and will eventually use pulumi to build the Container Apps.

There is also a devcontainer that contains all the necessary dependencies.

## Running

Login to azure

```shell
az login
```

Create the Event hub with pulumi.

```shell
mkdir state
pulumi login file://${PWD}/state
cd infra
pulumi stack select
pulumi up
```

Run the containers locally

```shell
./utils/start.sh
```
