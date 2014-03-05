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
            showPlayerInfo(summonerDto);

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
                
                currentRunes = buildCurrentRunePage(runePagesManager);
                otherRunes = buildOtherRunePages(runePagesManager);

                SummonerDataManager.WriteRuneTotals(summonerDto, runePagesManager, regionRadioList.SelectedValue, Server);
            }
            else
            {
                currentRunes = otherRunes = "<span style=\"color: red\"Runes error: " + response.StatusCode + "</span>";
                SummonerDataManager.ResetSummonerRevisionDate(summonerDto, regionRadioList.SelectedValue, Server);
            }
        }
        else
        {
            if (SummonerDataManager.RunesFileExists(summonerDto, regionRadioList.SelectedValue, Server))
            {
                currentRunes = buildCurrentRunePageFromFile(summonerDto);
                otherRunes = buildOtherRunePagesFromFile(summonerDto);
            }
            else
            {
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

                currentMasteries = buildCurrentMasteryPage(masteryPages);
                otherMasteries = buildOtherMasteryPages(masteryPages);

                SummonerDataManager.WriteSummonerMasteriesFile(summonerDto, masteryPages, regionRadioList.SelectedValue, Server);
            }
            else
            {
                currentMasteries = otherMasteries = "<span style=\"color: red\">Masteries error: " + response.StatusCode + "</span>";
                SummonerDataManager.ResetSummonerRevisionDate(summonerDto, regionRadioList.SelectedValue, Server);
            }
        }
        else
        {
            if (SummonerDataManager.MasteriesFileExists(summonerDto, regionRadioList.SelectedValue, Server))
            {
                masteryPages = SummonerDataManager.ReadSummonerMasteriesFile(summonerDto, regionRadioList.SelectedValue, Server);
                currentMasteries = buildCurrentMasteryPage(masteryPages);
                otherMasteries = buildOtherMasteryPages(masteryPages);
            }
            else
            {
                currentMasteries = otherMasteries = "<span style=\"color: red\">Masteries error: cache error. just search again</span>";
                SummonerDataManager.ResetSummonerRevisionDate(summonerDto, regionRadioList.SelectedValue, Server);
            }
        }

        showCombinedCurrentTable(currentRunes, currentMasteries);

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
        playerInfoLabel.Text = "<div id=\"playerInfoBorder\"><span style=\"margin-bottom: 0px; padding-bottom: 0px; font-size: x-large; font-weight: bold\">" + summoner.name + "</span><br/><span style=\"font-size: 12px; margin-top: 0px; padding-top: 0px\">Level " + summoner.summonerLevel + "</span></div>";
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
        StringBuilder sb = new StringBuilder();

        foreach (RunePageDto page in runePagesManager.runePagesDto.pages)
        {
            if (!page.current)
            {
                string totalsTable = runePagesManager.RunePageTotalsTable(page);

                if (totalsTable != "")
                {
                    sb.Append("<span class=\"PageName\">");
                    sb.Append(page.name);
                    sb.Append("</span><br/><br/>");

                    sb.Append("<span class=\"RuneTotals\">");
                    sb.Append(totalsTable);
                    sb.Append("</span><br/><br/>");
                }
            }
        }

        if (sb.Length > 10)
            sb.Remove(sb.Length - 10, 10);

        if (sb.Length == 0)
            sb.Append("<span class=\"PageName\">none</span><br/>");

        return sb.ToString();
    }

    string buildOtherRunePagesFromFile(SummonerDto summoner)
    {
        string[] lines = SummonerDataManager.ReadRuneTotals(summoner, regionRadioList.SelectedValue, Server);

        StringBuilder sb = new StringBuilder();

        for (int i = 2; i < lines.Length; i += 2)
        {
            if (lines[i + 1] != "")
            {
                sb.Append("<span class=\"PageName\">");
                sb.Append(lines[i]);
                sb.Append("</span><br/><br/>");

                sb.Append("<span class=\"RuneTotals\">");
                sb.Append(lines[i + 1]);
                sb.Append("</span><br/><br/>");
            }
        }

        if (sb.Length > 10)
            sb.Remove(sb.Length - 10, 10);

        if (sb.Length == 0)
            sb.Append("<span class=\"PageName\">none</span><br/>");

        return sb.ToString();
    }

    string buildOtherMasteryPages(MasteryPagesDto masteryPages)
    {
        StringBuilder sb = new StringBuilder();

        foreach (MasteryPageDto page in masteryPages.pages)
        {
            if (!page.current)
            {
                string allTalentsTable = page.AllTalentsTable();

                if (allTalentsTable != "")
                {
                    sb.Append("<span class=\"PageName\">");
                    sb.Append(page.name);
                    sb.Append("</span><br/>");

                    sb.Append("<span class=\"PageName\">");
                    sb.Append(page.TreeCounts());
                    sb.Append("</span><br/>");

                    sb.Append("<span class=\"MasteryTotals\">");
                    sb.Append(allTalentsTable);
                    sb.Append("</span><br/><br/>");
                }
            }
        }

        if (sb.Length > 10)
            sb.Remove(sb.Length - 10, 10);

        if (sb.Length == 0)
            sb.Append("<span class=\"PageName\">none</span><br/>");

        return sb.ToString();
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

        StringBuilder sb = new StringBuilder();

        sb.Append("<br/><span class=\"TableHeader\">Other </span>");
        sb.Append("<a onclick=\"showOthersClick();\" id=\"showOthersButton\" href=\"javascript:void(0)\">[show]</a>");
        sb.Append("<a onclick=\"hideOthersClick();\" id=\"hideOthersButton\" style=\"display: none\" href=\"javascript:void(0)\">[hide]</a>");
        sb.Append("<div id=\"other\"><br/><table align=\"center\" class=\"CurrentTable\"><tr><td class=\"CurrentTableLeftColumn\"><span class=\"ColumnHeader\">Runes</span><br/>");
        sb.Append("</td><td class=\"CurrentTableRightColumn\"><span class=\"ColumnHeader\">Masteries</span><br/>");
        sb.Append("</td></tr><tr><td class=\"CurrentTableLeftColumn\"><br/>");
        sb.Append(runes);
        sb.Append("<br/></td><td class=\"CurrentTableRightColumn\"><br/>");
        sb.Append(masteries);
        sb.Append("<br/></td></tr></table></div>");

        resultsLabel.Text += sb.ToString();
    }

    protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (nameTxtBox.Text.Length > 2 && nameTxtBox.Text.Length < 17);
    }
}