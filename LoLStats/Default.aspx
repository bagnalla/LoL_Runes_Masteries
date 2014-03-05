<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Summoner Runes/Masteries</title>
    <link href="StyleSheet.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        function containsSpecialCharacters(str) {
            var iChars = "!@#$%^&*()+=-[]\\\';,./{}|\":<>?";
            for (var i = 0; i < str.length; i++) {
                if (iChars.indexOf(str.charAt(i)) != -1)
                    return true;
            }
        }
    </script>

    <script type="text/javascript">
        function nameTxtBoxValidator(source, args) {
            hideGreetingLabel();

            var nameTxtBox = document.getElementById('<%= nameTxtBox.ClientID %>');

            if (nameTxtBox.value.length < 3) {
                source.textContent = "not long enough";
                args.IsValid = false;
            }
            else if (nameTxtBox.value.length > 16) {
                source.textContent = "too long";
                args.IsValid = false;
            }
            else if (containsSpecialCharacters(nameTxtBox.value)) {
                source.textContent = "illegal characters";
                args.IsValid = false;
            }
            else
                submitClick();
        }
    </script>

</head>
<body>

    <!--<div id="leftImageContainer"></div>
    <div style="position: relative; min-width: 286px">
        <img src="dragonright.png" style="position: fixed; right: 0; top: 0" />
    </div>-->

    <div id="container">
        <div id="header">
            <br />

            <header>
                <a href="/"></a>
            </header>
        </div>
        <div id="body">

            <form id="form1" runat="server">

                <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

                <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="False" UpdateMode="Conditional">
                    <ContentTemplate>

                        <div id="userInput">
                            <asp:CustomValidator ID="CustomValidator1" runat="server" ClientValidationFunction="nameTxtBoxValidator" CssClass="errorMessage" Display="Dynamic" ErrorMessage="CustomValidator" OnServerValidate="CustomValidator1_ServerValidate" Font-Size="Small">*</asp:CustomValidator>

                            <asp:Label ID="greetingLabel" runat="server" Font-Size="Small" Text="Enter a summoner name"></asp:Label>

                            <br />

                            <asp:TextBox ID="nameTxtBox" runat="server" Width="160px"></asp:TextBox>
                            <asp:Button ID="submitButton" runat="server" CssClass="buttonStyle" OnClick="Button1_Click" Text="Submit" BackColor="#999999" BorderStyle="Solid" Font-Size="Large" />
                            <br />
                            <asp:RadioButtonList ID="regionRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                <asp:ListItem Selected="True">NA</asp:ListItem>
                                <asp:ListItem>EUW</asp:ListItem>
                                <asp:ListItem>EUNE</asp:ListItem>
                            </asp:RadioButtonList>
                            <br />
                            <asp:CheckBox ID="currentCheckBox" runat="server" Text="only current runes/masteries" Checked="True" Visible="False" />

                            <br />
                            &nbsp;<br />
                            &nbsp;
                        </div>

                        <div id="resultsDiv">
                            <div id="playerInfoDiv">
                                <asp:Label ID="playerInfoLabel" runat="server"></asp:Label>
                                <!--
                        <br />
                        <asp:Label ID="moreStatsLabel" runat="server" Text="more stats: " Visible="False"></asp:Label>
                        <asp:LinkButton ID="season3StatsButton" runat="server" OnClick="moreStatsButton_Click" Visible="False" OnClientClick="moreStatsClick()">season 3</asp:LinkButton>
                        <asp:LinkButton ID="season4StatsButton" runat="server" Enabled="False" Visible="False">season 4</asp:LinkButton>
                        <br />
                        <asp:LinkButton ID="matchHistoryButton" runat="server" OnClick="matchHistoryButton_Click" Visible="False">match history</asp:LinkButton>
                            -->
                            </div>

                            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="0" EnableViewState="False">
                                <ProgressTemplate>
                                    <div class="PleaseWait">
                                        <br />
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>

                            <asp:Label ID="resultsLabel" runat="server" CssClass="resultsStyle" EnableViewState="False"></asp:Label>

                            <br />

                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="submitButton" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="season3StatsButton" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>

                <br />
                <asp:HiddenField ID="idHiddenField" runat="server" Visible="False" />
                <asp:Label ID="cacheLabel" Style="display: none" runat="server" EnableViewState="False"></asp:Label>
                <asp:Button ID="hiddenButton" runat="server" Style="display: none;" OnClick="hiddenButton_Click" />

            </form>
        </div>
        <div id="footer">
            <footer>
                This site is not endorsed, certified or otherwise approved in any way by Riot Games, Inc. or any of its affiliates.
            </footer>
        </div>
    </div>

    <script type="text/javascript">
        function submitClick() {
            document.getElementById("playerInfoDiv").innerHTML = "";
            document.getElementById("resultsLabel").innerHTML = "";

        }
    </script>

    <script type="text/javascript">
        function showOthersClick() {
            document.getElementById("showOthersButton").style.display = "none";
            document.getElementById("hideOthersButton").style.display = "inline";
            document.getElementById("other").style.display = "inline";
        }
    </script>

    <script type="text/javascript">
        function hideOthersClick() {
            document.getElementById("hideOthersButton").style.display = "none";
            document.getElementById("showOthersButton").style.display = "inline";
            document.getElementById("other").style.display = "none";
        }
    </script>

    <script type="text/javascript">
        function hideGreetingLabel() {
            document.getElementById("<%= greetingLabel.ClientID %>").style.display = "none";
        }
    </script>
</body>

</html>
