using System.Collections.Generic;
using AzureBuildVerifier.Utilities;
using Sitecore.Azure.Pipelines.BasePipeline;
using Sitecore.Azure.Pipelines.CreateAzurePackage;

namespace AzureBuildVerifier.Processors.Sharknado
{
    public class ArtifactsProcessor : CreateAzureDeploymentPipelineProcessor
    {
        public IEnumerable<string> Artifacts { get; private set; }

        protected override void Action(RolePipelineArgsBase args)
        {
            var excludeFiles = string.Concat(args.Deployment.FilePathFilter.ExcludeFiles, args.Deployment.FilePathFilter.DeploymentTypeExcludeFiles);
            var excludeDirectories = string.Concat(args.Deployment.FilePathFilter.ExcludeDirectories, args.Deployment.FilePathFilter.DeploymentTypeExcludeDirectories);
            var excludeFileExtensions = string.Concat(args.Deployment.FilePathFilter.ExcludeFileExtensions, args.Deployment.FilePathFilter.DeploymentTypeExcludeFileExtensions);

            var environmentItem = AzureItemUtilities.GetEnvironmentItem(args.Deployment.ID);
            Artifacts = GetArtifacts(environmentItem.BuildFolder, excludeFiles.Split(';'), excludeFileExtensions.Split(';'), excludeDirectories.Split(';'));
        }
    }
}