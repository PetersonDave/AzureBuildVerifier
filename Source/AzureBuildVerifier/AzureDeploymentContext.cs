using System;
using Sitecore.Azure.Configuration;
using Sitecore.Azure.Configuration.Environments;
using Sitecore.Azure.Deployments.AzureDeployments;
using Sitecore.Azure.Deployments.Environments;
using Sitecore.Azure.Deployments.Farms;
using Sitecore.Azure.Deployments.Locations;
using Sitecore.Azure.Deployments.Roles;
using Sitecore.Data;
using Environment = Sitecore.Azure.Deployments.Environments.Environment;

namespace AzureBuildVerifier
{
    public class AzureDeploymentContext
    {
        public AzureDeployment Deployment { get; set; }
        public AzureDeploymentItem AzureDeploymentItem { get; set; }
        public WebRole WebRole { get; set; }
        public WebRoleItem WebRoleItem { get; set; }
        public Farm Farm { get; set; }
        public FarmItem FarmItem { get; set; }
        public Location Location { get; set; }
        public LocationItem LocationItem { get; set; }
        public Environment Environment { get; set; }
        public EnvironmentItem EnvironmentItem { get; set; }
        public EnvironmentDefinition EnvironmentDefinition { get; set; }

        public AzureDeploymentContext(string deploymentIdValue) : this(ID.Parse(deploymentIdValue)) { }

        public AzureDeploymentContext(ID deploymentId)
        {
            var db = Database.GetDatabase("master");

            var azureDeploymentDataItem = db.Items[deploymentId];
            AzureDeploymentItem = new AzureDeploymentItem(azureDeploymentDataItem);

            var roleDataItem = AzureDeploymentItem.InnerItem.Parent;
            WebRoleItem = new WebRoleItem(roleDataItem);

            var farmDataItem = WebRoleItem.InnerItem.Parent;
            FarmItem = new FarmItem(farmDataItem);

            var locationDataItem = FarmItem.InnerItem.Parent;
            LocationItem = new LocationItem(locationDataItem);

            var environmentDataItem = LocationItem.InnerItem.Parent;
            EnvironmentItem = new EnvironmentItem(environmentDataItem);

            DeploymentType deploymentType;
            Enum.TryParse(FarmItem.DeploymentType, out deploymentType);

            EnvironmentDefinition = Settings.EnvironmentDefinitions[EnvironmentItem.EnvironmentId];
            Environment = Environment.GetEnvironment(EnvironmentDefinition);
            Location = Environment.GetLocation(LocationItem.Name);
            Farm = Location.GetFarm(FarmItem.Name, deploymentType);
            WebRole = Farm.GetWebRole(WebRoleItem.Name);

            var deployment = deploymentType == DeploymentType.ContentDelivery
                ? new ContentDeliveryDeployment(WebRole, AzureDeploymentItem)
                : new ContentEditingDeployment(WebRole, AzureDeploymentItem) as AzureDeployment;

            Deployment = deployment;
        }
    }
}