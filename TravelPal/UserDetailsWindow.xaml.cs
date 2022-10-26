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
using TravelPal.Interfaces;
using TravelPal.Managers;

namespace TravelPal
{
    /// <summary>
    /// Interaction logic for UserDetailsWindow.xaml
    /// </summary>
    public partial class UserDetailsWindow : Window
    {
        private UserManager userManager = new();
        private TravelsWindow travelsWindow;
        private IUser signedInUser;
        public UserDetailsWindow(UserManager userManager , TravelsWindow travelsWindow)
        {
            InitializeComponent();

            this.userManager = userManager;
            this.travelsWindow = travelsWindow;
            signedInUser = userManager.SignedInUser;

            cbCountries.ItemsSource = Enum.GetNames(typeof(Countries));
            cbCountries.Text = userManager.SignedInUser.Location.ToString();

            txtUserName.Text = userManager.SignedInUser.Username;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = txtUserName.Text;

            string newPassword = pbPassword.Password;
            string newConfirmPassword = pbConfirmPassword.Password;

            bool isPasswordUpdated = UpdatePassword(newPassword, newConfirmPassword);
            
            Countries newLocation = (Countries)Enum.Parse(typeof(Countries), cbCountries.SelectedItem.ToString());
            signedInUser.Location = newLocation;

            bool isUsernameUpdated = userManager.UpdateUsername(signedInUser,newUsername);

            if(isUsernameUpdated)
            {
                travelsWindow.UpdateUsernameLabel();
                Close();
            }
            else
            {
                
            }

            
        }

        private bool UpdatePassword(string newPassword, string newConfirmPassword)
        {
            if (!string.IsNullOrEmpty(newPassword))
            {
                if (!(newPassword.Length < 5))
                {
                    if (newPassword.Equals(newConfirmPassword))
                    {
                        signedInUser.Password = newPassword;

                        return true;
                    }
                    else
                    {
                        throw new ArgumentException("Password does not match");
                    }
                }
                else
                {
                    throw new ArgumentException("Password must be at least 5 characters");
                }
            }

            return false;
        }

    }
}
