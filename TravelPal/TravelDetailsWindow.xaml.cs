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
using System.Xml.Linq;
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

        if (userManager.SignedInUser.IsAdmin)
        {
            btnEdit.IsEnabled = false;
        }

        PopulateCommonFields();
        PopulateTravelTypeFields();
        PopulateListView();

        datePickerStartDate.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today));
    }

    /// <summary>
    /// Populates all fields that are not specific to one of the travel types.
    /// </summary>
    private void PopulateCommonFields()
    {
        txtDestination.Text = this.currentTravel.Destination;
        cbCountries.ItemsSource = Enum.GetNames(typeof(Countries));
        cbCountries.Text = this.currentTravel.Country.ToString();
        txtTravelers.Text = this.currentTravel.Travellers.ToString();
        datePickerStartDate.Text = this.currentTravel.StartDate.ToString();
        datePickerEndDate.Text = this.currentTravel.EndDate.ToString();
        lblTravelDays.Content = $"Number of travel days: {this.currentTravel.TravelDays}";
    }

    /// <summary>
    /// Populates the fields that are specific to the travel type, hides UI elements that are not needed.
    /// </summary>
    private void PopulateTravelTypeFields()
    {
        cbTravelType.ItemsSource = new[] { "Trip", "Vacation" };
        cbTripType.ItemsSource = Enum.GetNames(typeof(TripTypes));

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

    /// <summary>
    /// Method for populating the ListView with the items in the packing list. Disables the passport to be manually edited.
    /// </summary>
    private void PopulateListView()
    {
        lvPackingList.Items.Clear();

        foreach (IPackingListItem packingListItem in this.currentTravel.PackingList)
        {
            ListViewItem item = new();
            item.Content = packingListItem.GetInfo();
            item.Tag = packingListItem;

            if (packingListItem.Name.Equals("Passport"))
            {
                item.IsEnabled = false;
            }

            lvPackingList.Items.Add(item);
        }
    }

    /// <summary>
    /// Method for the Cancel-button, Closes TravelDetailsWindow
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Method for the Edit-button, basically just sets all relevant UI elements to enabled for the user to be able to edit.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
        btnSave.IsEnabled = true;
        btnEdit.IsEnabled = false;
    }

    /// <summary>
    /// Method for the Save-button, saves all the changes done by the user, if the input passes all the checks in place.
    /// Its even possible to switch between Trip/Vacation. Possible by removing the object and then recreating it with the new travel type.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        string newDestination = txtDestination.Text;
        string newCountry = (string)cbCountries.SelectedItem;
        string newStrTravellers = txtTravelers.Text;
        string newTravelType = (string)cbTravelType.SelectedItem;
        string newTripType = (string)cbTripType.SelectedItem;

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

            if (!newTravelType.Equals(this.currentTravel.GetType().Name)) 
            {
                if (currentTravel is Trip)
                {
                    Trip trip = (Trip)currentTravel;
                    User user = (User)userManager.SignedInUser;

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

                    user.Travels.Add(travelManager.AddTravel(newDestination, (Countries)Enum.Parse(typeof(Countries), newCountry), newTravellers, this.currentTravel.PackingList, newStartDate, newEndDate, isAllInclusive));

                    this.travelsWindow.DisplayTravels();
                    Close();
                }
                else if (currentTravel is Vacation)
                {
                    Vacation vacation = (Vacation)currentTravel;
                    User user = (User)userManager.SignedInUser;
                    
                    travelManager.RemoveTravel(vacation);
                    user.Travels.Remove(vacation);

                    user.Travels.Add(travelManager.AddTravel(newDestination, (Countries)Enum.Parse(typeof(Countries), newCountry), newTravellers, this.currentTravel.PackingList, newStartDate, newEndDate, (TripTypes)Enum.Parse(typeof(TripTypes), cbTripType.SelectedIndex.ToString())));

                    travelsWindow.DisplayTravels();
                    Close();
                }
            }
            else if(newTravelType == this.currentTravel.GetType().Name) 
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

    /// <summary>
    /// Method for the selecting travel type in the ComboBox, makes sure that the correct UI elements are visible depending on which trip type that is selected.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Method for selecting a item in the ListView, depending on which item the user selects, the correct UI elements will be visible and populated with the correct value.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void lvPackingList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ListViewItem item = (ListViewItem)lvPackingList.SelectedItem;

        if (item is not null)
        {
            IPackingListItem selectedItem = (IPackingListItem)item.Tag;

            btnEditItem.IsEnabled = true;

            txtNameOfTheItem.Text = selectedItem.Name;

            if (selectedItem is OtherItem)
            {
                chbDocument.Visibility = Visibility.Collapsed;
                chbRequired.Visibility = Visibility.Collapsed;
                txtNameOfTheItem.IsEnabled = true;
                txtQuantity.IsEnabled = true;

                OtherItem otherItem = (OtherItem)selectedItem;
                txtQuantity.Text = otherItem.Quantity.ToString();
            }
            else if (selectedItem is TravelDocument)
            {
                lblQuantity.Visibility = Visibility.Collapsed;
                txtQuantity.Visibility = Visibility.Collapsed;
                chbDocument.Visibility = Visibility.Visible;
                chbRequired.Visibility = Visibility.Visible;
                txtNameOfTheItem.IsEnabled = true;
                chbRequired.IsEnabled = true;
                chbDocument.IsChecked = true;

                TravelDocument travelDocumnet = (TravelDocument)selectedItem;

                if (travelDocumnet.Required)
                {
                    chbRequired.IsChecked = true;
                }
                else
                {
                    chbRequired.IsChecked = false;
                }
            }
        } 
    }

    /// <summary>
    /// Method for the EditItem-button, lets the user edit the item selected in the ListView.
    /// It is not possible to edit a TravelDocument to OtherItem and vice versa.
    /// The passport can not be changed by the user, its automatically updated when changing country.
    /// Error messages will be displayed if any check fails. The ListView updates after an item have been added.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnEditItem_Click(object sender, RoutedEventArgs e)
    {
        ListViewItem item = (ListViewItem)lvPackingList.SelectedItem;
        
        if(item is not null)
        {
            IPackingListItem selectedItem = (IPackingListItem)item.Tag;

            string newName = txtNameOfTheItem.Text;

            try
            {
                if (string.IsNullOrWhiteSpace(newName))
                {
                    throw new ArgumentException("Type in the name of the item");
                }
                else if (chbDocument.IsChecked is true)
                {
                    TravelDocument travelDocument = (TravelDocument)selectedItem;
                    travelDocument.Name = newName;

                    if (chbRequired.IsChecked is true)
                    {
                        travelDocument.Required = true;
                    }
                    else
                    {
                        travelDocument.Required = false;
                    }
                }
                else if (chbDocument.IsChecked is false)
                {
                    bool isQuantityAnInteger = int.TryParse(txtQuantity.Text, out int quantity);

                    if (isQuantityAnInteger)
                    {
                        OtherItem otherItem = (OtherItem)selectedItem;
                        otherItem.Name = newName;
                        otherItem.Quantity = quantity;
                    }
                    else
                    {
                        throw new ArgumentException("Type in the quantity of the item");
                    }
                }

                btnEditItem.IsEnabled = false;
                lvPackingList.SelectedItem = null;
                txtNameOfTheItem.Text = "";
                txtQuantity.Text = "";
                chbDocument.IsChecked = false;
                chbRequired.IsChecked = false;

                PopulateListView();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

    }

    /// <summary>
    /// Method for selecting country in the ComboBox. It displays the passport in the ListView.
    /// If the user decides to change country, then the passport will automatically have the required setting updated to reflect the new country.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cbCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        bool IsUserLocatedInEu = Enum.IsDefined(typeof(EuropeanCountries), userManager.SignedInUser.Location.ToString()); // true, if user live in Eu
        bool IsDestinationCountryInEu = Enum.IsDefined(typeof(EuropeanCountries), cbCountries.SelectedItem); // true, if destination country is in EU
        bool isRequired;

        if (IsUserLocatedInEu && !IsDestinationCountryInEu)
        {
            isRequired = true;
        }
        else if (IsUserLocatedInEu && IsDestinationCountryInEu)
        {
            isRequired = false;
        }
        else
        {
            isRequired = true;
        }

        foreach(ListViewItem item in lvPackingList.Items)
        {
            IPackingListItem packingListItem = (IPackingListItem)item.Tag;
            
            if(packingListItem.Name.Equals("Passport"))
            {
                TravelDocument travelDocument = (TravelDocument)packingListItem;
                travelDocument.Required = isRequired;
            }
        }

        PopulateListView();
    }

    /// <summary>
    /// Method for selecting an start date, calculates and displays the amount of travel days, if the user selected both start date and end date.
    /// Also makes user the user the sure cant select an end date that is before the selected start date.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void datePickerStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        datePickerEndDate.SelectedDate = null;

        if (datePickerEndDate.SelectedDate is not null)
        {

            int travelDays = travelManager.CalculateTravelDays((DateTime)datePickerStartDate.SelectedDate, (DateTime)datePickerEndDate.SelectedDate);
            lblTravelDays.Content = $"Number of travel days: {travelDays}";
        }

        datePickerEndDate.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, (DateTime)datePickerStartDate.SelectedDate));
    }

    /// <summary>
    /// Method for selecting an end date, calculates and displays the amount of travel days, if the user selected both start date and end date.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void datePickerEndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        if (datePickerStartDate.SelectedDate is not null && datePickerEndDate.SelectedDate is not null)
        {
            int travelDays = travelManager.CalculateTravelDays((DateTime)datePickerStartDate.SelectedDate, (DateTime)datePickerEndDate.SelectedDate);
            lblTravelDays.Content = $"Number of travel days: {travelDays}";
        }
    }
}
