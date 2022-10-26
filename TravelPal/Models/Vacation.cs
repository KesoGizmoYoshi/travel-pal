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

    public Vacation(string destination, Countries country, int travellers, List<PackingListItem> packingList, DateTime startDate, DateTime endDate, bool allInclusive) : base(destination, country, travellers, packingList, startDate, endDate)
    {
        AllInclusive = allInclusive;
    }

    public override string GetInfo()
    {
        return $"{base.Destination}";
    }
}
