# Get-HandsOn : How we reviewed the codebase of Multi-million dollar Azure and IoT Enterprise Application

Hi! Welcome to the hands-on lab. 

Are you having the nightmare of reviewing and maintaining a large Enterprise Application code base ? 

In this lab we are going to see 4 steps that can help us deliver code with good quality, consistency and reduce redundant manual effort during development.

The 4 steps are : 
1. Leveraging the on-the-fly code analysis capability of Roslyn to detect vulnerabilitis while developing right in your IDE and further leverage its code-generation capability of Roslyn to inject custom code and documentation and saved a lot of development effort!

2. Integrating static code analyzers in CI/CD pipeline

3. Leveraging Azure Automation with run books to report compliance and auto-fix resources as per custom rules

4. Leveraging Azure Monitor, Azure Advisor, Azure Security to monitor the resources in action.


# Steps :

1. Open the solution **"Quickstarts.sln"** on your desktop, in Visual Studio
2. Build the solution
3. Open App.config in the path Quickstarts/ConfigUpdater/
4. Under appsettings ,update the value of "aliasOrUniquevalue"  with your alias or a unique value < add key="aliasOrUniquevalue" value="xxxx"/>
5. Build and execute the ConfigUpdater project. This will update the names of the resources in ARM template.

## Observe and Run custom code analyzers 
1. Open the containing folder of the **"Quickstarts.sln"**
2. Navigate to CustomAnalyzers>Analyser VSIXs Folder
3. Double click on the AttributeAnalyzer.vsix
4. This would open the VSIX Installer, Selected the Visual Studio 2017 and click Install
5. Please close the VSIX Installer dialog box and restart your instance of Visual Studio
6. In Visual Studio go to the Tools>Extension and Updates on the navigation bar at the top
7. You would see the the AttributeInstaller installed
8. Open the Quickstarts.sln in the Visual Studio instance
9. Open the file ____
10. Go to the PostAsync() Method, you would see a green warning indicated for the method
11. Hover over it and you would see the message "Necessery or its Alternate Attribute is missing"
12. You would also see a small yellow bulb, Click the arrow button. You would see the message "Add required Attributes"
13. Clicking this would add the necessary Attribute to the method. Resolve the necessary namespaces.

## Integrate static code analyzers in Azure DevOps CI/CD Pipeline
1. Observe the session as the instructor demonstrates integrating code analysers in DevOps pipeline.
https://docs.microsoft.com/en-us/azure/devops/articles/security-validation-cicd-pipeline?view=vsts 

## Execute Azure Automation runbook

1. Deploy the ARM deployment project Quickstarts/ProvisioningProject
2. If asked for a resource group name, enter "quickstarts"
3. Using Azure portal, check if resources are deployed
4. Once resources are deployed, create a Azure automation account on the Azure portal (+Create a resource -> Management tools -> Automation)
5. Once the automation account us created, on the left menu pane of the resource, click on 'runbooks'
6. Click on '+ Create a runbook'
7. Enter the 'Name' as 'Naming convention check'
8. Select and 'Runbook Type' as **PowerShell**
9. Click on 'Edit' once the runbook is created
10. Copy the PowerShell code in 'Quickstarts/ProvisioningProject/AutomationRunbooks/ReportNamingConvention.ps1' and paste in the runbook editor.
11. Click on 'Save'. Once Saved click on 'Test Pane'
12. Click on 'Start' to test the runbook.
13. The Runbook will report, if any resources have been created without following naming conventions 
14. If you do not see any violations reported, create a storage with a name that does not start with 'storage-'. On running, the runbook will report the storage name in such case.

## Effectively use Azure Security Center and Azure Advisor

1. Observe as the instructor shows you the issues that can be monitored using Azure Advisor and Azure Security Center




