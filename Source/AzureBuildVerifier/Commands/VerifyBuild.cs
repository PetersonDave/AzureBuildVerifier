using System.Collections.Specialized;
using System.Linq;
using Sitecore.Azure.Deployments.AzureDeployments;
using Sitecore.Shell.Framework.Commands;

namespace AzureBuildVerifier.Commands
{
    internal class VerifyBuild : Command
    {
        private const string DialogUrl = "/sitecore/shell/Applications/AzureBuildVerifier/AzureBuildVerifierSummary.aspx";

        public override void Execute(CommandContext context)
        {
            if (context.Items.Length != 1) return;

            var item = context.Items[0];
            var parameters = new NameValueCollection();
            parameters["id"] = item.ID.ToString();

            Sitecore.Context.ClientPage.Start(this, "Run", parameters);
        }

        protected void Run(Sitecore.Web.UI.Sheer.ClientPipelineArgs args)
        {
            var url = new Sitecore.Text.UrlString(DialogUrl);
            url.Append("id", args.Parameters["id"]);

            Sitecore.Context.ClientPage.ClientResponse.ShowModalDialog(url.ToString(), "800", "450");
        }

        public override CommandState QueryState(CommandContext context)
        {
            bool isTemplateTypeDeployment = context.Items.Any(item => item.TemplateID == AzureDeploymentItem.TemplateID);
            return isTemplateTypeDeployment ? base.QueryState(context) : CommandState.Hidden;
        }
    }
}