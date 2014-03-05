using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class RuneSlotDto// : IComparable<RuneSlotDto>
{
    public RuneDto rune;
    public int runeSlotId;

	public RuneSlotDto()
	{
	}

    /*public override string ToString()
    {
        return "slot " + runeSlotId + ": " + rune;
    }

    public int CompareTo(RuneSlotDto other)
    {
        //return rune.name.CompareTo(other.rune.name);
        return runeSlotId.CompareTo(other.runeSlotId);
    }*/
}