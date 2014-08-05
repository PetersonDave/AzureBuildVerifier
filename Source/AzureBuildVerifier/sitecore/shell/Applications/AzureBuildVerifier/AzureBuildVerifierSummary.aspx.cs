using System;
using System.Linq;
using AzureBuildVerifier;

namespace AzureBuildVerifier.sitecore.shell.Applications.AzureBuildVerifier
{
    public partial class AzureBuildVerifierSummary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var deploymentId = Request.Params["id"];
            var settings = new AzureDeploymentContext(deploymentId);
            var verifier = new Verifier(settings);

            var paths = verifier.GetInvalidPaths();
            var pathsArray = paths as string[] ?? paths.ToArray();
            if (pathsArray.Any())
            {
                var output = string.Empty;
                foreach (var path in pathsArray)
                {
                    output += string.Format("<strong>[{0}]</strong> {1} <br/>", path.Length, path);
                }

                divTitle.Attributes["class"] = "failure";
                lblResults.Text = output;
                lblTitle.Text = "Failed";
                lblSubTitle.Text = "Invalid paths:";
            }
            else
            {
                divTitle.Attributes["class"] = "success";
                lblResults.Text = string.Empty;
                lblTitle.Text = "Passed";
                lblSubTitle.Text = string.Empty;
            }
        }
    }
}