<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestReport.aspx.cs" Inherits="FOS.Web.UI.TestReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" SizeToReportContent="true">
            
             <LocalReport ReportPath="TestReport.rdlc">
                   
               </LocalReport>
        </rsweb:ReportViewer> 
    <%--<div class="control-group" style="margin-bottom:0px">
                                    <div class="form-actions"  style="margin-left: 24%;">
                                     
                                   
        <asp:button id="print" OnClick="print_Click" runat="server" CssClass="btn btn-Primary" value="Print" Text="Print">
            
        </asp:button>
</div>
            </div>--%>
</div>
    </form>
</body>
</html>
