using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelPal.Enums;
using TravelPal.Interfaces;

namespace TravelPal.Models;

public class Vacation : Travel
{
    public bool AllInclusive { get; set; }

    public Vacation(string destination, Countries country, int travellers, List<IPackingListItem> packingList, DateTime startDate, DateTime endDate, bool allInclusive) : base(destination, country, travellers, packingList, startDate, endDate)
    {
        AllInclusive = allInclusive;
    }

    /// <summary>
    /// Method for returning a interpolated string with info about the vacation.
    /// </summary>
    /// <returns>The method returns a string.</returns>
    public override string GetInfo()
    {
        return $"Destination: {base.Destination}\t Length of travel: {base.TravelDays} days";
    }
}
