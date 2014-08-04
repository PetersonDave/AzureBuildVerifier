using System;
using System.Web;
using AzureBuildVerifier;

namespace Website.Modifications
{
    public partial class AzureBuildVerifierSummary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var deploymentId = Request.Params["id"];
            var settings = new AzureDeploymentContext(deploymentId);
            var verifier = new Verifier(settings);

            var paths = verifier.GetInvalidPaths();
            foreach (var path in paths)
            {
                HttpContext.Current.Response.Write(path + "<br />");
            }
        }
    }
}