using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;


public class RunePagesDto
{
    public HashSet<RunePageDto> pages;
    public long summonerId;
}

public class RunePagesDtoManager
{
    public RunePagesDto runePagesDto;

    public RunePageDto CurrentPage
    {
        get
        {
            foreach (RunePageDto page in runePagesDto.pages)
                if (page.current)
                    return page;
            return null;
        }
    }

    public RunePagesDtoManager(RunePagesDto runePagesDto)
    {
        this.runePagesDto = runePagesDto;
    }

    /*public void DoAllCalculations(bool onlyCurrent)
    {
        // set CurrentPage
        foreach (RunePageDto page in runePagesDto.pages)
        {
            if (page.current)
            {
                CurrentPage = page;
                break;
            }
        }

        calculateTotals(onlyCurrent);
    }

    void calculateTotals(bool onlyCurrent)
    {
        if (onlyCurrent)
        {
            if (CurrentPage != null)
                CurrentPage.CalculateTotals();
        }
        else
        {
            foreach (RunePageDto page in runePagesDto.pages)
                page.CalculateTotals();
        }
    }*/

    public KeyValuePair<string, float>[] CalculateTotals(RunePageDto runePage)
    {
        List<KeyValuePair<string, float>> totals = new List<KeyValuePair<string, float>>();
        string str;

        // loop through rune slots
        foreach (RuneSlotDto runeSlot in runePage.slots)
        {
            // get rune description
            str = runeSlot.rune.description;
            str = str.ToLower();

            // remove anything in parentheses from string
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '(')
                {
                    str = str.Remove(i - 1);
                    break;
                }
            }

