using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelPal.Enums;
using TravelPal.Interfaces;
using TravelPal.Models;

namespace TravelPal.Managers;

public class TravelManager
{
    public List<Travel> Travels { get; set; } = new();

    //Add Travel as a trip, returns the trip, so that the signedInUser can add it to its own list of travels
    public Travel AddTravel(string destination, Countries country, int travellers, List<IPackingListItem> packingList, DateTime startDate, DateTime endDate, TripTypes tripType)
    {
        Trip trip = new(destination, country, travellers, packingList, startDate, endDate, tripType);

        Travels.Add(trip);

        return trip;
    }

    // Add Travel as a vacation, returns the vacation, so that the signedInUser can add it to its own list of travels
    public Travel AddTravel(string destination, Countries country, int travellers, List<IPackingListItem> packingList, DateTime startDate, DateTime endDate, bool isAllInclusive)
    {
        Vacation vacation = new(destination, country, travellers, packingList, startDate, endDate, isAllInclusive);

        Travels.Add(vacation);

        return vacation;
    }

    public void RemoveTravel(Travel travelToRemove)
    {
        Travels.Remove(travelToRemove);

        // TODO add more logic here

        // ......
    }
    
    public int CalculateTravelDays(DateTime startDate, DateTime endDate)
    {
        return (int)(endDate - startDate).TotalDays;
    }
}


// TODO
//
//
// Displaya hela listan för Admin och men spara även travels borttagna av användaren i huvudlistan, bool isDeleted to ha koll
//


// Add public method to calculate travelDays for displaying in the AddTravelWindows


