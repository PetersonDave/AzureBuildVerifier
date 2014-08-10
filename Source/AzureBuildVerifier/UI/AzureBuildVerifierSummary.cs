using System;
using System.Linq;

namespace AzureBuildVerifier.UI
{
    public class AzureBuildVerifierSummary : System.Web.UI.Page
    {
        protected System.Web.UI.HtmlControls.HtmlForm form1;
        protected System.Web.UI.WebControls.Label lblResults;
        protected System.Web.UI.WebControls.Label lblTitle;
        protected System.Web.UI.WebControls.Label lblSubTitle;
        protected System.Web.UI.HtmlControls.HtmlGenericControl divTitle;

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
