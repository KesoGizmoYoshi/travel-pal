using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelPal.Interfaces;
using TravelPal.Models;

namespace TravelPal.Managers;

public class UserManager
{
    public List<IUser> Users { get; set; } = new();
    public IUser SignedInUser { get; set; }

    public UserManager()
    {
        Admin admin = new("Admin", "password", Enums.Countries.Antarctica);

        Users.Add(admin);
    }

    public bool AddUser(IUser userToAdd)
    {

        return true;
    }

    public void RemoveUser(IUser userToRemove)
    {

    }

    public bool UpdateUsername(IUser userToUpdate, string newUsername)
    {
        return true;
    }

    private bool ValidateUsername(string username)
    {
        return true;
    }

    public bool SignInUser(string username, string password)
    {
        return true;
    }



}
