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

    public void AddTravel(Travel travelToAdd)
    {
        Travels.Add(travelToAdd);
    }

    //Add Travel as a trip
    public Travel AddTravel(string destination, Countries country, int travellers, List<IPackingListItem> packingList, DateTime startDate, DateTime endDate, TripTypes tripType)
    {

    }

    // Add Travel as a Vacation

    public void RemoveTravel(Travel travelToRemove)
    {
        Travels.Remove(travelToRemove);
    }
}


// TODO
//
// add constructors, one for adding a trip and one for adding a vacation
//
// Displaya hela listan för Admin och men spara även travels borttagna av användaren i huvudlistan, bool isDeleted to ha koll
//
