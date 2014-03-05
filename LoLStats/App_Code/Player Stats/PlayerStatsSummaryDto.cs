using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PlayerStatsSummaryDto
/// </summary>
public class PlayerStatsSummaryDto
{
    public AggregatedStatsDto aggregatedStats;
    public int losses;
    public long modifyDate;
    public string playerStatSummaryType;
    public int wins;

    public override string ToString()
    {
        string str = "";
        string stats = aggregatedStats.AllStats;

        if (stats != "")
            str = "<span style=\"font-weight: bold\">" + playerStatSummaryType + "</span><br/>" + stats + "<br/>";

        return str;
    }
}