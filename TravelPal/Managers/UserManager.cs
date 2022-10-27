using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelPal.Enums;
using TravelPal.Interfaces;
using TravelPal.Models;

namespace TravelPal.Managers;

public class UserManager
{
    private TravelManager travelManager = new();
    public List<IUser> Users { get; set; } = new();
    public IUser SignedInUser { get; set; }

    public UserManager()
    {
        Admin admin = new("Admin", "password", Enums.Countries.Antarctica);
        
        User Gandalf = new("Gandalf", "password", Enums.Countries.Sweden);

        OtherItem otherItem = new("Toothbrush", 2);

        List<IPackingListItem> packingList = new();

        packingList.Add(otherItem);

        DateTime startDate = new DateTime(2022, 10, 26);
        DateTime endDate = new DateTime(2022, 10, 31);

        Trip trip = new("DreamHack", Countries.Japan, 6, packingList, startDate, endDate, TripTypes.Work);

        //Gandalf.Travels.Add(trip);

        travelManager.Travels.Add(trip);

        Gandalf.Travels = travelManager.Travels;

        Users.Add(admin);
        Users.Add(Gandalf);
    }

    public bool AddUser(IUser userToAdd)
    {
        if (ValidateUsername(userToAdd.Username))
        {
            Users.Add(userToAdd);

            return true;
        }

        return false;
    }

    public bool UpdateUsername(IUser userToUpdate, string newUsername)
    {
        if (ValidateUsername(newUsername))
        {
            userToUpdate.Username = newUsername;

            return true;
        }
        else
        {
            return false;
        }
    }

    private bool ValidateUsername(string username)
    {
        foreach (IUser user in Users)
        {
            if (user.Username.Equals(username))
            {
                return false;
            }
        }

        return true;
    }

    public bool SignInUser(string username, string password)
    {
        foreach (IUser user in Users)
        {
            if (user.Username == username && user.Password == password)
            {
                SignedInUser = user;

                return true;
            }
        }

        return false;
    }



}
