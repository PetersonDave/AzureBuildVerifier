using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AzureBuildVerifier
{
    public class ArtifactsAnalyzer
    {
        private const int MaxCharsFilename = 260;
        private const int MaxCharsPath = 248;

        private readonly IEnumerable<string> _artifacts;
        private readonly string _sourcePath;
        private readonly string _destinationPath;

        public List<string> InvalidPaths { get; private set; }

        public ArtifactsAnalyzer(IEnumerable<string> artifacts, string sourcePath, string destinationPath)
        {
            _artifacts = artifacts;
            _sourcePath = sourcePath;
            _destinationPath = destinationPath;

            InvalidPaths = new List<string>();
            ProcessArtifacts();
        }

        private void ProcessArtifacts()
        {
            if (!_artifacts.Any()) return;

            foreach (var artifact in _artifacts)
            {
                var newPath = artifact.Replace(_sourcePath, _destinationPath);
                bool isFile = Path.HasExtension(newPath);
                bool isInvalid = isFile ? newPath.Length >= MaxCharsFilename : newPath.Length >= MaxCharsPath;
                if (isInvalid)
                {
                    InvalidPaths.Add(newPath);
                }
            }
        }
    }
}