using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SummonerDto
/// </summary>
public class SummonerDto
{
    public long id;
    public string name;
    public int profileIconId;
    public long revisionDate;
    public long summonerLevel;

    public SummonerDto(long id, string name, int profileIconId, long revisionDate, long summonerLevel)
	{
        this.id = id;
        this.name = name;
        this.profileIconId = profileIconId;
        this.revisionDate = revisionDate;
        this.summonerLevel = summonerLevel;
	}

    public override string ToString()
    {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        epoch = epoch.AddMilliseconds(revisionDate);

        string str = "Name: " + name + "<br/>"
            + "Level: " + summonerLevel + "<br/>"
            //+ "icon id: " + profileIconId + "<br/>"
            //+ "revision date: " + epoch + "<br/>"
            + "ID: " + id;

        return str;
    }
}