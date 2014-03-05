using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



public class RunePageDto
{
    public bool current;
    public long id;
    public string name;
    public List<RuneSlotDto> slots;

    //public List<KeyValuePair<string, float>> totals;

	public RunePageDto()
	{
        //totals = new List<KeyValuePair<string, float>>();
	}

    /*public void CalculateTotals()
    {
        if (totals == null)
            totals = new List<KeyValuePair<string, float>>();

        this.totals.Clear();

        if (slots == null)
            return;

        // loop through rune slots
        foreach (RuneSlotDto runeSlot in slots)
        {
            // get rune description
            string str = runeSlot.rune.description;
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
    }

    public string AllRunesString()
    {
            string str = "";

            if (slots == null)
            {
                str += "empty" + "<br/>";
                return str;
            }

            slots.Sort();

            foreach (RuneSlotDto runeSlot in slots)
                str += runeSlot.ToString() + "<br/>";

            return str;
    }

    public string TotalsString()
    {
            string str = "";

            // build totals string
            foreach (KeyValuePair<string, float> pair in totals)
            {
                string description = pair.Key;
                string plus = (pair.Value > 0 ? "+" : "");

                if (description.Contains("per level"))
                {
                    description += " (" + plus + (pair.Value * 18).ToString("0.##") + (description.Contains('%') ? "%" : "") + " at lvl 18)";
                    description = description.Replace("level", "lvl");
                }
                str += plus + pair.Value.ToString("0.##") + description + "<br/>";
            }

            //str.Replace("level", "lvl");

            return str;
    }

    public string TotalsTable()
    {
            string str = "<table style=\"text-align: left; margin: auto; margin-left: 40px\">";

            // build totals string
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

                str += "<tr><td style=\"text-align: left; width: 55px\">" + plus + valueString + "</td><td style=\"padding-left: 4px; width: 200px\">" + description + "</td></tr>";
            }

            str += "</table>";

            return str;
    }

    string addColorToDescription(string description)
    {
        string str;

        str = description.Replace("magic penetration", "<span style=\"color: darkred\">magic penetration</span>");
        str = str.Replace("ability power", "<span style=\"color: darkred\">ability power</span>");
        str = str.Replace("spellvamp", "<span style=\"color: darkred\">spellvamp</span>");
        str = str.Replace("movement speed", "<span style=\"color: green\">movement speed</span>");
        str = str.Replace("attack speed", "<span style=\"color: darkred\">attack speed</span>");
        str = str.Replace("gold", "<span style=\"color: #FFCC00\">gold</span>");
        str = str.Replace("armor penetration", "<span style=\"color: darkred\">armor penetration</span>");
        str = str.Replace("cooldowns", "<span style=\"color: green\">cooldowns</span>");
        str = str.Replace("lifesteal", "<span style=\"color: darkred\">lifesteal</span>");
        str = str.Replace("attack damage", "<span style=\"color: darkred\">attack damage</span>");
        str = str.Replace("critical chance", "<span style=\"color: darkred\">critical chance</span>");
        str = str.Replace("magic resist", "<span style=\"color: blue\">magic resist</span>");
        str = str.Replace("health regen", "<span style=\"color: green\">health regen</span>");
        str = str.Replace("critical damage", "<span style=\"color: darkred\">critical damage</span>");
        str = str.Replace("mana regen", "<span style=\"color: blue\">mana regen</span>");

        if (!str.Contains("penetration"))
            str = str.Replace("armor", "<span style=\"color: blue\">armor</span>");
        if (!str.Contains("regen"))
        {
            str = str.Replace("health", "<span style=\"color: green\">health</span>");
            str = str.Replace("mana", "<span style=\"color: blue\">mana</span>");
        }

        return str;
    }*/
}