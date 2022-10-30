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

        PopulateAllComboBoxes();
    }

    private void PopulateAllComboBoxes()
    {
        cbCountries.ItemsSource = Enum.GetNames(typeof(Countries));
        cbTravelType.ItemsSource = new[] { "Trip", "Vacation" };
        cbTripType.ItemsSource = Enum.GetNames(typeof(TripTypes));
    }

    private void btnAddTravel_Click(object sender, RoutedEventArgs e)
    {
        string destination = txtDestination.Text;
        Countries country = (Countries)Enum.Parse(typeof(Countries), cbCountries.SelectedItem.ToString());
        string strTravelers = txtTravelers.Text;
        string travelType = cbTravelType.SelectedItem.ToString();

        DateTime startDate = (DateTime)datePickerStartDate.SelectedDate;
        DateTime endDate = (DateTime)datePickerEndDate.SelectedDate;

        string[] inputsToNullCheck = { destination, country.ToString(), strTravelers, travelType, startDate.ToString(), endDate.ToString() };

        int index = 0;

        foreach(string input in inputsToNullCheck)
        {
            if (string.IsNullOrWhiteSpace(input)){

                throw new ArgumentException($"Field for {inputsToNullCheck[index]} can not be left empty!");
            }

            index++;
        }

        try
        {
            
        }
        catch (ArgumentException)
        {
            
        }
        //string destination = txtDestination.Text;
        //Countries country = (Countries)Enum.Parse(typeof(Countries), cbCountries.SelectedItem.ToString());
        //string strTravelers = txtTravelers.Text;
        //string travelType = cbTravelType.SelectedItem.ToString();

        //int travellers = Convert.ToInt32(strTravelers);

        //DateTime startDate = (DateTime)datePickerStartDate.SelectedDate;
        //DateTime endDate = (DateTime)datePickerEndDate.SelectedDate;

        //int travelDays = travelManager.CalculateTravelDays(startDate, endDate);

        //if (travelType.Equals("Trip"))
        //{
        //    TripTypes tripType = (TripTypes)Enum.Parse(typeof(TripTypes),cbTripType.SelectedItem.ToString());

        //    signedInUser.Travels.Add(travelManager.AddTravel(destination, country, travellers, packingList, startDate, endDate, tripType));
        //}
        //else if (travelType.Equals("Vacation"))
        //{
        //    bool isAllInclusive = (bool)chbAllInclusive.IsChecked;

        //    signedInUser.Travels.Add(travelManager.AddTravel(destination, country, travellers, packingList, startDate, endDate, isAllInclusive));
        //}

        //this.travelsWindow.DisplayTravels();
        //Close();
    }

    private void btnAddItem_Click(object sender, RoutedEventArgs e)
    {
        string name = txtNameOfTheItem.Text;
        bool isRequired = false;

        if (chbDocument.IsChecked is true)
        {
            if(chbRequired.IsChecked is true)
            {
                isRequired = true;
            }

            TravelDocument travelDocument = new(name, isRequired);

            packingList.Add(travelDocument);
        }
        else if(chbDocument.IsChecked is false)
        {
            int quantity = Convert.ToInt32(txtQuantity.Text);

            OtherItem otherItem = new(name, quantity);

            packingList.Add(otherItem);
        }

        UpdateListViewForPackingItems();

        txtNameOfTheItem.Clear();
        txtQuantity.Clear();
        chbDocument.IsChecked = false;
        chbRequired.IsChecked = false;
    }

    private void chbDocument_Checked(object sender, RoutedEventArgs e)
    {
        if (chbDocument.IsChecked is true)
        {
            lblQuantity.Visibility = Visibility.Collapsed;
            txtQuantity.Visibility = Visibility.Collapsed;
            chbRequired.Visibility = Visibility.Visible;
        }
    }

    private void chbDocument_Unchecked(object sender, RoutedEventArgs e)
    {
        if (chbDocument.IsChecked is false)
        {
            lblQuantity.Visibility = Visibility.Visible;
            txtQuantity.Visibility = Visibility.Visible;
            chbRequired.Visibility = Visibility.Collapsed;
        }
    }

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

        if (lvPackingList.Items.Count == 0)
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

    private void UpdateListViewForPackingItems()
    {
        lvPackingList.Items.Clear();

        foreach (IPackingListItem packingListItem in packingList)
        {
            ListViewItem item = new();
            item.Content = packingListItem.GetInfo();
            item.Tag = packingListItem;
            lvPackingList.Items.Add(item);

            //if (packingListItem is TravelDocument)
            //{
            //    TravelDocument travelDocument = (TravelDocument)packingListItem;
            //    ListViewItem item = new();
            //    item.Content = travelDocument.GetInfo();
            //    item.Tag = travelDocument;
            //    lvPackingList.Items.Add(item);
            //}
            //else if (packingListItem is OtherItem)
            //{
            //    OtherItem otherItem = (OtherItem)packingListItem;
            //    ListViewItem item = new();
            //    item.Content = otherItem.GetInfo();
            //    item.Tag = otherItem;
            //    lvPackingList.Items.Add(item);
            //}

        }
    }

    private void datePickerStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void datePickerEndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        DateTime startDate = (DateTime)datePickerStartDate.SelectedDate;
        DateTime endDate = (DateTime)datePickerEndDate.SelectedDate;

        int travelDays = (int)(endDate - startDate).TotalDays;

        lblTravelDays.Content += travelDays.ToString();
    }
}
