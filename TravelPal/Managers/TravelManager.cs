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

    /// <summary>
    /// Method for creating a Trip with parameters and adding it to the Travels list.
    /// /// </summary>
    /// <param name="destination"></param>
    /// <param name="country"></param>
    /// <param name="travellers"></param>
    /// <param name="packingList"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="tripType"></param>
    /// <returns>This method returns the Trip that gets added to the list Travels.</returns>
    public Travel AddTravel(string destination, Countries country, int travellers, List<IPackingListItem> packingList, DateTime startDate, DateTime endDate, TripTypes tripType)
    {
        Trip trip = new(destination, country, travellers, packingList, startDate, endDate, tripType);
        Travels.Add(trip);
        return trip;
    }

    /// <summary>
    /// Method for creating a Vacation with the parameters and adding it to the Travels list.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="country"></param>
    /// <param name="travellers"></param>
    /// <param name="packingList"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="isAllInclusive"></param>
    /// <returns>This method returns the Vacation that gets added to the list Travels.</returns>
    public Travel AddTravel(string destination, Countries country, int travellers, List<IPackingListItem> packingList, DateTime startDate, DateTime endDate, bool isAllInclusive)
    {
        Vacation vacation = new(destination, country, travellers, packingList, startDate, endDate, isAllInclusive);
        Travels.Add(vacation);
        return vacation;
    }

    /// <summary>
    /// Method for removing a Travel from the Travels list.
    /// </summary>
    /// <param name="travelToRemove"></param>
    public void RemoveTravel(Travel travelToRemove)
    {
        Travels.Remove(travelToRemove);
    }

    /// <summary>
    /// Method for calculating travel days, by using the start date and the end date for a travel.
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns>This method returns an integer value of the amount of travel days</returns>
    public int CalculateTravelDays(DateTime startDate, DateTime endDate)
    {
        return (int)(endDate - startDate).TotalDays;
    }
}



