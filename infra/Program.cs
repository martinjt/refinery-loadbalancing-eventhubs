using Pulumi;
using Pulumi.AzureNative.EventHub;
using Pulumi.AzureNative.EventHub.Inputs;
using Pulumi.AzureNative.Resources;
using System.Collections.Generic;

return await Pulumi.Deployment.RunAsync(() =>
{
    var resourceGroup = new ResourceGroup("collector-eventhub-rg");
    var eventhubNamespace = new Namespace("collector-eventhub-ns", new()
    {
        ResourceGroupName = resourceGroup.Name,
        Sku = new SkuArgs
        {
            Name = SkuName.Standard,
            Tier = SkuTier.Standard,
        },
    });
    var eventHub = new EventHub("collector-eventhub", new()
    {
        MessageRetentionInDays = 4,
        NamespaceName = eventhubNamespace.Name,
        PartitionCount = 4,
        ResourceGroupName = resourceGroup.Name,
        RetentionDescription = new RetentionDescriptionArgs
        {
            CleanupPolicy = "Compact",
            RetentionTimeInHours = 96,
            TombstoneRetentionTimeInHours = 1,
        },
        Status = EntityStatus.Active,
    });

    var sendRule = new EventHubAuthorizationRule("collector-eventhub-auth", new()
    {
        EventHubName = eventHub.Name,
        NamespaceName = eventhubNamespace.Name,
        ResourceGroupName = resourceGroup.Name,
        Rights = { "Send" },
    });

    var listenRule = new EventHubAuthorizationRule("collector-eventhub-auth-listen", new()
    {
        EventHubName = eventHub.Name,
        NamespaceName = eventhubNamespace.Name,
        ResourceGroupName = resourceGroup.Name,
        Rights = { "Listen" },
    });

    var sendKeys = Output.Tuple(eventHub.Name, eventhubNamespace.Name, resourceGroup.Name, sendRule.Name).Apply(
        (names) => ListEventHubKeys.Invoke(new ListEventHubKeysInvokeArgs
    {
        EventHubName = names.Item1,
        NamespaceName = names.Item2,
        ResourceGroupName = names.Item3,
        AuthorizationRuleName = names.Item4,
    }));

    var listenKeys = Output.Tuple(eventHub.Name, eventhubNamespace.Name, resourceGroup.Name, listenRule.Name).Apply(
        (names) => ListEventHubKeys.Invoke(new ListEventHubKeysInvokeArgs
    {
        EventHubName = names.Item1,
        NamespaceName = names.Item2,
        ResourceGroupName = names.Item3,
        AuthorizationRuleName = names.Item4,
    }));

    return new Dictionary<string, object?>
    {
        ["eventHubName"] = eventHub.Name,
        ["eventHubNamespaceName"] = eventhubNamespace.Name,
        ["eventHubResourceGroupName"] = resourceGroup.Name,
        ["eventHubEndpoint"] = Output.Format($"{eventhubNamespace.Name}.servicebus.windows.net:9093"),
        ["eventHubSendKey"] = sendKeys.Apply(keys => keys.PrimaryConnectionString),
        ["eventHubListenKey"] = listenKeys.Apply(keys => keys.PrimaryConnectionString),
    };
});