using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelPal.Enums;
using TravelPal.Interfaces;
using TravelPal.Models;

namespace TravelPal.Managers;

public class UserManager
{
    public List<IUser> Users { get; set; } = new();
    public IUser SignedInUser { get; set; }

    public void LoadDefaultUsers(TravelManager travelManager)
    {
        Admin admin = new("admin", "password", Enums.Countries.Antarctica);
        User gandalf = new("Gandalf", "password", Enums.Countries.Sweden);

        Users.Add(admin);
        Users.Add(gandalf);

        TravelDocument travelDocument1 = new("Passport", true);
        TravelDocument travelDocument2 = new("Passport", false);

        OtherItem otherItem1 = new("Toothbrush", 1);
        OtherItem otherItem2 = new("Sunscreen", 2);

        List<IPackingListItem> packingList1 = new();
        List<IPackingListItem> packingList2 = new();

        packingList1.Add(travelDocument1);
        packingList1.Add(otherItem1);
        packingList2.Add(travelDocument2);
        packingList2.Add(otherItem2);

        gandalf.Travels.Add(travelManager.AddTravel("DreamHack", Countries.Japan, 2, packingList1, new DateTime(2022, 11, 20), new DateTime(2022, 11, 25), TripTypes.Leisure));
        gandalf.Travels.Add(travelManager.AddTravel("Mallorca", Countries.Spain, 2, packingList2, new DateTime(2023, 07, 01), new DateTime(2023, 07, 14), true));
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
