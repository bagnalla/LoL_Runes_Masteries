using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class PlayerDto
{
    public int championId;
    public long summonerId;
    public int teamId;

    public string Summary()
    {
        return "champion id: " + championId + "<br/>" +
            "summoner id: " + summonerId + "<br/>" +
            "team id: " + teamId;
    }
}