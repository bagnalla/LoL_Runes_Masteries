using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PlayerStatsSummaryListDto
/// </summary>
public class PlayerStatsSummaryListDto
{
    public List<PlayerStatsSummaryDto> playerStatSummaries;
    public long summonerId;

    public PlayerStatsSummaryListDto()
    {
    }
}