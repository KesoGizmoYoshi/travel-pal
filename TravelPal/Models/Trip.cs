using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelPal.Enums;
using TravelPal.Interfaces;

namespace TravelPal.Models;

public class Trip : Travel
{
    public TripTypes Type { get; set; }

    public Trip(string destination, Countries country, int travellers, List<PackingListItem> packingList, DateTime startDate, DateTime endDate, TripTypes type) : base(destination, country, travellers, packingList, startDate, endDate)
    {
        Type = type;
    }

    public override string GetInfo()
    {
        return "";
    }
}
