using Sitecore.Azure.Deployments;
using Sitecore.Azure.Deployments.Farms;

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

        public VerifierSettings(string environmentType, string locationName, string farmName, DeploymentType deploymentType, string roleName, DeploymentSlot deploymentSlot, bool isDevFabric)
        {
            EnvironmentType = environmentType;
            LocationName = locationName;
            FarmName = farmName;
            DeploymentType = deploymentType;
            RoleName = roleName;
            DeploymentSlot = deploymentSlot;
            IsDevFabric = isDevFabric;
        }
    }
}