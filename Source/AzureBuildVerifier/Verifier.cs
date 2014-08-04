using System;
using System.Collections.Generic;
using System.IO;
using Sitecore.Azure.Configuration;
using Sitecore.Azure.Pipelines.BasePipeline;
using Sitecore.Azure.Sys.IO;
using Sitecore.IO;
using Sitecore.Pipelines;

namespace AzureBuildVerifier
{
    public class Verifier
    {
        private readonly AzureDeploymentContext _azureDeploymentContext;

        public Verifier(AzureDeploymentContext azureDeploymentContext)
        {
            _azureDeploymentContext = azureDeploymentContext;
        }

        public IEnumerable<string> GetInvalidPaths()
        {
            var args = new RolePipelineArgsBase { Deployment = _azureDeploymentContext.Deployment };

            CorePipeline.Run("azureBuildVerifier", args);
            var artifacts = args.CustomData["artifacts"] as IEnumerable<string>;
            if (artifacts == null) return null;

            var deploymentPath = GetLatestDeploymentPath();
            var processor = new ArtifactsAnalyzer(artifacts, _azureDeploymentContext.Environment.BuildFolder, deploymentPath);

            return processor.InvalidPaths;
        }

        private string GetLatestDeploymentPath()
        {
            var packagesDirectoryInfo = new DirectoryInfo(FileUtil.MapDataFilePath("AzurePackages"));
            var buildFolderPathInfo = VariablesReplacer.MapFolderPathWithVariables(_azureDeploymentContext.Environment.BuildFolder, packagesDirectoryInfo, Settings.GlobalVariables);
            if (!buildFolderPathInfo.Exists) throw new Exception(string.Format("Default build folder does not exist for environment: {0}", _azureDeploymentContext.Environment.BuildFolder));

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
            return _azureDeploymentContext.EnvironmentDefinition.IsDevFabric ? "DevFabric" : "Azure";
        }
    }
}