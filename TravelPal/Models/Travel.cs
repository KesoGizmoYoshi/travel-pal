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
    public List<PackingListItem> PackingList { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TravelDays { get; set; }

    public Travel(string destination, Countries country, int travellers, List<PackingListItem> packingList, DateTime startDate, DateTime endDate)
    {
        Destination = destination;
        Country = country;
        Travellers = travellers;
        PackingList = packingList;
        StartDate = startDate;
        EndDate = endDate;

        TravelDays = CalculateTravelDays();
    }

    public virtual string GetInfo()
    {
        return "";
    }

    private int CalculateTravelDays()
    {
        return 0;
    }
}
