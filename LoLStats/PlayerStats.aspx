<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PlayerStats.aspx.cs" Inherits="PlayerStats" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Player Stats</title>
    <link href="StyleSheet.css" rel="stylesheet" type="text/css" />
</head>
<body style="background-color: #D7D7D7">
    <form id="form1" runat="server">
        <div id="userInput">
            <asp:TextBox ID="nameTxtBox" runat="server"></asp:TextBox>
            <asp:Button ID="Button1" runat="server" CssClass="buttonStyle" OnClick="Button1_Click" Text="Submit" BackColor="#999999" BorderStyle="Solid" Font-Size="Large" />
        </div>

        <div>
            <asp:Label ID="resultsLabel" runat="server" CssClass="resultsStyle"></asp:Label>
        </div>
    </form>
</body>
</html>
