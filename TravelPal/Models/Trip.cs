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
    public TripTypes TripType { get; set; }

    public Trip(string destination, Countries country, int travellers, List<IPackingListItem> packingList, DateTime startDate, DateTime endDate, TripTypes tripType) : base(destination, country, travellers, packingList, startDate, endDate)
    {
        TripType = tripType;
    }

    public override string GetInfo()
    {
        return $"Destination: {base.Destination}\t Length of travel: {base.TravelDays} days";
    }
}
