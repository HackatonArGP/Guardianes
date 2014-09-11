<%@ Page Language="C#" AutoEventWireup="true" Async="true" CodeBehind="Test.aspx.cs" Inherits="Earthwatchers.Services.Test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="hexCode" runat="server" Width="100" />
        <asp:Button Text="Testear" ID="testButton" runat="server" OnClick="testButton_Click" />
    </div>
        <div id="contentDiv" runat="server">

        </div>
    </form>
</body>
</html>
