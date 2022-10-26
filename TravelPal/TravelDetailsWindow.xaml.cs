using System;
using System.Collections.Generic;
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
using TravelPal.Managers;
using TravelPal.Models;

namespace TravelPal;

/// <summary>
/// Interaction logic for TravelDetailsWindow.xaml
/// </summary>
public partial class TravelDetailsWindow : Window
{
    public TravelDetailsWindow(UserManager userManager, Travel travel)
    {
        InitializeComponent();

        txtDestination.Text = travel.Destination;

        cbCountries.ItemsSource = Enum.GetNames(typeof(Countries));
        cbCountries.Text = travel.Country.ToString();

        txtTravelers.Text = travel.Travellers.ToString();

        cbTravelType.ItemsSource = new[]{ "Trip", "Vacation"};

        cbTripType.ItemsSource = Enum.GetNames(typeof(TripTypes));

        if (travel is Trip)
        {
            Trip trip = (Trip)travel;

            cbTravelType.Text = "Trip";
            cbTripType.Text = trip.TripType.ToString();

        }
        else if (travel is Vacation)
        {
            Vacation vacation = (Vacation)travel;

            cbTravelType.Text = "Vacation";

            if (vacation.AllInclusive)
            {
                chbAllInclusive.IsChecked = true;
            }
        }

    }
}