            // if hybrid
            if (runeSlot.rune.name.Contains("Hybrid"))
            {
                string str1 = "", str2 = "";

                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == '/')
                    {
                        str1 = str.Substring(0, i - 1);
                        str2 = str.Substring(i + 2);
                        break;
                    }
                }

                int multiplier;

                if (str1[0] == '+')
                    multiplier = 1;
                else
                    multiplier = -1;

                // cut off + or -
                str1 = str1.Substring(1);
                str2 = str2.Substring(1);

                // strings to hold the value
                string valueStr1 = "", valueStr2 = "";

                // build valueStr1 then cut it off str1
                for (int i = 0; i < str1.Length; i++)
                {
                    char c = str1[i];

                    if (char.IsDigit(c) || c == '.')
                        valueStr1 += c;
                    else
                    {
                        str1 = str1.Substring(i);
                        break;
                    }
                }
                // build valueStr2 then cut it off str2
                for (int i = 0; i < str2.Length; i++)
                {
                    char c = str2[i];

                    if (char.IsDigit(c) || c == '.')
                        valueStr2 += c;
                    else
                    {
                        str2 = str2.Substring(i);
                        break;
                    }
                }

                // parse actual value from valueStr1
                float value1 = float.Parse(valueStr1) * multiplier;
                // parse actual value from valueStr2
                float value2 = float.Parse(valueStr2) * multiplier;

                // add value1 to totals
                bool found = false;
                for (int i = 0; i < totals.Count; i++)
                {
                    KeyValuePair<string, float> pair = totals[i];

                    if (pair.Key == str1)
                    {
                        totals[i] = new KeyValuePair<string, float>(pair.Key, pair.Value + value1);
                        found = true;
                        break;
                    }
                }
                // if not already in list, add it
                if (!found)
                    totals.Add(new KeyValuePair<string, float>(str1, value1));

                // add value2 to totals
                found = false;
                for (int i = 0; i < totals.Count; i++)
                {
                    KeyValuePair<string, float> pair = totals[i];

                    if (pair.Key == str2)
                    {
                        totals[i] = new KeyValuePair<string, float>(pair.Key, pair.Value + value2);
                        found = true;
                        break;
                    }
                }
                // if not already in list, add it
                if (!found)
                    totals.Add(new KeyValuePair<string, float>(str2, value2));
            }
            // if not hybrid
            else
            {

                int multiplier;

                if (str[0] == '+')
                    multiplier = 1;
                else
                    multiplier = -1;

                // cut off + or -
                str = str.Substring(1);

                // string to hold the value
                string valueStr = "";

                // build value string then cut it off str
                for (int i = 0; i < str.Length; i++)
                {
                    char c = str[i];

                    if (char.IsDigit(c) || c == '.')
                        valueStr += c;
                    else
                    {
                        str = str.Substring(i);
                        break;
                    }
                }

                // parse actual value from valueStr
                float value = float.Parse(valueStr) * multiplier;

                // add value to totals
                bool found = false;
                for (int i = 0; i < totals.Count; i++)
                {
                    KeyValuePair<string, float> pair = totals[i];

                    if (pair.Key == str)
                    {
                        totals[i] = new KeyValuePair<string, float>(pair.Key, pair.Value + value);
                        found = true;
                        break;
                    }
                }
                // if not already in list, add it
                if (!found)
                    totals.Add(new KeyValuePair<string, float>(str, value));
            }
        }

        return totals.ToArray();
    }

    public string RunePageTotalsTable(RunePageDto runePage)
    {
        if (runePage.slots == null)
            return "";

        KeyValuePair<string, float>[] totals = CalculateTotals(runePage);

        // build totals table
        //string str = "<table class=\"runeTotalsTable\">";
        StringBuilder str = new StringBuilder("<table class=\"runeTotalsTable\">");

        foreach (KeyValuePair<string, float> pair in totals)
        {
            string description = pair.Key;
            string plus = (pair.Value > 0 ? "+" : "");
            string valueString = pair.Value.ToString("0.##");

            if (description.Contains("per level"))
            {
                //valueString += '(' + plus + (pair.Value * 18).ToString("0.##") + ')';
                valueString = (pair.Value * 18).ToString("0.##");
                description = description.Replace("per level", "at lvl 18");
            }
            //str += plus + pair.Value.ToString("0.##") + description + "<br/>";

            if (description[0] == '%')
            {
                valueString += '%';
                description = description.Substring(1);
            }

            if (description.Contains('.'))
                description = description.Remove(description.IndexOf('.'), 1);

            description = description.Replace("increased ", "");

            description = addColorToDescription(description);

            //str += "<tr><td class=\"runeTotalsLeftColumn\">" + plus + valueString + "</td><td class=\"runeTotalsRightColumn\">" + description + "</td></tr>";
            str.Append("<tr><td class=\"runeTotalsLeftColumn\">");
            str.Append(plus);
            str.Append(valueString);
            str.Append("</td><td class=\"runeTotalsRightColumn\">");
            str.Append(description);
            str.Append("</td></tr>");
        }

        //str += "</table>";
        str.Append("</table>");

        return str.ToString();
    }

    // current page is first
    /*public string[] RawTotals()
    {
        List<string> lines = new List<string>();



        foreach (RunePageDto runePage in runePagesDto.pages)
        {
        }
    }*/

    string addColorToDescription(string description)
    {
        if (description.Contains("penetration"))
        {
            if (description.Contains("armor"))
                return description.Replace("armor penetration", "<span style=\"color: darkred\">armor penetration</span>");
            else
                return description.Replace("magic penetration", "<span style=\"color: darkred\">magic penetration</span>");
        }
        else
        {
            if (description.Contains("armor"))
                return description.Replace("armor", "<span style=\"color: blue\">armor</span>");
        }

        if (description.Contains("regen"))
        {
            if (description.Contains("health"))
                return description.Replace("health regen", "<span style=\"color: green\">health regen</span>");
            else
                return description.Replace("mana regen", "<span style=\"color: blue\">mana regen</span>");
        }
        else
        {
            if (description.Contains("health"))
                return description.Replace("health", "<span style=\"color: green\">health</span>");
            else if (description.Contains("mana"))
                return description.Replace("mana", "<span style=\"color: blue\">mana</span>");
        }

        if (description.Contains("ability power"))
            return description.Replace("ability power", "<span style=\"color: darkred\">ability power</span>");
        if (description.Contains("spellvamp"))
            return description.Replace("spellvamp", "<span style=\"color: darkred\">spellvamp</span>");
        if (description.Contains("movement speed"))
            return description.Replace("movement speed", "<span style=\"color: green\">movement speed</span>");
        if (description.Contains("attack speed"))
            return description.Replace("attack speed", "<span style=\"color: darkred\">attack speed</span>");
        if (description.Contains("gold"))
            return description.Replace("gold", "<span style=\"color: #FFCC00\">gold</span>");
        if (description.Contains("cooldowns"))
            return description.Replace("cooldowns", "<span style=\"color: green\">cooldowns</span>");
        if (description.Contains("lifesteal"))
            return description.Replace("lifesteal", "<span style=\"color: darkred\">lifesteal</span>");
        if (description.Contains("attack damage"))
            return description.Replace("attack damage", "<span style=\"color: darkred\">attack damage</span>");
        if (description.Contains("critical chance"))
            return description.Replace("critical chance", "<span style=\"color: darkred\">critical chance</span>");
        if (description.Contains("magic resist"))
            return description.Replace("magic resist", "<span style=\"color: blue\">magic resist</span>");
        if (description.Contains("critical damage"))
            return description.Replace("critical damage", "<span style=\"color: darkred\">critical damage</span>");

        return description;
    }

    public override string ToString()
    {
        string str = "";

        foreach (RunePageDto runePage in runePagesDto.pages)
        {
            str += runePage + "<br/>";
        }

        return str;
    }
}