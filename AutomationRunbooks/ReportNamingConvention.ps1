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
        if($Resource.ResourceType -eq "Microsoft.Devices/IotHubs" -and !(($Resource.ResourceName) -match '^([a-zA-Z0-9]*-[a-zA-Z0-9]*-[a-zA-Z0-9]*)+$'))
        {
        Write-Output ( "Naming convention violation :" + $Resource.ResourceName + " of type " +  $Resource.ResourceType)
        }
    }
    Write-Output ("Completed analyzing resource group " + $ResourceGroup.ResourceGroupName)
} 
