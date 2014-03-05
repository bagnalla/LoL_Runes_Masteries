using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Text;

public partial class _Default : System.Web.UI.Page
{
    HttpClient client = new HttpClient();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            HttpCookie region = Request.Cookies.Get("region");
            if (region != null)
            {
                foreach (ListItem item in regionRadioList.Items)
                {
                    if (item.Value == region.Value)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }

            HttpCookie showCurrent = Request.Cookies.Get("showCurrent");
            if (showCurrent != null)
            {
                currentCheckBox.Checked = bool.Parse(showCurrent.Value);
            }

            //SummonerDataManager.ClearCache(Server);
            greetingLabel.Style.Add(HtmlTextWriterStyle.Display, "inline");
        }
        else
            greetingLabel.Style.Add(HtmlTextWriterStyle.Display, "none");

        long cacheSize = (SummonerDataManager.CacheSize(Server) / 1024);
        cacheLabel.Text = cacheSize + "kB (up to " + (long)(cacheSize * 1.75) + "kB on disk)";
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        playerInfoLabel.Text = resultsLabel.Text = "";
        //season3StatsButton.Visible = season4StatsButton.Visible = moreStatsLabel.Visible = false;
        //matchHistoryButton.Visible = false;

        if (!Page.IsValid)
            return;

        HttpResponseMessage response = client.GetAsync("https://prod.api.pvp.net/api/lol/" + regionRadioList.SelectedItem.Value.ToLower() + "/v1.2/summoner/by-name/" + nameTxtBox.Text + "?api_key=995d135e-ab6f-4412-bb3c-1bbcd217c252").Result;

        SummonerDto summonerDto;
        bool hasSummonerChanged;

        if (response.StatusCode == HttpStatusCode.OK)
        {
            summonerDto = response.Content.ReadAsAsync<SummonerDto>().Result;

            Header.Title = summonerDto.name + "'s runes/masteries.";

            idHiddenField.Value = summonerDto.id.ToString();
            //idLabel.Text = summonerDto.id.ToString();
            showPlayerInfo(summonerDto);
            //season3StatsButton.Visible = season4StatsButton.Visible = moreStatsLabel.Visible = true;
            //matchHistoryButton.Visible = true;

            hasSummonerChanged = SummonerDataManager.HasSummonerChanged(summonerDto, regionRadioList.SelectedValue, Server);
            SummonerDataManager.WriteSummonerInfoFile(summonerDto, regionRadioList.SelectedValue, Server);
        }
        else
        {
            playerInfoLabel.Text = "";
            if (response.StatusCode.ToString() == "NotFound")
                resultsLabel.Text = "<br/><br/><div style=\"width: 250px; margin: auto\" class=\"errorMessage\">Summoner not found</div>";
            else if (response.StatusCode.ToString() == "429")
                resultsLabel.Text = "<br/><br/><div style=\"width: 250px; margin: auto\" class=\"errorMessage\">Too many requests</div>";
            else
                resultsLabel.Text = "<br/><br/><div style=\"width: 250px; margin: auto\" class=\"errorMessage\">Error: " + response.StatusCode + "</div>";
            return;
        }

        // get rune pages
        RunePagesDto runePages;
        RunePagesDtoManager runePagesManager;
        string currentRunes = "", otherRunes = "";

