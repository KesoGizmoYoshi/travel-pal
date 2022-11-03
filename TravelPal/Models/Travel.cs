using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelPal.Enums;
using TravelPal.Interfaces;

namespace TravelPal.Models;

public class Travel
{
    public string Destination { get; set; }
    public Countries Country { get; set; }
    public int Travellers { get; set; }
    public List<IPackingListItem> PackingList { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TravelDays { get; set; }
    public bool IsRemovedByUser { get; set; } = false;

    public Travel(string destination, Countries country, int travellers, List<IPackingListItem> packingList, DateTime startDate, DateTime endDate)
    {
        Destination = destination;
        Country = country;
        Travellers = travellers;
        PackingList = packingList;
        StartDate = startDate;
        EndDate = endDate;
        IsRemovedByUser = false;

        TravelDays = CalculateTravelDays();
    }

    public virtual string GetInfo()
    {
        return "";
    }

    /// <summary>
    /// Method for calculating travel days, by using the start date and the end date for a travel.
    /// </summary>
    /// <returns>This method returns an integer value of the amount of travel days</returns>
    private int CalculateTravelDays()
    {
        return (int)(EndDate - StartDate).TotalDays; ;
    }
}
