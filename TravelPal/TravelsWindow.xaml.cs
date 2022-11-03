using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TravelPal.Interfaces;
using TravelPal.Managers;
using TravelPal.Models;

namespace TravelPal;

/// <summary>
/// Interaction logic for TravelsWindow.xaml
/// </summary>
public partial class TravelsWindow : Window
{
    private UserManager userManager;
    private TravelManager travelManager;
    private Travel selectedTravel;
    private IUser signedInUser;
    
    public TravelsWindow(UserManager userManager, TravelManager travelManager)
    {
        InitializeComponent();

        this.userManager = userManager;
        this.travelManager = travelManager;
        signedInUser = this.userManager.SignedInUser;

        UpdateUsernameLabel();
        DisplayTravels();    
    }

    /// <summary>
    /// Displays travels in the ListView 
    /// </summary>
    public void DisplayTravels()
    {
        lvTravels.Items.Clear();

        if (userManager.SignedInUser is User)
        {
            User signedInUser = (User)userManager.SignedInUser;

            foreach (Travel travel in signedInUser.Travels)
            {
                ListViewItem item = new();
                item.Tag = travel;
                item.Content = travel.GetInfo();
                lvTravels.Items.Add(item);
            }
        }
        else if (signedInUser.IsAdmin)
        {
            foreach (Travel travel in travelManager.Travels)
            {
                ListViewItem item = new();
                item.Tag = travel;

                if (travel.IsRemovedByUser)
                {
                    string removedByUser = $"{travel.GetInfo()}\tRemoved by user";
                    item.Content = removedByUser;
                }
                else
                {
                    item.Content = travel.GetInfo();
                }

                lvTravels.Items.Add(item);
            }
        }
    }

    /// <summary>
    /// Method for the MyDetails-button, opens the UserDetailsWindow
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnMyDetails_Click(object sender, RoutedEventArgs e)
    {
        UserDetailsWindow userDetailsWindow = new(userManager, this);

        userDetailsWindow.Show();
    }

    /// <summary>
    /// Method for updating the username label.
    /// </summary>
    public void UpdateUsernameLabel()
    {
        lblUsername.Content = $"{userManager.SignedInUser.Username}";
    }

    /// <summary>
    /// Method for the SignOut-button, signs out the user.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSignOut_Click(object sender, RoutedEventArgs e)
    {
        this.userManager.SignedInUser = null;
        MainWindow mainWindow = new MainWindow(this.userManager, this.travelManager);
        mainWindow.Show();
        Close();
    }

    /// <summary>
    /// Method for the TravelDetails-button, opens the TravelDetailsWindow if the user have selected a travel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnTravelDetails_Click(object sender, RoutedEventArgs e)
    {
        if (selectedTravel != null)
        {
            TravelDetailsWindow travelDetailsWindow = new(this.userManager, this.travelManager, this, selectedTravel);

            travelDetailsWindow.Show();
        }
        else
        {
            MessageBox.Show("No travel selected!", "Warning!");
        }
    }

    /// <summary>
    /// Method for the RemoveTravel-button, the travel gets removed from the users Travels list and the travel is flagged as removed in the list that the Admin can access.
    /// When an Admin removes a travel its removed from the main list but also the specific list for the user.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRemoveTravel_Click(object sender, RoutedEventArgs e)
    {
        if (selectedTravel != null)
        {   
            if(!signedInUser.IsAdmin)
            {
                User user = (User)signedInUser;
                user.Travels.Remove(selectedTravel);

                foreach(Travel travel in travelManager.Travels)
                {
                    if (travel.Equals(selectedTravel))
                    {
                        travel.IsRemovedByUser = true;
                    }
                }
            }
            else
            {
                foreach(IUser iUser in userManager.Users)
                {
                    if(iUser is User)
                    {
                        User user = (User)iUser;

                        if(user.Travels.Contains(selectedTravel))
                        {
                            user.Travels.Remove(selectedTravel);
                        }
                    }
                }
                travelManager.RemoveTravel(selectedTravel);
            }

            selectedTravel = null;
            DisplayTravels();
        }
        else
        {
            MessageBox.Show("No travel selected!", "Warning!");
        }

    }

    /// <summary>
    /// Method for the AddTravel-button, opens the AddTravelWindow for the user, displays an error message for the Admin.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddTravel_Click(object sender, RoutedEventArgs e)
    {
        if (!signedInUser.IsAdmin)
        {
            AddTravelWindow addTravelWindow = new(this.userManager, this.travelManager, this);

            addTravelWindow.Show();
        }
        else
        {
            MessageBox.Show("Only users are allowed to add travels!", "Warning!");
        }

    }

    /// <summary>
    /// Method for the selection of the travel in the ListView, preventing the app from crashing if the selectedItem is null.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void lvTravels_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ListViewItem selectedItem = (ListViewItem)lvTravels.SelectedItem;

        if (selectedItem is not null)
        {
            selectedTravel = (Travel)selectedItem.Tag;
        }
    }

    /// <summary>
    /// Method of the About-button, just displaying a MessageBox with some info about TravelPal
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAbout_Click(object sender, RoutedEventArgs e)
    {
        string infoMessage = "Dear User! Welcome to TravelPal!" +
            "\nYour new favorite app for managing your travels." +
            "\nYou can add, edit and remove travels." +
            "\nWe do not offer any support, not even any useful tool tips." +
            "\nSo you are completely on your own, but thanks again for using TravelPal";

        MessageBox.Show(infoMessage, "About", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
