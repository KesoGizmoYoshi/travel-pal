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

namespace TravelPal
{
    /// <summary>
    /// Interaction logic for UserDetailsWindow.xaml
    /// </summary>
    public partial class UserDetailsWindow : Window
    {
        private UserManager userManager = new();
        public UserDetailsWindow(UserManager userManager)
        {
            InitializeComponent();

            this.userManager = userManager;

            cbCountries.ItemsSource = Enum.GetNames(typeof(Countries));
            cbCountries.Text = userManager.SignedInUser.Location.ToString();

            txtUserName.Text = userManager.SignedInUser.Username;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = txtUserName.Text;
            
            Countries newLocation = (Countries)Enum.Parse(typeof(Countries), cbCountries.SelectedItem.ToString());
            userManager.SignedInUser.Location = newLocation;

            bool isUsernameUpdated = userManager.UpdateUsername(userManager.SignedInUser,newUsername);

            if(isUsernameUpdated)
            {
                MessageBox.Show("Yey");
            }
        }
    }
}
