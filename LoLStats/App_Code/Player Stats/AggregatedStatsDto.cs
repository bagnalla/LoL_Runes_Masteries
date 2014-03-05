using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AggregatedStatsDto
/// </summary>
public class AggregatedStatsDto
{
    public int averageAssists,
        averageChampionsKilled,
        averageCombatPlayerScore,
        averageNodeCapture,
        averageNodeCaptureAssist,
        averageNodeNeutralize,
        averageNodeNeutralizeAssist,
        averageNumDeaths,
        averageObjectivePlayerScore,
        averageTeamObjective,
        averageTotalPlayerScore,
        botGamesPlayed,
        killingSpree,
        maxAssists,
        maxChampionsKilled,
        maxCombatPlayerScore,
        maxLargestCriticalStrike,
        maxLargestKillingSpree,
        maxNodeCapture,
        maxNodeCaptureAssist,
        maxNodeNeutralize,
        maxNodeNeutralizeAssist,
        maxObjectivePlayerScore,
        maxTeamObjective,
        maxTimePlayed,
        maxTimeSpentLiving,
        maxTotalPlayerScore,
        mostChampionKillsPerSession,
        mostSpellsCast,
        normalGamesPlayed,
        rankedPremadeGamesPlayed,
        rankedSoloGamesPlayed,
        totalAssists,
        totalChampionKills,
        totalDamageDealt,
        totalDamageTaken,
        totalDoubleKills,
        totalFirstBlood,
        totalGoldEarned,
        totalHeal,
        totalMagicDamageDealt,
        totalMinionKills,
        totalNeutralMinionsKilled,
        totalNodeCapture,
        totalNodeNeutralize,
        totalPentaKills,
        totalPhysicalDamageDealt,
        totalQuadraKills,
        totalSessionsLost,
        totalSessionsPlayed,
        totalSessionsWon,
        totalTripleKills,
        totalTurretsKilled,
        totalUnrealKills;

    public AggregatedStatsDto()
    {
    }

