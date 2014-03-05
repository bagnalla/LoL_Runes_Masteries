using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;


public class SummonerDataManager
{
    public static void WriteSummonerInfoFile(SummonerDto summoner, string region, HttpServerUtility server)
    {
        string summonerFilePath = server.MapPath(@"~/App_Data/Summoner_Data/" + region + '/' + summoner.id);

        string str = summoner.revisionDate + "\n" +
            summoner.id + "\n" +
            summoner.name + "\n" +
            summoner.profileIconId + "\n" +
            summoner.summonerLevel;

        if (!Directory.Exists(summonerFilePath))
            Directory.CreateDirectory(summonerFilePath);

        File.WriteAllText(summonerFilePath + @"/info.txt", str);
    }

    public static void WriteSummonerRunesFile(SummonerDto summoner, RunePagesDtoManager runePagesManager, string region, HttpServerUtility server)
    {
        string summonerRunesPath = server.MapPath(@"~/App_Data/Summoner_Data/" + region + '/' + summoner.id + @"/runes.json");

        FileStream fout = File.OpenWrite(summonerRunesPath);

        JsonSerializer serializer = new JsonSerializer();
        JsonTextWriter jsonTextWriter = new JsonTextWriter(new StreamWriter(fout));

        serializer.Serialize(jsonTextWriter, runePagesManager.runePagesDto);

        jsonTextWriter.Flush();
        jsonTextWriter.Close();

        fout.Close();
    }

    public static RunePagesDto ReadSummonerRunesFile(SummonerDto summoner, string region, HttpServerUtility server)
    {
        string summonerRunesPath = server.MapPath(@"~/App_Data/Summoner_Data/" + region + '/' + summoner.id + @"/runes.json");

        FileStream fin = File.OpenRead(summonerRunesPath);

        //FileStream fin = new FileStream(summonerRunesPath, FileMode.Open);

        JsonSerializer serializer = new JsonSerializer();
        JsonTextReader jsonTextReader = new JsonTextReader(new StreamReader(fin));
        RunePagesDto runePages = (RunePagesDto)serializer.Deserialize(jsonTextReader, typeof(RunePagesDto));

       // RunePagesDto runePages = (RunePagesDto)JsonFormatter.Deserialize(typeof(RunePagesDto), fin);

        fin.Close();

        return runePages;
    }

    public static void WriteSummonerMasteriesFile(SummonerDto summoner, MasteryPagesDto masteryPages, string region, HttpServerUtility server)
    {
        string summonerMasteriesPath = server.MapPath(@"~/App_Data/Summoner_Data/" + region + '/' + summoner.id + @"/masteries.json");

        FileStream fout = File.OpenWrite(summonerMasteriesPath);

        JsonSerializer serializer = new JsonSerializer();
        JsonTextWriter jsonTextWriter = new JsonTextWriter(new StreamWriter(fout));

        serializer.Serialize(jsonTextWriter, masteryPages);

        jsonTextWriter.Flush();
        jsonTextWriter.Close();

        fout.Close();
    }

    public static MasteryPagesDto ReadSummonerMasteriesFile(SummonerDto summoner, string region, HttpServerUtility server)
    {
        string summonerMasteriesPath = server.MapPath(@"~/App_Data/Summoner_Data/" + region + '/' + summoner.id + @"/masteries.json");

        FileStream fin = File.OpenRead(summonerMasteriesPath);

        //FileStream fin = new FileStream(summonerRunesPath, FileMode.Open);

        JsonSerializer serializer = new JsonSerializer();
        JsonTextReader jsonTextReader = new JsonTextReader(new StreamReader(fin));
        MasteryPagesDto masterypages = (MasteryPagesDto)serializer.Deserialize(jsonTextReader, typeof(MasteryPagesDto));

        // RunePagesDto runePages = (RunePagesDto)JsonFormatter.Deserialize(typeof(RunePagesDto), fin);

        fin.Close();

        return masterypages;
    }

    public static bool HasSummonerChanged(SummonerDto summoner, string region, HttpServerUtility server)
    {
        string summonerFilePath = server.MapPath(@"~/App_Data/Summoner_Data/" + region + '/' + summoner.id);

        if (Directory.Exists(summonerFilePath))
        {
            long revisionDate = long.Parse(File.ReadAllLines(summonerFilePath + @"/info.txt")[0]);

            return (revisionDate != summoner.revisionDate);
        }
        else
            return true;
    }

    public static bool RunesFileExists(SummonerDto summoner, string region, HttpServerUtility server)
    {
        //return (File.Exists(server.MapPath(@"~/App_Data/Summoner_Data/" + region + '/' + summoner.id + @"/runes.json")));
        return (File.Exists(server.MapPath(@"~/App_Data/Summoner_Data/" + region + '/' + summoner.id + @"/runetotals.dat")));
    }

    public static bool MasteriesFileExists(SummonerDto summoner, string region, HttpServerUtility server)
    {
        return (File.Exists(server.MapPath(@"~/App_Data/Summoner_Data/" + region + '/' + summoner.id + @"/masteries.json")));
    }

    public static void ResetSummonerRevisionDate(SummonerDto summoner, string region, HttpServerUtility server)
    {
        string summonerFilePath = server.MapPath(@"~/App_Data/Summoner_Data/" + region + '/' + summoner.id);

        if (Directory.Exists(summonerFilePath))
        {
            string[] lines = File.ReadAllLines(summonerFilePath + @"/info.txt");

            lines[0] = "0";

            File.WriteAllLines(summonerFilePath + @"/info.txt", lines);
        }
    }

    public static long CacheSize(HttpServerUtility server)
    {
        DirectoryInfo d = new DirectoryInfo(server.MapPath(@"~/App_Data/Summoner_Data/"));

        if (d.Exists)
        {
            var files = d.GetFiles("*", SearchOption.AllDirectories);
            return files.Sum(fi => fi.Length);
        }
        else
            return 0;
    }

    public static void ClearCache(HttpServerUtility server)
    {
        DirectoryInfo dir = new DirectoryInfo(server.MapPath(@"~/App_Data/Summoner_Data/"));

        foreach (FileInfo file in dir.GetFiles())
            file.Delete();
        foreach (DirectoryInfo d in dir.GetDirectories())
            d.Delete(true);
    }

    public static void WriteRuneTotals(SummonerDto summoner, RunePagesDtoManager runePagesManager, string region, HttpServerUtility server)
    {
        List<string> lines = new List<string>();
        if (runePagesManager.CurrentPage != null)
        {
            lines.Add(runePagesManager.CurrentPage.name);
            lines.Add(runePagesManager.RunePageTotalsTable(runePagesManager.CurrentPage));
        }
        else
        {
            lines.Add("");
            lines.Add("");
        }

        foreach (RunePageDto runePage in runePagesManager.runePagesDto.pages)
        {
            if (!runePage.current)
            {
                lines.Add(runePage.name);
                lines.Add(runePagesManager.RunePageTotalsTable(runePage));
            }
        }

        File.WriteAllLines(server.MapPath(@"~/App_Data/Summoner_Data/" + region + '/' + summoner.id + @"/runetotals.dat"), lines);
    }

    public static string[] ReadRuneTotals(SummonerDto summoner, string region, HttpServerUtility server)
    {
        return File.ReadAllLines(server.MapPath(@"~/App_Data/Summoner_Data/" + region + '/' + summoner.id + @"/runetotals.dat"));
    }
}