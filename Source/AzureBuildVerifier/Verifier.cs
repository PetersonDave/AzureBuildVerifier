using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Azure.Commands.Context;
using Sitecore.Azure.Configuration;
using Sitecore.Azure.Deployments.AzureDeployments;
using Sitecore.Azure.Deployments.Farms;
using Sitecore.Azure.Deployments.Roles;
using Sitecore.Azure.Pipelines.CreateAzurePackage;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Pipelines;

namespace AzureBuildVerifier
{
    public class Verifier
    {
        public void Verify()
        {
            var test = Settings.EnvironmentDefinitions;

            ID devFabricEvironmentId;
            ID.TryParse("{4FF3DB12-586B-461B-A491-5392E8C492B8}", out devFabricEvironmentId);       // /sitecore/system/Modules/Azure/DevFabric

            var context = new CommandContextBase(devFabricEvironmentId, "Local Emulator");
            var environment = Sitecore.Azure.Deployments.Environments.Environment.GetEnvironment(Settings.EnvironmentDefinitions.GetEnvironment(context.EnvironmentType));

            ID azureDeploymentItemId;
            ID.TryParse("{501FBF6B-7931-40DC-9A85-60F59EB3DA93}", out azureDeploymentItemId);

            var db = Sitecore.Data.Database.GetDatabase("master");
            var azureDeploymentItemItem = db.GetItem(azureDeploymentItemId);
            var azureDeploymentItem = new AzureDeploymentItem(azureDeploymentItemItem);         // /sitecore/system/Modules/Azure/DevFabric/localhost/Delivery01/Role01/Production

            Farm farm = Sitecore.Azure.Deployments.Environments.Environment.GetEnvironment(environment.EnvironmentDefinition).GetLocation("Localhosr").GetFarm("Delivery01", DeploymentType.ContentDelivery);

            var azureRole = new WebRole(farm, (Item) null);

            var cd = new ContentDeliveryDeployment(azureRole, azureDeploymentItem);

            var args = new CreateAzureDeploymentPipelineArgs();
            args.Deployment = cd;

            //var factory = new EntitiesFactory();
            //factory.Create()

            //Farm farm = Sitecore.Azure.Deployments.Environments.Environment.GetEnvironment(environmentDefinition).GetLocation(locationName).GetFarm(farmName, deploymentType);



            var deploymentPipelineArgs = new CreateAzureDeploymentPipelineArgs();
            Pipeline.Start("CreateAzurePackage", deploymentPipelineArgs);
        }
    }
}
