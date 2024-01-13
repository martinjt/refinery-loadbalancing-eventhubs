#!/bin/sh

export PULUMI_CONFIG_PASSPHRASE=
cd infra
eval $(pulumi stack output --shell)

export eventHubNamespaceName=$eventHubNamespaceName
export eventHubName=$eventHubName
export eventHubSendKey=$eventHubSendKey
export eventHubListenKey=$eventHubListenKey
export eventHubEndpoint=$eventHubEndpoint

echo $HONEYCOMB_API_KEY

cd - > /dev/null
docker-compose up -d
