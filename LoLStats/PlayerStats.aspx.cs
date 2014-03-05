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

public partial class PlayerStats : System.Web.UI.Page
{
    HttpClient client = new HttpClient();

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        if (nameTxtBox.Text == "")
        {
            resultsLabel.Text = "";
            return;
        }

        HttpResponseMessage response = client.GetAsync("https://prod.api.pvp.net/api/lol/{region}/v1.2/stats/by-summoner/{summonerId}/summary" + nameTxtBox.Text + "?api_key=995d135e-ab6f-4412-bb3c-1bbcd217c252").Result;
    }
}