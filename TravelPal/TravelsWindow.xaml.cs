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
    private UserManager userManager = new();
    private TravelManager travelManager = new();
    private Travel selectedTravel;
    

    public TravelsWindow(UserManager userManager)
    {
        InitializeComponent();

        this.userManager = userManager;

        UpdateUsernameLabel();
        DisplayTravels();

        
    }

    private void DisplayTravels()
    {
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

        else if (userManager.SignedInUser.IsAdmin)
        {
            foreach(IUser iUser in userManager.Users)
            {
                if(iUser is User)
                {
                    User user = (User)iUser;

                    foreach (Travel travel in user.Travels)
                    {
                        ListViewItem item = new();
                        item.Tag = travel;
                        item.Content = travel.GetInfo();
                        lvTravels.Items.Add(item);
                    }
                }  
            }
        }
    }

    private void btnMyDetails_Click(object sender, RoutedEventArgs e)
    {
        UserDetailsWindow userDetailsWindow = new(userManager, this);

        userDetailsWindow.Show();
    }

    public void UpdateUsernameLabel()
    {
        lblUsername.Content = $"Welcome, {userManager.SignedInUser.Username}";
    }

    private void btnSignOut_Click(object sender, RoutedEventArgs e)
    {
        this.userManager.SignedInUser = null;
        MainWindow mainWindow = new MainWindow(this.userManager);
        mainWindow.Show();
        Close();
    }

    private void btnTravelDetails_Click(object sender, RoutedEventArgs e)
    {
        if (selectedTravel != null)
        {
            TravelDetailsWindow travelDetailsWindow = new(this.userManager, selectedTravel);

            travelDetailsWindow.Show();
        }
        else
        {
            MessageBox.Show("No travel selected!", "Warning!");
        }
    }

    private void btnRemoveTravel_Click(object sender, RoutedEventArgs e)
    {
        if (selectedTravel != null)
        {
            travelManager.RemoveTravel(selectedTravel);
            DisplayTravels();
        }
        else
        {
            MessageBox.Show("No travel selected!", "Warning!");
        }

    }

    private void btnAddTravel_Click(object sender, RoutedEventArgs e)
    {
        AddTravelWindow addTravelWindow = new(this.userManager);

        addTravelWindow.Show();
    }

    private void lvTravels_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ListViewItem selectedItem = (ListViewItem)lvTravels.SelectedItem;

        selectedTravel = (Travel)selectedItem.Tag;
    }
}
