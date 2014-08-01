using System;

namespace AzureBuildVerifier.Exceptions
{
    public class VerifierException : Exception
    {
        public VerifierException(string message) : base(message) { }
    }
}
