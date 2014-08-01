using System;
using System.IO;
using AzureBuildVerifier.Exceptions;
using Sitecore.Azure.Configuration;
using Sitecore.Azure.Deployments.AzureDeployments;
using Sitecore.Azure.Sys.IO;
using Sitecore.IO;
using Environment = Sitecore.Azure.Deployments.Environments.Environment;

namespace AzureBuildVerifier
{
    public class Verifier
    {
        private readonly VerifierSettings _verifierSettings;
        private readonly Environment _environment;

        public Verifier(VerifierSettings verifierSettings)
        {
            _verifierSettings = verifierSettings;
            AssertVerifierSettings();

            _environment = GetEnvironment();
        }

        private void AssertVerifierSettings()
        {
            if (_verifierSettings == null) throw new ArgumentNullException("verifierSettings");

            bool argsValid = !string.IsNullOrEmpty(_verifierSettings.EnvironmentType) &&
                             !string.IsNullOrEmpty(_verifierSettings.FarmName) &&
                             !string.IsNullOrEmpty(_verifierSettings.LocationName) &&
                             !string.IsNullOrEmpty(_verifierSettings.RoleName);

            if (!argsValid) throw new Exception("verifierSettings must have values for all properties");
        }

        public void Verify()
        {
            var deploymentPath = GetLatestDeploymentPath();
            var processor = new Processor(deploymentPath, _environment.BuildFolder);
            processor.ProcessDirectory(_environment.BuildFolder);
        }

        private Environment GetEnvironment()
        {
            var environmentDefinition = Settings.EnvironmentDefinitions.GetEnvironment(_verifierSettings.EnvironmentType);
            if (environmentDefinition == null) throw new VerifierException(string.Format("Could not obtain environment definition from {0}", _verifierSettings.EnvironmentType));

            var environment = Environment.GetEnvironment(environmentDefinition);
            if (environment == null) throw new VerifierException(string.Format("Could not obtain environment from valid environment definition {0}", _verifierSettings.EnvironmentType));

            return environment;
        }

        private AzureDeployment GetAzureDeployment()
        {
            var location = _environment.GetLocation(_verifierSettings.LocationName);
            if (location == null) throw new VerifierException(string.Format("Could not obtain location for {0}", _verifierSettings.LocationName));

            var farm = location.GetFarm(_verifierSettings.FarmName, _verifierSettings.DeploymentType);
            if (farm == null) throw new VerifierException(string.Format("Could not obtain farm name {0} and deploymen type {1}", _verifierSettings.FarmName, _verifierSettings.DeploymentType));

            var role = farm.GetWebRole(_verifierSettings.RoleName);
            if (role == null) throw new VerifierException(string.Format("Could not obtain web role {0}", _verifierSettings.RoleName));

            var deployment = role.GetDeployment(_verifierSettings.DeploymentSlot);
            if (deployment == null) throw new VerifierException(string.Format("Could not obtain deployment for slot {0}", _verifierSettings.DeploymentSlot));

            return deployment;
        }

        private string GetLatestDeploymentPath()
        {
            var packagesDirectoryInfo = new DirectoryInfo(FileUtil.MapDataFilePath("AzurePackages"));
            var buildFolderPathInfo = VariablesReplacer.MapFolderPathWithVariables(_environment.BuildFolder, packagesDirectoryInfo, Settings.GlobalVariables);
            if (!buildFolderPathInfo.Exists) throw new Exception(string.Format("Default build folder does not exist for environment: {0}", _environment.BuildFolder));

            string deploymentFolder = GetDeploymentFolder();

            int folderCounter = 0;
            DirectoryInfo deployDirectoryInfo = buildFolderPathInfo;
            while (deployDirectoryInfo.Exists)
            {
                deployDirectoryInfo = new DirectoryInfo(FileUtil.MakePath(buildFolderPathInfo.FullName, string.Format("({0}) {1}", folderCounter, deploymentFolder), '\\'));
                ++folderCounter;
            }

            if (deployDirectoryInfo == null) throw new Exception(string.Format("Deployment directory could not be established for build folder: {0}", buildFolderPathInfo.FullName));

            return deployDirectoryInfo.FullName;
        }

        /// <summary>
        ///     values are hard-coded in:
        ///         1. Sitecore.Azure.Pipelines.DeployAndRun.DevFabric.ResolveSources
        ///         2. Sitecore.Azure.Pipelines.DeployAndRun.Azure.ResolveSources
        /// </summary>
        /// <returns></returns>
        public string GetDeploymentFolder()
        {
            return _verifierSettings.IsDevFabric ? "DevFabric" : "Azure";
        }
    }
}