    public string AllStats
    {
        get
        {
            string str = "";

            if (averageAssists != 0)
                str += "averageAssists: " + averageAssists + "<br/>";
            if (averageChampionsKilled != 0)
                str += "averageChampionsKilled: " + averageChampionsKilled + "<br/>";
            if (averageCombatPlayerScore != 0)
                str += "averageCombatPlayerScore: " + averageCombatPlayerScore + "<br/>";
            if (averageNodeCapture != 0)
                str += "averageNodeCapture: " + averageNodeCapture + "<br/>";
            if (averageNodeCaptureAssist != 0)
                str += "averageNodeCaptureAssist: " + averageNodeCaptureAssist + "<br/>";
            if (averageNodeNeutralize != 0)
                str += "averageNodeNeutralize: " + averageNodeNeutralize + "<br/>";
            if (averageNodeNeutralizeAssist != 0)
                str += "averageNodeNeutralizeAssist: " + averageNodeNeutralizeAssist + "<br/>";
            if (averageNumDeaths != 0)
                str += "averageNumDeaths: " + averageNumDeaths + "<br/>";
            if (averageObjectivePlayerScore != 0)
                str += "averageObjectivePlayerScore: " + averageObjectivePlayerScore + "<br/>";
            if (averageTeamObjective != 0)
                str += "averageTeamObjective: " + averageTeamObjective + "<br/>";
            if (averageTotalPlayerScore != 0)
                str += "averageTotalPlayerScore: " + averageTotalPlayerScore + "<br/>";
            if (botGamesPlayed != 0)
                str += "botGamesPlayed: " + botGamesPlayed + "<br/>";
            if (killingSpree != 0)
                str += "killingSpree: " + killingSpree + "<br/>";
            if (maxAssists != 0)
                str += "maxAssists: " + maxAssists + "<br/>";
            if (maxChampionsKilled != 0)
                str += "maxChampionsKilled: " + maxChampionsKilled + "<br/>";
            if (maxCombatPlayerScore != 0)
                str += "maxCombatPlayerScore: " + maxCombatPlayerScore + "<br/>";
            if (maxLargestCriticalStrike != 0)
                str += "maxLargestCriticalStrike: " + maxLargestCriticalStrike + "<br/>";
            if (maxLargestKillingSpree != 0)
                str += "maxLargestKillingSpree: " + maxLargestKillingSpree + "<br/>";
            if (maxNodeCapture != 0)
                str += "maxNodeCapture: " + maxNodeCapture + "<br/>";
            if (maxNodeCaptureAssist != 0)
                str += "maxNodeCaptureAssist: " + maxNodeCaptureAssist + "<br/>";
            if (maxNodeNeutralize != 0)
                str += "maxNodeNeutralize: " + maxNodeNeutralize + "<br/>";
            if (maxNodeNeutralizeAssist != 0)
                str += "maxNodeNeutralizeAssist: " + maxNodeNeutralizeAssist + "<br/>";
            if (maxObjectivePlayerScore != 0)
                str += "maxObjectivePlayerScore: " + maxObjectivePlayerScore + "<br/>";
            if (maxTeamObjective != 0)
                str += "maxTeamObjective: " + maxTeamObjective + "<br/>";
            if (maxTimePlayed != 0)
                str += "maxTimePlayed: " + maxTimePlayed + "<br/>";
            if (maxTimeSpentLiving != 0)
                str += "maxTimeSpentLiving: " + maxTimeSpentLiving + "<br/>";
            if (maxTotalPlayerScore != 0)
                str += "maxTotalPlayerScore: " + maxTotalPlayerScore + "<br/>";
            if (mostChampionKillsPerSession != 0)
                str += "mostChampionKillsPerSession: " + mostChampionKillsPerSession + "<br/>";
            if (mostSpellsCast != 0)
                str += "mostSpellsCast: " + mostSpellsCast + "<br/>";
            if (normalGamesPlayed != 0)
                str += "normalGamesPlayed: " + normalGamesPlayed + "<br/>";
            if (rankedPremadeGamesPlayed != 0)
                str += "rankedPremadeGamesPlayed: " + rankedPremadeGamesPlayed + "<br/>";
            if (rankedSoloGamesPlayed != 0)
                str += "rankedSoloGamesPlayed: " + rankedSoloGamesPlayed + "<br/>";
            if (totalAssists != 0)
                str += "totalAssists: " + totalAssists + "<br/>";
            if (totalChampionKills != 0)
                str += "totalChampionKills: " + totalChampionKills + "<br/>";
            if (totalDamageDealt != 0)
                str += "totalDamageDealt: " + totalDamageDealt + "<br/>";
            if (totalDamageTaken != 0)
                str += "totalDamageTaken: " + totalDamageTaken + "<br/>";
            if (totalDoubleKills != 0)
                str += "totalDoubleKills: " + totalDoubleKills + "<br/>";
            if (totalFirstBlood != 0)
                str += "totalFirstBlood: " + totalFirstBlood + "<br/>";
            if (totalGoldEarned != 0)
                str += "totalGoldEarned: " + totalGoldEarned + "<br/>";
            if (totalHeal != 0)
                str += "totalHeal: " + totalHeal + "<br/>";
            if (totalMagicDamageDealt != 0)
                str += "totalMagicDamageDealt: " + totalMagicDamageDealt + "<br/>";
            if (totalMinionKills != 0)
                str += "totalMinionKills: " + totalMinionKills + "<br/>";
            if (totalNeutralMinionsKilled != 0)
                str += "totalNeutralMinionsKilled: " + totalNeutralMinionsKilled + "<br/>";
            if (totalNodeCapture != 0)
                str += "totalNodeCapture: " + totalNodeCapture + "<br/>";
            if (totalNodeNeutralize != 0)
                str += "totalNodeNeutralize: " + totalNodeNeutralize + "<br/>";
            if (totalPentaKills != 0)
                str += "totalPentaKills: " + totalPentaKills + "<br/>";
            if (totalPhysicalDamageDealt != 0)
                str += "totalPhysicalDamageDealt: " + totalPhysicalDamageDealt + "<br/>";
            if (totalQuadraKills != 0)
                str += "totalQuadraKills: " + totalQuadraKills + "<br/>";
            if (totalSessionsLost != 0)
                str += "totalSessionsLost: " + totalSessionsLost + "<br/>";
            if (totalSessionsPlayed != 0)
                str += "totalSessionsPlayed: " + totalSessionsPlayed + "<br/>";
            if (totalSessionsWon != 0)
                str += "totalSessionsWon: " + totalSessionsWon + "<br/>";
            if (totalTripleKills != 0)
                str += "totalTripleKills: " + totalTripleKills + "<br/>";
            if (totalTurretsKilled != 0)
                str += "totalTurretsKilled: " + totalTurretsKilled + "<br/>";
            if (totalUnrealKills != 0)
                str += "totalUnrealKills: " + totalUnrealKills + "<br/>";

            return str;
        }
    }
}