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

    public TravelManager()
    {
        //OtherItem otherItem = new("Toothbrush", 2);

        //List<IPackingListItem> packingList = new();

        //packingList.Add(otherItem);

        //DateTime startDate = new DateTime(2022, 10, 26);
        //DateTime endDate = new DateTime(2022, 10, 31);

        //AddTravel("DreamHack", Countries.Japan, 6, packingList, startDate, endDate, TripTypes.Work);
    }

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
    }
}


// TODO
//
//
// Displaya hela listan för Admin och men spara även travels borttagna av användaren i huvudlistan, bool isDeleted to ha koll
//



// maybe change privat field variable to public property, OR create a travelManager-object in MainWindow

// Add public method to calculate travelDays for displaying in the AddTravelWindows


