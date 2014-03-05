using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// MasteryPageDto class
/// </summary>
public class MasteryPageDto
{
    static Dictionary<string, int> masteryDictionary = new Dictionary<string, int>();
    static MasteryPageDto()
    {
        // offense
        masteryDictionary.Add("Double-Edged Sword", 0);
        masteryDictionary.Add("Fury", 0);
        masteryDictionary.Add("Sorcery", 0);
        masteryDictionary.Add("Butcher", 0);
        masteryDictionary.Add("Expose Weakness", 0);
        masteryDictionary.Add("Brute Force", 0);
        masteryDictionary.Add("Mental Force", 0);
        masteryDictionary.Add("Feast", 0);
        masteryDictionary.Add("Spell Weaving", 0);
        masteryDictionary.Add("Martial Mastery", 0);
        masteryDictionary.Add("Arcane Mastery", 0);
        masteryDictionary.Add("Executioner", 0);
        masteryDictionary.Add("Blade Weaving", 0);
        masteryDictionary.Add("Warlord", 0);
        masteryDictionary.Add("Archmage", 0);
        masteryDictionary.Add("Dangerous Game", 0);
        masteryDictionary.Add("Frenzy", 0);
        masteryDictionary.Add("Devastating Strikes", 0);
        masteryDictionary.Add("Arcane Blade", 0);
        masteryDictionary.Add("Havoc", 0);

        // defense
        masteryDictionary.Add("Block", 1);
        masteryDictionary.Add("Recovery", 1);
        masteryDictionary.Add("Enchanted Armor", 1);
        masteryDictionary.Add("Tough Skin", 1);
        masteryDictionary.Add("Unyielding", 1);
        masteryDictionary.Add("Veteran's Scars", 1);
        masteryDictionary.Add("Bladed Armor", 1);
        masteryDictionary.Add("Oppression", 1);
        masteryDictionary.Add("Juggernaut", 1);
        masteryDictionary.Add("Hardiness", 1);
        masteryDictionary.Add("Resistance", 1);
        masteryDictionary.Add("Perseverance", 1);
        masteryDictionary.Add("Swiftness", 1);
        masteryDictionary.Add("Reinforced Armor", 1);
        masteryDictionary.Add("Evasive", 1);
        masteryDictionary.Add("Second Wind", 1);
        masteryDictionary.Add("Legendary Guardian", 1);
        masteryDictionary.Add("Runic Blessing", 1);
        masteryDictionary.Add("Tenacious", 1);

        // utility
        masteryDictionary.Add("Phasewalker", 2);
        masteryDictionary.Add("Fleet of Foot", 2);
        masteryDictionary.Add("Meditation", 2);
        masteryDictionary.Add("Scout", 2);
        masteryDictionary.Add("Summoner's Insight", 2);
        masteryDictionary.Add("Strength of Spirit", 2);
        masteryDictionary.Add("Alchemist", 2);
        masteryDictionary.Add("Greed", 2);
        masteryDictionary.Add("Runic Affinity", 2);
        masteryDictionary.Add("Vampirism", 2);
        masteryDictionary.Add("Culinary Master", 2);
        masteryDictionary.Add("Scavenger", 2);
        masteryDictionary.Add("Wealth", 2);
        masteryDictionary.Add("Expanded Mind", 2);
        masteryDictionary.Add("Inspiration", 2);
        masteryDictionary.Add("Bandit", 2);
        masteryDictionary.Add("Intelligence", 2);
        masteryDictionary.Add("Wanderer", 2);
    }

    static MasteryComparer masteryComparer = new MasteryComparer();

    public bool current;
    public long id;
    public string name;
    public List<TalentDto> talents;

    public int[] treeCounts;

    public MasteryPageDto() { }

    public void SortMasteries()
    {
        if (talents == null)
            return;

        talents.Sort(masteryComparer);
    }

    public void DoTreeCounts()
    {
        treeCounts = new int[3];

        if (talents == null)
            return;

        foreach (TalentDto talent in talents)
            treeCounts[masteryDictionary[talent.name]] += talent.rank;
    }

    public string TreeCounts()
    {
            return treeCounts[0] + "/" + treeCounts[1] + "/" + treeCounts[2];
    }

    /*public string AllTalents()
    {
            string str = "";

            if (talents == null)
            {
                str += "empty" + "<br/>";
                return str;
            }

            foreach (TalentDto talent in talents)
            {
                //treeCounts[masteryDictionary[talent.name]] += talent.rank;

                if (masteryDictionary[talent.name] == 0)
                    str += "<span style=\"color: darkred;\">";
                else if (masteryDictionary[talent.name] == 1)
                    str += "<span style=\"color: blue;\">";
                else
                    str += "<span style=\"color: green;\">";

                str += talent + "</span><br/>";
            }

            // string spec = "<span style=\"font-weight: bold\">" + treeCounts[0] + "/" + treeCounts[1] + "/" + treeCounts[2] + "</span>";

            //str = spec + "<br/>" + str;

            return str;
    }*/

    public string AllTalentsTable()
    {
        if (talents == null)
            return "";

        string str = "<table class=\"masteriesTable\">";


        foreach (TalentDto talent in talents)
        {
            str += "<tr><td style=\"text-align: right;\">" + talent.rank + "</td><td style=\"padding-left: 4px\">";

            if (masteryDictionary[talent.name] == 0)
                str += "<span style=\"color: darkred;\">";
            else if (masteryDictionary[talent.name] == 1)
                str += "<span style=\"color: blue;\">";
            else
                str += "<span style=\"color: green;\">";

            str += talent.name + "</span><br/></td></tr>";
        }

        str += "</table>";

        return str;
    }

    /*public override string ToString()
    {
        string spec = "";

        string str = "<span style=\"font-weight: bold;\">" + name + "</span><br/>";

        if (talents == null)
        {
            str += "empty" + "<br/>";
            return str;
        }

        foreach (TalentDto talent in talents)
        {
            str += talent + "<br/>";

            if (talent.name == "Havoc")
                spec = "OFFENSE SPEC";
            else if (talent.name == "Tenacious")
                spec = "DEFENSE SPEC";
            else if (talent.name == "Wanderer")
                spec = "UTILITY SPEC";
        }

        str += spec + "<br/>";

        return str;
    }*/

    public class MasteryComparer : IComparer<TalentDto>
    {
        int IComparer<TalentDto>.Compare(TalentDto t1, TalentDto t2)
        {
            return (masteryDictionary[t1.name] - masteryDictionary[t2.name]);
        }

    }
}