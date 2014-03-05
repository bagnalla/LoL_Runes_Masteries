using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class GameDto
{
    public int championId;
    public long createDate;
    public List<PlayerDto> fellowPlayers;
    public long gameId;
    public string gameMode;
    public string gameType;
    public bool invalid;
    public int level;
    public int mapId;
    public int spell1;
    public int spell2;
    public RawStatsDto stats;
    public string subType;
    public int teamId;

    public string Summary()
    {
        string str = "";

        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        epoch = epoch.AddMilliseconds(createDate);

        str += "championId: " + championId + "<br/>" +
            "createDate: " + epoch + "<br/><br/>" +
            "players:<br/>";

        foreach (PlayerDto player in fellowPlayers)
            str += player.Summary() + "<br/>";

        str += "<br/>" + "gameId: " + gameId + "<br/>" +
            "gameMode: " + gameMode + "<br/>" +
            "gameType: " + gameType + "<br/>" +
            "invalid: " + invalid + "<br/>" +
            "level: " + level + "<br/>" +
            "mapId: " + mapId + "<br/>" +
            "spell1: " + spell1 + "<br/>" +
            "spell2: " + spell2 + "<br/><br/>" +
            "raw stats:<br/>" + stats.Summary() + "<br/>" +
            "subType: " + subType + "<br/>" +
            "teamId: " + teamId;

        return str;
    }
}