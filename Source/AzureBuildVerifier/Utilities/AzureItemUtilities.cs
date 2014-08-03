using Sitecore.Azure.Deployments.AzureDeployments;
using Sitecore.Azure.Deployments.Environments;
using Sitecore.Azure.Deployments.Farms;
using Sitecore.Azure.Deployments.Locations;
using Sitecore.Azure.Deployments.Roles;
using Sitecore.Data;

namespace AzureBuildVerifier.Utilities
{
    public class AzureItemUtilities
    {
        public static EnvironmentItem GetEnvironmentItem(ID deploymentId)
        {
            var db = Database.GetDatabase("master");

            var deploymentDataItem = db.Items[deploymentId];
            var azureDeploymentItem = new AzureDeploymentItem(deploymentDataItem);

            var roleDataItem = azureDeploymentItem.InnerItem.Parent;
            var roleItem = new WebRoleItem(roleDataItem);
            
            var farmDataItem = roleItem.InnerItem.Parent;
            var farmItem = new FarmItem(farmDataItem);

            var locationDataItem = farmItem.InnerItem.Parent;
            var locationItem = new LocationItem(locationDataItem);

            var environmentDataItem = locationItem.InnerItem.Parent;
            var environmentItem = new EnvironmentItem(environmentDataItem);

            return environmentItem;
        }
    }
}
