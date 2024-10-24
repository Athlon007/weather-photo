# This script will deploy the Bicep template to Azure and publish the Azure Functions to the Function App

# I hate Microsoft, Azure and PowerShell - my MacBook feels violated for running this script.
# I hope Bill Gates gets a paper cut on his tongue.

$resourceGroupName = "kfweather"
$location = "westeurope"
$bicepFile = "./main.bicep"
$functionAppName = "kf-weather-app"
$deploymentName = "kfweather-deployment"

# Check if the resource group exists
$resourceGroup = az group show --name $resourceGroupName --output json
if (-not $resourceGroup)
{
    # Create the resource group if it doesn't exist
    az group create --name $resourceGroupName --location $location

    # Wait for the resource group to be created
    # Check every 5 seconds if the resource group exists. Wait for maximum 5 minutes
    $i = 0
    while ($i -lt 60)
    {
        $resourceGroup = az group show --name $resourceGroupName --output json
        if ($resourceGroup)
        {
            break
        }
        Start-Sleep -s 5
        $i++

        if ($i -eq 60)
        {
            Write-Error "Resource group $resourceGroupName was not created"
            exit 1
        }
    }
}

# Deploy Bicep template
$deployment = az deployment group create --resource-group $resourceGroupName --template-file $bicepFile --parameters location=$location --name $deploymentName

# Start the Function App
az functionapp start --name $functionAppName --resource-group $resourceGroupName

# Publish
func azure functionapp publish $functionAppName --build-native-deps --force

# Verify published Azure Functions
$functionsList = az functionapp function list --name $functionAppName --resource-group $resourceGroupName -o table
Write-Host $functionsList