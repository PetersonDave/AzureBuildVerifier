using System;
using Sitecore.Azure.Configuration;
using Sitecore.Azure.Deployments;
using Sitecore.Azure.Deployments.AzureDeployments;
using Sitecore.Azure.Deployments.Environments;
using Sitecore.Azure.Deployments.Farms;
using Sitecore.Azure.Deployments.Locations;
using Sitecore.Azure.Deployments.Roles;
using Sitecore.Data;

namespace AzureBuildVerifier
{
    public class VerifierSettings
    {
        public string EnvironmentType { get; set; }
        public string LocationName { get; set; }
        public string FarmName { get; set; }
        public DeploymentType DeploymentType { get; set; }
        public string RoleName { get; set; }
        public DeploymentSlot DeploymentSlot { get; set; }
        public bool IsDevFabric { get; set; }

        public VerifierSettings(string deploymentIdValue) : this(ID.Parse(deploymentIdValue)) { }

        public VerifierSettings(ID deploymentId)
        {
            var db = Database.GetDatabase("master");

            var deploymentDataItem = db.Items[deploymentId];
            var deploymentItem = new AzureDeploymentItem(deploymentDataItem);

            var roleDataItem = deploymentItem.InnerItem.Parent;
            var roleItem = new WebRoleItem(roleDataItem);

            var farmDataItem = roleItem.InnerItem.Parent;
            var farmItem = new FarmItem(farmDataItem);

            var locationDataItem = farmItem.InnerItem.Parent;
            var locationItem = new LocationItem(locationDataItem);

            var environmentDataItem = locationItem.InnerItem.Parent;
            var environmentItem = new EnvironmentItem(environmentDataItem);
            var environmentDefinition = Settings.EnvironmentDefinitions[environmentItem.EnvironmentId];

            DeploymentType deploymentType;
            Enum.TryParse(farmItem.DeploymentType, out deploymentType);

            DeploymentSlot deploymentSlot;
            Enum.TryParse(deploymentItem.DeploymentSlot, out deploymentSlot);

            EnvironmentType = environmentDefinition.EnvironmentType;
            LocationName = locationItem.Name;
            FarmName = farmItem.Name;
            DeploymentType = deploymentType;
            RoleName = roleItem.Name;
            DeploymentSlot = deploymentSlot;
            IsDevFabric = environmentDefinition.IsDevFabric;            
        }
    }
}