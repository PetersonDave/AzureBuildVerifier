using System.Collections.Generic;
using System.IO;

namespace AzureBuildVerifier.Processors
{
    public class Processor
    {
        private const int MaxCharsFilename = 260;
        private const int MaxCharsPath = 248;

        private static string _deploymentPath;
        private static string _sourcePath;

        public List<string> InvalidPaths { get; private set; }

        public Processor(string deploymentPath, string sourcePath)
        {
            _deploymentPath = deploymentPath;
            _sourcePath = sourcePath;

            InvalidPaths = new List<string>();
        }

        public void ProcessDirectory(string targetDirectory)
        {
            var newPath = targetDirectory.Replace(_sourcePath, _deploymentPath);

            if (newPath.Length >= MaxCharsPath)
            {
                InvalidPaths.Add(newPath);
            }

            ProcessFilesInDirectory(targetDirectory);
            ProcessSubdirectories(targetDirectory);
        }

        private void ProcessSubdirectories(string targetDirectory)
        {
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                ProcessDirectory(subdirectory);
            }
        }

        private void ProcessFilesInDirectory(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName);
            }
        }

        public void ProcessFile(string filenamePath)
        {
            var newPath = filenamePath.Replace(_sourcePath, _deploymentPath);
            if (newPath.Length >= MaxCharsFilename)
            {
                InvalidPaths.Add(newPath);
            }
        }
    }
}