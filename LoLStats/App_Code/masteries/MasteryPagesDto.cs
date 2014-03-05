using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// MasteryPagesDto class
/// </summary>
public class MasteryPagesDto
{
    public HashSet<MasteryPageDto> pages;
    public long summonerId;

    public MasteryPageDto CurrentPage;

    public void DoAllCalculations()
    {
        foreach (MasteryPageDto page in pages)
            if (page.current)
                CurrentPage = page;

        doTreeCounts();

        sortMasteries();
    }

    void doTreeCounts()
    {
        //if (onlyCurrent)
        //{
        //    if (CurrentPage != null)
        //        CurrentPage.DoTreeCounts();
        //}
        //else
        //{
            foreach (MasteryPageDto page in pages)
                page.DoTreeCounts();
        //}
    }

    void sortMasteries()
    {
        foreach (MasteryPageDto page in pages)
        {
            page.SortMasteries();
        }
    }

    /*public override string ToString()
    {
        string str = "";

        foreach (MasteryPageDto page in pages)
        {
            str += page.ToString() + "<br/>";
        }

        return str;
    }*/
}