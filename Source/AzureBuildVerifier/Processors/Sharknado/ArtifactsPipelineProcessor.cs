using System.Collections.Generic;
using AzureBuildVerifier.Utilities;
using Sitecore.Azure.Pipelines.BasePipeline;
using Sitecore.Azure.Pipelines.CreateAzurePackage;
using Sitecore.Diagnostics;

namespace AzureBuildVerifier.Processors.Sharknado
{
    public class ArtifactsPipelineProcessor : CreateAzureDeploymentPipelineProcessor
    {
        public IEnumerable<string> Artifacts { get; private set; }

        protected override void LogCompletedMessage(RolePipelineArgsBase args)
        {
            Assert.ArgumentNotNull(args, "args");
            LogInfoMessage(args, "{0} {1} [done]", new string[0]);
        }

        protected override void Action(RolePipelineArgsBase args)
        {
            var excludeFiles = GetConcatenatedIgnoreList(args.Deployment.FilePathFilter.ExcludeFiles, args.Deployment.FilePathFilter.DeploymentTypeExcludeFiles);
            var excludeDirectories = GetConcatenatedIgnoreList(args.Deployment.FilePathFilter.ExcludeDirectories, args.Deployment.FilePathFilter.DeploymentTypeExcludeDirectories);
            var excludeFileExtensions = GetConcatenatedIgnoreList(args.Deployment.FilePathFilter.ExcludeFileExtensions, args.Deployment.FilePathFilter.DeploymentTypeExcludeFileExtensions);

            var environmentItem = AzureItemUtilities.GetEnvironmentItem(args.Deployment.ID);
            Artifacts = GetArtifacts(environmentItem.BuildFolder, excludeFiles.Split(';'), excludeFileExtensions.Split(';'), excludeDirectories.Split(';'));

            args.CustomData["artifacts"] = Artifacts;
        }

        private string GetConcatenatedIgnoreList(string list1, string list2)
        {
            if (string.IsNullOrEmpty(list1) && string.IsNullOrEmpty(list2)) return string.Empty;
            if (!string.IsNullOrEmpty(list1) && string.IsNullOrEmpty(list2)) return list1;
            if (string.IsNullOrEmpty(list1) && !string.IsNullOrEmpty(list2)) return list2;

            bool hasSeparator = list1.EndsWith(";") || list2.StartsWith(";");
            return hasSeparator ? string.Concat(list1, list2) : string.Concat(list1, ";", list2);
        }
    }
}