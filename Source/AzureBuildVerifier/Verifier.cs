using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.Azure.Configuration;
using Sitecore.Azure.Managers;
using Sitecore.Azure.Pipelines.BasePipeline;
using Sitecore.Pipelines;

namespace AzureBuildVerifier
{
    public class Verifier
    {
        private readonly AzureDeploymentContext _azureDeploymentContext;
        private const string PipelineDevFabric = "azureBuildVerifierDevFabric";
        private const string PipelineAzure = "azureBuildVerifierAzure";

        public Verifier(AzureDeploymentContext azureDeploymentContext)
        {
            _azureDeploymentContext = azureDeploymentContext;
        }

        public IEnumerable<string> GetInvalidPaths()
        {
            var pipelineName = _azureDeploymentContext.EnvironmentDefinition.IsDevFabric ? PipelineDevFabric : PipelineAzure;
            var args = new AzureRolePipelineArgs() { Deployment = _azureDeploymentContext.Deployment };

            var pipeline = PipelineFactory.GetPipeline(pipelineName);
            var manager = new PipelineJobManager(pipeline, args);       // required by ResolveSources pipeline processor

            CorePipeline.Run(pipelineName, args);

            var artifacts = args.CustomData["artifacts"] as IEnumerable<string>;
            if (artifacts == null) return null;

            var deploymentPath = GetDestinationPath(args);
            if (string.IsNullOrEmpty(deploymentPath)) return null;

            var processor = new ArtifactsAnalyzer(artifacts, Settings.WebSiteFolder.FullName, deploymentPath);
            return processor.InvalidPaths;
        }

        private string GetDestinationPath(AzureRolePipelineArgs args)
        {
            bool hasPackage = args.CustomData["Package"] != null && args.CustomData["Package"] as FileInfo != null;
            if (!hasPackage) return string.Empty;

            var package = (args.CustomData["Package"] as FileInfo).FullName;
            if (string.IsNullOrEmpty(package)) return string.Empty;

            var webRole = _azureDeploymentContext.Deployment.ServiceDefinition.WebRole.FirstOrDefault();
            if (webRole == null) return string.Empty;

            return string.Format(@"{0}\roles\{1}\approot", package, webRole.name);
        }
    }
}