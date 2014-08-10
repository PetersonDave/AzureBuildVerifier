<%@ Page Language="C#" AutoEventWireup="true" Inherits="AzureBuildVerifier.UI.AzureBuildVerifierSummary" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        body { 
             font: 81.25%/1.333333333333333 Arial, Helvetica, sans-serif; 
             overflow: scroll;
         }

        div {
            width: 2000px;
        }

        div.success {
            font-size: 1.415em;
            font-weight: bold;
            color: green;
        }

        div.failure {
            font-size: 1.415em;
            font-weight: bold;
            color: red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    
    <div id="divTitle" runat="server"><asp:Label runat="server" ID="lblTitle"></asp:Label></div>
    <h3><asp:label ID="lblSubTitle" runat="server"></asp:label></h3>
    <div>
        <asp:label ID="lblResults" runat="server"></asp:label>
    </div>
    </form>
</body>
</html>