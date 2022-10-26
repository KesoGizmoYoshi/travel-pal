using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelPal.Models;

namespace TravelPal.Managers;

public class TravelManager
{
    public List<Travel> Travels { get; set; } = new();

    // List<Travel> Travels ska vara samma i User-klassen och i travelManagern-klassen för en specifik user.
    // Displaya listan i TravelMananger för inloggad user
    // Är admin loggad så loopa igenom alla användare och skriv ut listan för varje användare


    public void AddTravel(Travel travelToAdd)
    {
        Travels.Add(travelToAdd);
    }

    public void RemoveTravel(Travel travelToRemove)
    {
        Travels.Remove(travelToRemove);
    }
}