        if (hasSummonerChanged)
        {
            response = client.GetAsync("https://prod.api.pvp.net/api/lol/" + regionRadioList.SelectedItem.Value.ToLower() + "/v1.2/summoner/" + summonerDto.id + "/runes?api_key=995d135e-ab6f-4412-bb3c-1bbcd217c252").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                runePages = response.Content.ReadAsAsync<RunePagesDto>().Result;
                runePagesManager = new RunePagesDtoManager(runePages);

                //runePages.DoAllCalculations(currentCheckBox.Checked);

                //showRunePages(runePagesManager);
                currentRunes = buildCurrentRunePage(runePagesManager);
                //if (!currentCheckBox.Checked)
                otherRunes = buildOtherRunePages(runePagesManager);

                //SummonerDataManager.WriteSummonerRunesFile(summonerDto, runePagesManager, regionRadioList.SelectedValue, Server);
                SummonerDataManager.WriteRuneTotals(summonerDto, runePagesManager, regionRadioList.SelectedValue, Server);
            }
            else
            {
                //resultsLabel.Text += "<br/><div style=\"width: 250px; margin: auto\">Runes error: " + response.StatusCode + "</div>";
                currentRunes = otherRunes = "<span style=\"color: red\"Runes error: " + response.StatusCode + "</span>";
                SummonerDataManager.ResetSummonerRevisionDate(summonerDto, regionRadioList.SelectedValue, Server);
            }
        }
        else
        {
            if (SummonerDataManager.RunesFileExists(summonerDto, regionRadioList.SelectedValue, Server))
            {
                //showRunePagesFromFile(summonerDto);
                currentRunes = buildCurrentRunePageFromFile(summonerDto);
                //if (!currentCheckBox.Checked)
                otherRunes = buildOtherRunePagesFromFile(summonerDto);
            }
            else
            {
                //resultsLabel.Text += "<br/><div style=\"width: 250px; margin: auto\">Runes error: cache error. just search again</div>";
                //SummonerDataManager.ResetSummonerRevisionDate(summonerDto, regionRadioList.SelectedValue, Server);
                currentRunes = otherRunes = "<span style=\"color: red\">Runes error: cache error. just search again</span>";
            }
        }


        // get mastery pages
        MasteryPagesDto masteryPages;
        string currentMasteries = "", otherMasteries = "";

        if (hasSummonerChanged)
        {
            response = client.GetAsync("https://prod.api.pvp.net/api/lol/" + regionRadioList.SelectedItem.Value.ToLower() + "/v1.2/summoner/" + summonerDto.id + "/masteries?api_key=995d135e-ab6f-4412-bb3c-1bbcd217c252").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                masteryPages = response.Content.ReadAsAsync<MasteryPagesDto>().Result;

                masteryPages.DoAllCalculations();

                //showMasteryPages(masteryPages);
                currentMasteries = buildCurrentMasteryPage(masteryPages);
                //if (!currentCheckBox.Checked)
                otherMasteries = buildOtherMasteryPages(masteryPages);

                SummonerDataManager.WriteSummonerMasteriesFile(summonerDto, masteryPages, regionRadioList.SelectedValue, Server);
            }
            else
            {
                //resultsLabel.Text += "<br/><div style=\"width: 250px; margin: auto\">Masteries error: " + response.StatusCode + "</div>";
                currentMasteries = otherMasteries = "<span style=\"color: red\">Masteries error: " + response.StatusCode + "</span>";
                SummonerDataManager.ResetSummonerRevisionDate(summonerDto, regionRadioList.SelectedValue, Server);
            }
        }
        else
        {
            if (SummonerDataManager.MasteriesFileExists(summonerDto, regionRadioList.SelectedValue, Server))
            {
                masteryPages = SummonerDataManager.ReadSummonerMasteriesFile(summonerDto, regionRadioList.SelectedValue, Server);
                //showMasteryPages(masteryPages);
                currentMasteries = buildCurrentMasteryPage(masteryPages);
                //if (!currentCheckBox.Checked)
                otherMasteries = buildOtherMasteryPages(masteryPages);
            }
            else
            {
                //resultsLabel.Text += "<br/><div style=\"width: 250px; margin: auto\">Masteries error: cache error. just search again</div>";
                currentMasteries = otherMasteries = "<span style=\"color: red\">Masteries error: cache error. just search again</span>";
                SummonerDataManager.ResetSummonerRevisionDate(summonerDto, regionRadioList.SelectedValue, Server);
            }
        }

        showCombinedCurrentTable(currentRunes, currentMasteries);

        //if (!currentCheckBox.Checked)
        showCombinedOtherTable(otherRunes, otherMasteries);

        setCookies();
    }

    void setCookies()
    {
        // region cookie
        HttpCookie region = new HttpCookie("region", regionRadioList.SelectedValue);
        region.Expires = DateTime.Now.AddMonths(3);
        Response.Cookies.Add(region);

        // show current cookie
        HttpCookie showCurrent = new HttpCookie("showCurrent", currentCheckBox.Checked.ToString());
        showCurrent.Expires = DateTime.Now.AddMonths(3);
        Response.Cookies.Add(showCurrent);
    }

    void showPlayerInfo(SummonerDto summoner)
    {
        //playerInfoLabel.Text = summoner.ToString();

        playerInfoLabel.Text = "<div id=\"playerInfoBorder\"><span style=\"margin-bottom: 0px; padding-bottom: 0px; font-size: x-large; font-weight: bold\">" + summoner.name + "</span><br/><span style=\"font-size: 12px; margin-top: 0px; padding-top: 0px\">Level " + summoner.summonerLevel + "</span></div>";
    }

    void showRunePages(RunePagesDtoManager runePagesManager)
    {
        resultsLabel.Text += "<br/><table id=\"Table1\" align=\"Center\" rules=\"cols\" border=\"1\" class=\"ResultsTable\"> <tr> <td align=\"center\" valign=\"middle\" class=\"LeftColumn\">";

        if (runePagesManager.CurrentPage == null || runePagesManager.RunePageTotalsTable(runePagesManager.CurrentPage) == "")
            resultsLabel.Text += "<b>No current rune page</b></td><td valign=\"middle\" class=\"RightColumn\"></td></tr></table>";
        else
            resultsLabel.Text += "<b>Current rune page:</b></td><td valign=\"middle\" class=\"RightColumn\"><div style=\"margin:25px\"><strong>" + runePagesManager.CurrentPage.name
                + "</strong><br/>" + runePagesManager.RunePageTotalsTable(runePagesManager.CurrentPage) + "</div></td></tr></table>";

        if (!currentCheckBox.Checked)
        {
            resultsLabel.Text += "<br/><table id=\"Table1\" align=\"Center\" rules=\"cols\" border=\"1\" class=\"ResultsTable\">";
            resultsLabel.Text += "<tr><td align=\"center\" valign=\"middle\" class=\"LeftColumn\"><b>Other rune pages:</b></td>";
            foreach (RunePageDto page in runePagesManager.runePagesDto.pages)
            {
                if (!page.current)
                {
                    string totalsTable = runePagesManager.RunePageTotalsTable(page);

                    if (totalsTable != "")
                    {
                        resultsLabel.Text += "<td valign=\"middle\" class=\"RightColumn\">";
                        resultsLabel.Text += "<div style=\"margin:25px\"><strong>" + page.name + "</strong><br/>" + totalsTable + "</div>";
                        resultsLabel.Text += "</td></tr>";
                        resultsLabel.Text += "<tr> <td align=\"center\" valign=\"middle\" class=\"LeftColumn\">";
                    }
                }
            }

            resultsLabel.Text += "</td><td/></tr></table>";
        }
    }

    void showRunePagesFromFile(SummonerDto summoner)
    {
        resultsLabel.Text += "<br/><table id=\"Table1\" align=\"Center\" rules=\"cols\" border=\"1\" class=\"ResultsTable\"> <tr> <td align=\"center\" valign=\"middle\" class=\"LeftColumn\">";

        string[] lines = SummonerDataManager.ReadRuneTotals(summoner, regionRadioList.SelectedValue, Server);

        if (lines[1] == "")
            resultsLabel.Text += "<b>No current rune page</b></td><td valign=\"middle\" class=\"RightColumn\"></td></tr></table>";
        else
            resultsLabel.Text += "<b>Current rune page:</b></td><td valign=\"middle\" class=\"RightColumn\"><div style=\"margin:25px\"><strong>" + lines[0]
                + "</strong><br/>" + lines[1] + "</div></td></tr></table>";

        if (!currentCheckBox.Checked)
        {
            resultsLabel.Text += "<br/><table id=\"Table1\" align=\"Center\" rules=\"cols\" border=\"1\" class=\"ResultsTable\">";
            resultsLabel.Text += "<tr><td align=\"center\" valign=\"middle\" class=\"LeftColumn\"><b>Other rune pages:</b></td>";
            for (int i = 2; i < lines.Length; i += 2)
            {
                if (lines[i + 1] != "")
                {
                    resultsLabel.Text += "<td valign=\"middle\" class=\"RightColumn\">";
                    resultsLabel.Text += "<div style=\"margin:25px\"><strong>" + lines[i] + "</strong><br/>" + lines[i + 1] + "</div>";
                    resultsLabel.Text += "</td></tr>";
                    resultsLabel.Text += "<tr> <td align=\"center\" valign=\"middle\" class=\"LeftColumn\">";
                }
            }

            resultsLabel.Text += "</td><td/></tr></table>";
        }
    }

    void showMasteryPages(MasteryPagesDto masteryPages)
    {
        /*if (masteryPages.CurrentPage == null)
            resultsLabel.Text += "<br/>" + "<h3>No current mastery page</h3>";
        else
            resultsLabel.Text += "<br/>" + "<h3>Current mastery page:</h3>" + "<span style=\"font-weight: bold;\">" + masteryPages.CurrentPage.name
                + "</span><br/>" + masteryPages.CurrentPage.AllTalents + "<br/>";

        if (!currentCheckBox.Checked)
        {
            resultsLabel.Text += "<h3>Other mastery pages:</h3>";
            foreach (MasteryPageDto page in masteryPages.pages)
            {
                if (!page.current)
                    resultsLabel.Text += "<span style=\"font-weight: bold;\">" + page.name + "</span><br/>" + page.AllTalents + "<br/>";
            }
        }*/

        resultsLabel.Text += "<br/><table id=\"Table1\" align=\"Center\" rules=\"cols\" border=\"1\" class=\"ResultsTable\"> <tr> <td align=\"center\" valign=\"middle\" class=\"LeftColumn\">";

        if (masteryPages.CurrentPage == null)
            resultsLabel.Text += "<b>No current mastery page</b></td><td valign=\"middle\" class=\"RightColumn\"></td></tr></table>";
        else
            resultsLabel.Text += "<b>Current mastery page:</b></td><td valign=\"middle\" class=\"RightColumn\"><div style=\"margin:25px\">" +
                "<strong>" + masteryPages.CurrentPage.name + "</strong><br/>" +
                "<strong>" + masteryPages.CurrentPage.TreeCounts() + "</strong><br/>" +
                "<span>" + masteryPages.CurrentPage.AllTalentsTable() + "</span></div></td></tr></table>";

        if (!currentCheckBox.Checked)
        {
            resultsLabel.Text += "<br/><table id=\"Table1\" align=\"Center\" rules=\"cols\" border=\"1\" class=\"ResultsTable\"> <tr> <td align=\"center\" valign=\"middle\" class=\"LeftColumn\">";
            resultsLabel.Text += "<b>Other mastery pages:</b></td>";
            foreach (MasteryPageDto page in masteryPages.pages)
            {
                if (!page.current)
                {
                    string allTalentsTable = page.AllTalentsTable();

                    if (allTalentsTable != "")
                    {
                        resultsLabel.Text += "<td valign=\"middle\" class=\"RightColumn\">" + "<div style=\"margin:25px\">" +
                            "<strong>" + page.name + "</strong><br/>" +
                            "<strong>" + page.TreeCounts() + "</strong><br/>" +
                            "<span>" + allTalentsTable + "</span></div>" + "</td></tr>" +
                            "<tr> <td align=\"center\" valign=\"middle\" class=\"LeftColumn\">";
                    }
                }
            }

            resultsLabel.Text += "</td><td/></tr></table>";
        }
    }

    string buildCurrentRunePage(RunePagesDtoManager runePagesManager)
    {
        string str = "";

        if (runePagesManager.CurrentPage == null || runePagesManager.RunePageTotalsTable(runePagesManager.CurrentPage) == "")
        {
            str += "<span class=\"PageName\">none</span><br/>";
        }
        else
        {
            str += "<span class=\"PageName\">" + runePagesManager.CurrentPage.name + "</span><br/><br/>";
            str += "<span class=\"RuneTotals\">" + runePagesManager.RunePageTotalsTable(runePagesManager.CurrentPage) + "</span>";
        }

        return str;
    }

    string buildCurrentRunePageFromFile(SummonerDto summoner)
    {
        string[] lines = SummonerDataManager.ReadRuneTotals(summoner, regionRadioList.SelectedValue, Server);

        string str = "";

        if (lines[1] == "")
        {
            str += "<span class=\"PageName\">none</span><br/>";
        }
        else
        {
            str += "<span class=\"PageName\">" + lines[0] + "</span><br/><br/>";
            str += "<span class=\"RuneTotals\">" + lines[1] + "</span>";
        }

        return str;
    }

    string buildCurrentMasteryPage(MasteryPagesDto masteryPages)
    {
        string str = "";

        if (masteryPages.CurrentPage == null)
        {
            str += "<span class=\"PageName\">none</span><br/>";
        }
        else
        {
            str += "<span class=\"PageName\">" + masteryPages.CurrentPage.name + "</span><br/>";
            str += "<span class=\"PageName\">" + masteryPages.CurrentPage.TreeCounts() + "</span><br/>";
            str += "<span class=\"MasteryTotals\">" + masteryPages.CurrentPage.AllTalentsTable() + "</span>";
        }

        return str;
    }

    string buildOtherRunePages(RunePagesDtoManager runePagesManager)
    {
        StringBuilder str = new StringBuilder();

        foreach (RunePageDto page in runePagesManager.runePagesDto.pages)
        {
            if (!page.current)
            {
                string totalsTable = runePagesManager.RunePageTotalsTable(page);

                if (totalsTable != "")
                {
                    //str += "<span class=\"PageName\">" + page.name + "</span><br/><br/>";
                    str.Append("<span class=\"PageName\">");
                    str.Append(page.name);
                    str.Append("</span><br/><br/>");
                    //str += "<span class=\"RuneTotals\">" + totalsTable + "</span><br/><br/>";
                    str.Append("<span class=\"RuneTotals\">");
                    str.Append(totalsTable);
                    str.Append("</span><br/><br/>");
                }
            }
        }

        if (str.Length > 10)
            //str = str.Substring(0, str.Length - 10);
            str.Remove(str.Length - 10, 10);

        //if (str == "")
        if (str.Length == 0)
            //str = "<span class=\"PageName\">none</span><br/>";
            str.Append("<span class=\"PageName\">none</span><br/>");

        return str.ToString();
    }

    string buildOtherRunePagesFromFile(SummonerDto summoner)
    {
        string[] lines = SummonerDataManager.ReadRuneTotals(summoner, regionRadioList.SelectedValue, Server);

        //string str = "";
        StringBuilder str = new StringBuilder();

        for (int i = 2; i < lines.Length; i += 2)
        {
            if (lines[i + 1] != "")
            {
                //str += "<span class=\"PageName\">" + lines[i] + "</span><br/><br/>";
                str.Append("<span class=\"PageName\">");
                str.Append(lines[i]);
                str.Append("</span><br/><br/>");
                //str += "<span class=\"RuneTotals\">" + lines[i + 1] + "</span><br/><br/>";
                str.Append("<span class=\"RuneTotals\">");
                str.Append(lines[i + 1]);
                str.Append("</span><br/><br/>");
            }
        }

        if (str.Length > 10)
            //str = str.Substring(0, str.Length - 10);
            str.Remove(str.Length - 10, 10);

        if (str.Length == 0)
            //str = "<span class=\"PageName\">none</span><br/>";
            str.Append("<span class=\"PageName\">none</span><br/>");

        return str.ToString();
    }

    string buildOtherMasteryPages(MasteryPagesDto masteryPages)
    {
        //string str = "";
        StringBuilder str = new StringBuilder();

        foreach (MasteryPageDto page in masteryPages.pages)
        {
            if (!page.current)
            {
                string allTalentsTable = page.AllTalentsTable();

                if (allTalentsTable != "")
                {
                    //str += "<span class=\"PageName\">" + page.name + "</span><br/>";
                    str.Append("<span class=\"PageName\">");
                    str.Append(page.name);
                    str.Append("</span><br/>");
                    //str += "<span class=\"PageName\">" + page.TreeCounts() + "</span><br/>";
                    str.Append("<span class=\"PageName\">");
                    str.Append(page.TreeCounts());
                    str.Append("</span><br/>");
                    //str += "<span class=\"MasteryTotals\">" + allTalentsTable + "</span><br/><br/>";
                    str.Append("<span class=\"MasteryTotals\">");
                    str.Append(allTalentsTable);
                    str.Append("</span><br/><br/>");
                }
            }
        }

        if (str.Length > 10)
            //str = str.Substring(0, str.Length - 10);
            str.Remove(str.Length - 10, 10);

        if (str.Length == 0)
            //str = "<span class=\"PageName\">none</span><br/>";
            str.Append("<span class=\"PageName\">none</span><br/>");

        return str.ToString();
    }

    void showCombinedCurrentTable(string runes, string masteries)
    {
        resultsLabel.Text += "<br/><span class=\"TableHeader\">Current</span>" +
            "<br/><table align=\"center\" class=\"CurrentTable\"><tr><td class=\"CurrentTableLeftColumn\"><span class=\"ColumnHeader\">Runes</span><br/>" +
            "</td><td class=\"CurrentTableRightColumn\"><span class=\"ColumnHeader\">Masteries</span><br/>" +
            "</td></tr><tr><td class=\"CurrentTableLeftColumn\"><br/>" + runes +
            "<br/></td><td class=\"CurrentTableRightColumn\"><br/>" + masteries + "<br/></td></tr></table>";
    }

    void showCombinedOtherTable(string runes, string masteries)
    {
        /*resultsLabel.Text += "<br/><span class=\"TableHeader\">Other </span>";

        resultsLabel.Text += "<a onclick=\"showOthersClick();\" id=\"showOthersButton\" href=\"javascript:void(0)\">[show]</a>" +
            "<a onclick=\"hideOthersClick();\" id=\"hideOthersButton\" style=\"display: none\" href=\"javascript:void(0)\">[hide]</a>";


        resultsLabel.Text += "<div id=\"other\"><br/><table align=\"center\" class=\"CurrentTable\"><tr><td class=\"CurrentTableLeftColumn\"><span class=\"ColumnHeader\">Runes</span><br/>" +
            "</td><td class=\"CurrentTableRightColumn\"><span class=\"ColumnHeader\">Masteries</span><br/>" +
            "</td></tr><tr><td class=\"CurrentTableLeftColumn\"><br/>" + runes +
            "<br/></td><td class=\"CurrentTableRightColumn\"><br/>" + masteries + "<br/></td></tr></table></div>";*/

        StringBuilder str = new StringBuilder();

        str.Append("<br/><span class=\"TableHeader\">Other </span>");
        str.Append("<a onclick=\"showOthersClick();\" id=\"showOthersButton\" href=\"javascript:void(0)\">[show]</a>");
        str.Append("<a onclick=\"hideOthersClick();\" id=\"hideOthersButton\" style=\"display: none\" href=\"javascript:void(0)\">[hide]</a>");
        str.Append("<div id=\"other\"><br/><table align=\"center\" class=\"CurrentTable\"><tr><td class=\"CurrentTableLeftColumn\"><span class=\"ColumnHeader\">Runes</span><br/>");
        str.Append("</td><td class=\"CurrentTableRightColumn\"><span class=\"ColumnHeader\">Masteries</span><br/>");
        str.Append("</td></tr><tr><td class=\"CurrentTableLeftColumn\"><br/>");
        str.Append(runes);
        str.Append("<br/></td><td class=\"CurrentTableRightColumn\"><br/>");
        str.Append(masteries);
        str.Append("<br/></td></tr></table></div>");

        resultsLabel.Text += str.ToString();
    }

    protected void moreStatsButton_Click(object sender, EventArgs e)
    {
        resultsLabel.Text = "";

        HttpResponseMessage response = client.GetAsync("https://prod.api.pvp.net/api/lol/" + regionRadioList.SelectedItem.Value.ToLower() + "/v1.2/stats/by-summoner/" + idHiddenField.Value + "/summary?season=SEASON3&api_key=995d135e-ab6f-4412-bb3c-1bbcd217c252").Result;

        PlayerStatsSummaryListDto playerStatsDto = response.Content.ReadAsAsync<PlayerStatsSummaryListDto>().Result;

        //resultsLabel.Text = "<br/><div align=\"center\">";
        resultsLabel.Text = "<br/><div style=\"width: 400px; margin: auto\">";

        foreach (PlayerStatsSummaryDto summary in playerStatsDto.playerStatSummaries)
        {
            resultsLabel.Text += summary.ToString();
        }

        resultsLabel.Text += "</div>";
    }

    protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (nameTxtBox.Text.Length > 2 && nameTxtBox.Text.Length < 17);
    }

    protected void hiddenButton_Click(object sender, EventArgs e)
    {

    }

    /*protected void titleLink_Click(object sender, EventArgs e)
    {
        setCookies();
        Response.Redirect("/");
    }*/
    protected void matchHistoryButton_Click(object sender, EventArgs e)
    {
        resultsLabel.Text = "";

        HttpResponseMessage response = client.GetAsync("https://prod.api.pvp.net/api/lol/" + regionRadioList.SelectedItem.Value.ToLower() + "/v1.3/game/by-summoner/" + idHiddenField.Value + "/recent?season=SEASON3&api_key=995d135e-ab6f-4412-bb3c-1bbcd217c252").Result;

        RecentGamesDto recentGamesDto = response.Content.ReadAsAsync<RecentGamesDto>().Result;

        resultsLabel.Text = "<br/><div style=\"width: 400px; margin: auto\">";

        resultsLabel.Text += recentGamesDto.Summary();

        resultsLabel.Text += "</div>";
    }
}