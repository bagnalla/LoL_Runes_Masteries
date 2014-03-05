using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class RecentGamesDto
{
    public HashSet<GameDto> games;
    public long summonerId;

    public string Summary()
    {
        string str = "summoner id: " + summonerId + "<br/><br/>";

        foreach (GameDto game in games)
        {
            str += "<h1>game:</h1><br/><br/>" + game.Summary() + "<br/><br/>";
        }

        return str;
    }
}