<#
    .DESCRIPTION
        Reports all resources that do not follow naming standards.
#>

$connectionName = "AzureRunAsConnection"
try
{
    # Get the connection "AzureRunAsConnection "
    $servicePrincipalConnection=Get-AutomationConnection -Name $connectionName         

    "Logging in to Azure..."
    Add-AzureRmAccount `
        -ServicePrincipal `
        -TenantId $servicePrincipalConnection.TenantId `
        -ApplicationId $servicePrincipalConnection.ApplicationId `
        -CertificateThumbprint $servicePrincipalConnection.CertificateThumbprint 
}
catch {
    if (!$servicePrincipalConnection)
    {
        $ErrorMessage = "Connection $connectionName not found."
        throw $ErrorMessage
    } else{
        Write-Error -Message $_.Exception
        throw $_.Exception
    }
}

#Get all ARM resources from all resource groups
$ResourceGroups = Get-AzureRmResourceGroup 

foreach ($ResourceGroup in $ResourceGroups)
{    
    Write-Output ("Showing resources in resource group " + $ResourceGroup.ResourceGroupName)
    $Resources = Find-AzureRmResource -ResourceGroupNameContains $ResourceGroup.ResourceGroupName | Select ResourceName, ResourceType
    ForEach ($Resource in $Resources)
    {
        if(
 ($Resource.ResourceType -eq "Microsoft.Devices/IotHubs" -and !($Resource.ResourceName -match '^([i][o][t][h][u][b]-[a-zA-Z0-9]*)+$')) -or ($Resource.ResourceType -eq "Microsoft.Storage/storageAccounts" -and !($Resource.ResourceName -match '^([s][t][o][r][a][g][e][a-zA-Z0-9]*)+$')) -or ($Resource.ResourceType -eq "Microsoft.Web/sites" -and !($Resource.ResourceName -match '^([f][u][n][c][t][i][o][n]-[a-zA-Z0-9]*)+$')))
    {
        Write-Output ( "Naming convention violation :" + $Resource.ResourceName + " of type " +  $Resource.ResourceType)
    }

    }
    Write-Output ("Completed analyzing resource group " + $ResourceGroup.ResourceGroupName)
} 
