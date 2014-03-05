using System;
using System.Collections.Generic;

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
        foreach (MasteryPageDto page in pages)
            page.DoTreeCounts();
    }

    void sortMasteries()
    {
        foreach (MasteryPageDto page in pages)
        {
            page.SortMasteries();
        }
    }
}