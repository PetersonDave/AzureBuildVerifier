using System;
using System.Collections.Generic;
using System.IO;
using AzureBuildVerifier.Processors.Sharknado;
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
            var deploymentPath = GetLatestDeploymentPath();

            var args = new RolePipelineArgsBase {Deployment = _azureDeploymentContext.Deployment};
            var artifactsProcessor = new ArtifactsProcessor();

            CorePipeline.Run("ArtifactsProcessor", args);

            return null;


            //var processor = new Processor(deploymentPath, _environment.BuildFolder);
            //processor.ProcessDirectory(_environment.BuildFolder);

            //return processor.InvalidPaths;
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