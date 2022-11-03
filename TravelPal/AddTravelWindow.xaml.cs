using System;
using System.CodeDom;
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
/// Interaction logic for AddTravelWindow.xaml
/// </summary>
public partial class AddTravelWindow : Window
{
    private List<IPackingListItem> packingList = new();
    private TravelManager travelManager;
    private UserManager userManager;
    private TravelsWindow travelsWindow;
    private TravelDocument travelDocument;
    private User signedInUser;

    public AddTravelWindow(UserManager userManager, TravelManager travelManager, TravelsWindow travelsWindow)
    {
        InitializeComponent();

        this.userManager = userManager;
        this.travelManager = travelManager;
        this.travelsWindow = travelsWindow;

        signedInUser = (User)this.userManager.SignedInUser;

        datePickerStartDate.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today));

        PopulateAllComboBoxes();
    }

    /// <summary>
    /// Method for populating all the ComboBoxes with values
    /// </summary>
    private void PopulateAllComboBoxes()
    {
        cbCountries.ItemsSource = Enum.GetNames(typeof(Countries));
        cbTravelType.ItemsSource = new[] { "Trip", "Vacation" };
        cbTripType.ItemsSource = Enum.GetNames(typeof(TripTypes));
    }

    /// <summary>
    /// Method for AddTravel-button, calling methods for adding a new Travel, Trip or Vacation depending on what the user have selected.
    /// Plenty of checks for all the input data, error messages will be displayed if any check fails.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddTravel_Click(object sender, RoutedEventArgs e)
    {
        string destination = txtDestination.Text;
        string strCountry = (string)cbCountries.SelectedItem;
        string strTravellers = txtTravelers.Text;
        string travelType = (string)cbTravelType.SelectedItem;

        string[] inputsToCheck = { destination, strCountry, strTravellers, travelType };
        string[] errorMessages =
        {
                "Type in a destination",
                "Select a Country",
                "Type in the number of travellers",
                "Select a travel type",
                "Select a trip type",
                "Select a start date",
                "Select a end date",
                "Number of travellers must be an integer"
        };

        int index = 0;

        try
        {
            foreach (string input in inputsToCheck)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    throw new ArgumentException(errorMessages[index]);
                }

                index++;
            }

            Countries country = (Countries)Enum.Parse(typeof(Countries), strCountry);

            DateTime startDate;
            DateTime endDate;

            if (datePickerStartDate.SelectedDate is not null && datePickerStartDate.SelectedDate > DateTime.Today)
            {
                startDate = (DateTime)datePickerStartDate.SelectedDate;
            }
            else
            {
                throw new ArgumentException(errorMessages[5]);
            }

            if (datePickerEndDate.SelectedDate is not null && datePickerEndDate.SelectedDate > datePickerStartDate.SelectedDate)
            {
               endDate  = (DateTime)datePickerEndDate.SelectedDate;
            }
            else
            {
                throw new ArgumentException(errorMessages[6]);
            }

            int travelDays = travelManager.CalculateTravelDays(startDate, endDate);

            bool isTravellersAnInteger = int.TryParse(strTravellers, out int travellers);

            if (!isTravellersAnInteger)
            {
                throw new ArgumentException(errorMessages[7]);
            }

            if (travelType.Equals("Trip"))
            {
                string strTripType = (string)cbTripType.SelectedItem;

                TripTypes tripType;

                if (string.IsNullOrWhiteSpace(strTripType))
                {
                    throw new ArgumentException(errorMessages[4]);
                }
                else
                {
                    tripType = (TripTypes)Enum.Parse(typeof(TripTypes), strTripType);
                }

                signedInUser.Travels.Add(travelManager.AddTravel(destination, country, travellers, packingList, startDate, endDate, tripType));
            }
            else if (travelType.Equals("Vacation"))
            {
                bool isAllInclusive = (bool)chbAllInclusive.IsChecked;

                signedInUser.Travels.Add(travelManager.AddTravel(destination, country, travellers, packingList, startDate, endDate, isAllInclusive));
            }

            this.travelsWindow.DisplayTravels();
            Close();
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }

    /// <summary>
    /// Method for the AddItem-button, add a new item to the packingList, checks for the input data. 
    /// Error messages will be displayed if any check fails. The ListView updates after an item have been added.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddItem_Click(object sender, RoutedEventArgs e)
    {
        string name = txtNameOfTheItem.Text;
        bool isRequired = false;

        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Type in the name of the item");
            }
            else if (chbDocument.IsChecked is true)
            {
                if (chbRequired.IsChecked is true)
                {
                    isRequired = true;
                }

                TravelDocument travelDocument = new(name, isRequired);

                packingList.Add(travelDocument);
            }
            else if (chbDocument.IsChecked is false)
            {
                bool isQuantityAnInteger = int.TryParse(txtQuantity.Text, out int quantity);

                if (isQuantityAnInteger)
                {
                    OtherItem otherItem = new(name, quantity);
                    packingList.Add(otherItem);
                }
                else
                {
                    throw new ArgumentException("Type in the quantity of the item");
                }
            }

            UpdateListViewForPackingItems();

            txtNameOfTheItem.Clear();
            txtQuantity.Clear();
            chbDocument.IsChecked = false;
            chbRequired.IsChecked = false;
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }

    /// <summary>
    /// Method for the Document CheckBox, makes sure that the correct UI elements are visible if its "Checked".
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chbDocument_Checked(object sender, RoutedEventArgs e)
    {
        if (chbDocument.IsChecked is true)
        {
            lblQuantity.Visibility = Visibility.Collapsed;
            txtQuantity.Visibility = Visibility.Collapsed;
            chbRequired.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// Method for the Document CheckBox, makes sure that the correct UI elements are visible if its "Unchecked".
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chbDocument_Unchecked(object sender, RoutedEventArgs e)
    {
        if (chbDocument.IsChecked is false)
        {
            lblQuantity.Visibility = Visibility.Visible;
            txtQuantity.Visibility = Visibility.Visible;
            chbRequired.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Method for the selecting travel type in the ComboBox, makes sure that the correct UI elements are visible depending on which trip type that is selected.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cbTravelType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(cbTravelType.SelectedItem.ToString().Equals("Trip"))
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
    /// Method for selecting country in the ComboBox, auto-generates a passport. 
    /// Depending where the user is located and where the destination is located, it will set the passport as required or not.
    /// If the user decides to change country, then the passport will automatically have the required setting updated to reflect the new country.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cbCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        bool IsUserLocatedInEu = Enum.IsDefined(typeof(EuropeanCountries), signedInUser.Location.ToString()); // true, if user live in Eu
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

        if (lvPackingList.Items.Count == 0 || travelDocument == null)
        {
            travelDocument = new("Passport", isRequired);
            packingList.Add(travelDocument);
        }
        else
        {
            travelDocument.Required = isRequired;
        }

        UpdateListViewForPackingItems();
    }

    /// <summary>
    /// Method for populating the ListView with the items in the packing list.
    /// </summary>
    private void UpdateListViewForPackingItems()
    {
        lvPackingList.Items.Clear();

        foreach (IPackingListItem packingListItem in packingList)
        {
            ListViewItem item = new();
            item.Content = packingListItem.GetInfo();
            item.Tag = packingListItem;
            lvPackingList.Items.Add(item);
        }
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
