Sitecore Azure Build Verifier
==================

Sitecore Azure Build Verifier executes a test run of an Azure build for deployment, verifying the filename and path length against known limitations. It assists in identifying the files and folders which generate the following error when deploying:

_"The specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters"_

##Installation

1. Install via [NuGet](http://www.nuget.org/packages/AzureBuildVerifier/1.0.0)
2. Install the package ```/data/packages/Azure Build Verifier.zip```

##Verifying a Build

Verifying builds is done via a context command through the content editor. Follow the steps below to help identify files/paths which are too deeply nested for a successful deploy. 

1. Log into the Sitecore Client
2. Open the content editor
3. Navigate to an ```Azzure Deployment``` item under ```/sitecore/system/Modules/Azure```
4. Right-click and select _Verify Deploment_
