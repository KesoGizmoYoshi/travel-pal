using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
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
using TravelPal.Enums;
using TravelPal.Interfaces;
using TravelPal.Managers;
using TravelPal.Models;

namespace TravelPal;

/// <summary>
/// Interaction logic for TravelDetailsWindow.xaml
/// </summary>
public partial class TravelDetailsWindow : Window
{
    private UserManager userManager;
    private TravelManager travelManager;
    private TravelsWindow travelsWindow;
    private Travel currentTravel;
    public TravelDetailsWindow(UserManager userManager, TravelManager travelManager, TravelsWindow travelsWindow, Travel travel)
    {
        InitializeComponent();

        this.userManager = userManager;
        this.travelManager = travelManager;
        this.travelsWindow = travelsWindow;
        this.currentTravel = travel;

        txtDestination.Text = this.currentTravel.Destination;

        cbCountries.ItemsSource = Enum.GetNames(typeof(Countries));
        cbCountries.Text = this.currentTravel.Country.ToString();

        txtTravelers.Text = this.currentTravel.Travellers.ToString();

        cbTravelType.ItemsSource = new[]{ "Trip", "Vacation"};

        cbTripType.ItemsSource = Enum.GetNames(typeof(TripTypes));

        datePickerStartDate.Text = this.currentTravel.StartDate.ToString();
        datePickerEndDate.Text = this.currentTravel.EndDate.ToString();

        lblTravelDays.Content = $"Number of travel days: {this.currentTravel.TravelDays}";

        foreach(IPackingListItem packingListItem in this.currentTravel.PackingList) 
        {
            ListViewItem item = new();
            item.Content = packingListItem.GetInfo();
            item.Tag = packingListItem;
            lvPackingList.Items.Add(item);
        }

        if (this.currentTravel is Trip)
        {
            Trip trip = (Trip)this.currentTravel;

            cbTravelType.Text = "Trip";
            cbTripType.Text = trip.TripType.ToString();
            chbAllInclusive.Visibility = Visibility.Collapsed;

        }
        else if (this.currentTravel is Vacation)
        {
            Vacation vacation = (Vacation)this.currentTravel;

            cbTravelType.Text = "Vacation";

            if (vacation.AllInclusive)
            {
                chbAllInclusive.IsChecked = true;
            }

            cbTripType.Visibility = Visibility.Collapsed; 
        }

    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void btnEdit_Click(object sender, RoutedEventArgs e)
    {
        txtDestination.IsEnabled = true;
        cbCountries.IsEnabled = true;
        cbCountries.IsEnabled = true;
        txtTravelers.IsEnabled = true;
        cbTravelType.IsEnabled = true;
        cbTripType.IsEnabled = true;
        chbAllInclusive.IsEnabled = true;
        datePickerStartDate.IsEnabled = true;
        datePickerEndDate.IsEnabled = true;
        lvPackingList.IsEnabled = true;
        txtNameOfTheItem.IsEnabled = true;
        txtQuantity.IsEnabled = true;


        btnSave.IsEnabled = true;
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        string newDestination = txtDestination.Text;
        string newCountry = (string)cbCountries.SelectedItem;
        string newStrTravellers = txtTravelers.Text;
        string newTravelType = (string)cbTravelType.SelectedItem;
        string newTripType = (string)cbTripType.SelectedItem;
        //string newStrStartDate = datePickerStartDate.Text;
        //string newStrEndDate = datePickerEndDate.Text;

        try
        {
            if(string.IsNullOrWhiteSpace(newDestination))
            {
                throw new ArgumentException("Destination is empty");
            }
            else if (string.IsNullOrWhiteSpace(newStrTravellers))
            {
                throw new ArgumentException("Number of travellers is empty");
            }

            bool isTravellersAnInteger = int.TryParse(newStrTravellers, out int newTravellers);

            if (!isTravellersAnInteger)
            {
                throw new ArgumentException("Number of travellers must be an integer");
            }

            DateTime newStartDate;
            DateTime newEndDate;

            if (datePickerStartDate.SelectedDate > DateTime.Today)
            {
                newStartDate = (DateTime)datePickerStartDate.SelectedDate;
            }
            else
            {
                throw new ArgumentException("");
            }

            if (datePickerEndDate.SelectedDate > datePickerStartDate.SelectedDate)
            {
                newEndDate = (DateTime)datePickerEndDate.SelectedDate;
            }
            else
            {
                throw new ArgumentException("");
            }

            int travelDays = travelManager.CalculateTravelDays(newStartDate, newEndDate);

            if (cbTravelType.SelectedItem.ToString() != currentTravel.GetType().ToString())
            {
                if (currentTravel is Trip)
                {
                    Trip trip = (Trip)currentTravel;

                    User user = (User)userManager.SignedInUser;

                    List<IPackingListItem> packingList = new();

                    travelManager.RemoveTravel(trip);
                    user.Travels.Remove(trip);

                    bool isAllInclusive;

                    if ((bool) chbAllInclusive.IsChecked)
                    {
                        isAllInclusive = true;
                    }
                    else
                    {
                        isAllInclusive = false;
                    }

                    user.Travels.Add(travelManager.AddTravel(newDestination, (Countries)Enum.Parse(typeof(Countries), newCountry), newTravellers, packingList, newStartDate, newEndDate, isAllInclusive));

                    travelsWindow.DisplayTravels();
                    Close();
                }
                else if (currentTravel is Vacation)
                {
                    Vacation vacation = (Vacation)currentTravel;

                    User user = (User)userManager.SignedInUser;

                    List<IPackingListItem> packingList = new();
                    
                    travelManager.RemoveTravel(vacation);
                    user.Travels.Remove(vacation);

                    user.Travels.Add(travelManager.AddTravel(newDestination, (Countries)Enum.Parse(typeof(Countries), newCountry), newTravellers, packingList, newStartDate, newEndDate, (TripTypes)Enum.Parse(typeof(TripTypes), cbTripType.SelectedIndex.ToString())));

                    travelsWindow.DisplayTravels();
                    Close();
                }
            }
            else if(cbTravelType.SelectedItem.ToString() == currentTravel.GetType().ToString())
            {
                if (currentTravel is Trip)
                {
                    Trip trip = (Trip)currentTravel;

                    trip.TripType = (TripTypes)Enum.Parse(typeof(TripTypes), cbTripType.SelectedIndex.ToString());
                }
                else if (currentTravel is Vacation)
                {
                    Vacation vacation = (Vacation)currentTravel;

                    if (vacation.AllInclusive)
                    {
                        chbAllInclusive.IsChecked = true;
                    }
                    else
                    {
                        chbAllInclusive.IsChecked = false;
                    }
                }

                this.currentTravel.Destination = newDestination;
                this.currentTravel.Country = (Countries)Enum.Parse(typeof(Countries), newCountry);
                this.currentTravel.Travellers = newTravellers;
                this.currentTravel.StartDate = newStartDate;
                this.currentTravel.EndDate = newEndDate;

                this.currentTravel.TravelDays = travelManager.CalculateTravelDays(newStartDate, newEndDate);

                travelsWindow.DisplayTravels();
                Close();
            }
        }
        catch(ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }


    }

    private void cbTravelType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cbTravelType.SelectedItem.ToString().Equals("Trip"))
        {
            cbTripType.Visibility = Visibility.Visible;
            chbAllInclusive.Visibility = Visibility.Collapsed;
        }
        else if (cbTravelType.SelectedItem.ToString().Equals("Vacation"))
        {
            cbTripType.Visibility = Visibility.Collapsed;
            chbAllInclusive.Visibility = Visibility.Visible;
        }
    }

    private void lvPackingList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ListViewItem item = (ListViewItem)lvPackingList.SelectedItem;

        IPackingListItem selectedItem = (IPackingListItem)item.Tag;

        btnEditItem.IsEnabled = true;

        txtNameOfTheItem.Text = selectedItem.Name;
        
        if(selectedItem is OtherItem)
        {
            chbDocument.Visibility = Visibility.Hidden;
            chbRequired.Visibility = Visibility.Hidden;
            OtherItem otherItem = (OtherItem)selectedItem;

            txtQuantity.Text = otherItem.Quantity.ToString();
        }

    }

    private void btnEditItem_Click(object sender, RoutedEventArgs e)
    {
        ListViewItem item = (ListViewItem)lvPackingList.SelectedItem;

        IPackingListItem selectedItem = (IPackingListItem)item.Tag;

        selectedItem.Name = txtNameOfTheItem.Text;
        


    }
}
