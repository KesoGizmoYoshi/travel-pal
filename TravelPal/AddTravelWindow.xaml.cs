using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    private TravelManager travelManager = new();
    private UserManager userManager;

    private List<PackingListItem> packingList = new();
    public AddTravelWindow(UserManager userManager)
    {
        InitializeComponent();

        this.userManager = userManager;

        cbCountries.ItemsSource = Enum.GetNames(typeof(Countries));
        cbTravelType.ItemsSource = new[] { "Trip", "Vacation" };
        cbTripType.ItemsSource = Enum.GetNames(typeof(TripTypes));


    }

    private void btnAddTravel_Click(object sender, RoutedEventArgs e)
    {
        string destination = txtDestination.Text;
        Countries country = (Countries)Enum.Parse(typeof(Countries), cbCountries.SelectedItem.ToString());
        string travelers = txtTravelers.Text;
        string travelType = cbTravelType.SelectedItem.ToString();

        bool IsUserLocatedInEu = Enum.IsDefined(typeof(EuropeanCountries), userManager.SignedInUser.Location.ToString()); // true, if user live in Eu

        bool IsDestinationCountryInEu = Enum.IsDefined(typeof(EuropeanCountries), country.ToString()); // true, if destination country is in EU



        if (IsUserLocatedInEu && !IsDestinationCountryInEu)

        //EuropeanCountries test = (EuropeanCountries)Enum.Parse(typeof(EuropeanCountries), userManager.SignedInUser.Location.ToString());


        //       Om resan går till ett land utanför EU och användaren bor inom EU ska ett TravelDocument med name
        //       “Passport” (med required satt till true) automatiskt läggas till i packlistan

        //       Om resan går till ett annat land, inom EU och användaren bor inom EU, ska ett TravelDocument med
        //       name “Passport” (med required satt till false) automatiskt läggas till i packlistan

        //       Om användaren ändrar destinationsland under tiden ska required för passet ändras därefter

        //       Om användaren bor utanför EU, oavsett destinationsland, ska ett pass(med required true) läggas till
        //automatiskt

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
            lvPackingList.Items.Add(travelDocument.GetInfo());

        }
        else if(chbDocument.IsChecked is false)
        {
            int quantity = Convert.ToInt32(txtQuantity.Text);

            OtherItem otherItem = new(name, quantity);

            packingList.Add(otherItem);
            lvPackingList.Items.Add(otherItem.GetInfo());
        }

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
}
