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

namespace TravelPal;

/// <summary>
/// Interaction logic for UserDetailsWindow.xaml
/// </summary>
public partial class UserDetailsWindow : Window
{
    private UserManager userManager;
    private TravelsWindow travelsWindow;
    private IUser signedInUser;
    public UserDetailsWindow(UserManager userManager, TravelsWindow travelsWindow)
    {
        InitializeComponent();

        this.userManager = userManager;
        this.travelsWindow = travelsWindow;

        signedInUser = this.userManager.SignedInUser;

        txtUserName.Text = signedInUser.Username;

        cbCountries.ItemsSource = Enum.GetNames(typeof(Countries));
        cbCountries.Text = signedInUser.Location.ToString();

        pwPassword.Password = signedInUser.Password;
        pwConfirmPassword.Password = signedInUser.Password;     
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        string newUsername = txtUserName.Text;
        string newPassword = pwPassword.Password;
        string newConfirmPassword = pwConfirmPassword.Password;

        Countries newLocation = (Countries)Enum.Parse(typeof(Countries), cbCountries.SelectedItem.ToString());

        bool updatePassword = false;
        bool updateUsername = false;

        try
        {
            if (newUsername.Length < 3)
            {
                throw new ArgumentException("Username most be 3 or more characters.");
            }
            else
            {
                updateUsername = true;
            }

            if (newPassword.Length > 4)
            {
                if (newPassword.Equals(newConfirmPassword))
                {
                    updatePassword = true;
                }
                else
                {
                    throw new ArgumentException("Password does not match.");
                }
            }
            else
            {
                throw new ArgumentException("Password must be at least 5 characters.");
            }

            if (updatePassword && updateUsername)
            {
                bool isUsernameUpdated = userManager.UpdateUsername(signedInUser, newUsername);

                if (!isUsernameUpdated && !signedInUser.Username.Equals(newUsername))
                {
                    throw new ArgumentException("Username does already exist.");
                }
                else
                {
                    signedInUser.Location = newLocation;
                    signedInUser.Password = newPassword;
                    travelsWindow.UpdateUsernameLabel();
                    Close();
                }
            }
        }
        catch (ArgumentException ex)
        {
            lblErrorMessage.Content = ex.Message;
            lblErrorMessage.Visibility = Visibility.Visible;
        }
    }

    private bool UpdatePassword(string newPassword, string newConfirmPassword)
    {
        if (newPassword.Length > 4)
        {
            if (newPassword.Equals(newConfirmPassword))
            {
                signedInUser.Password = newPassword;

                return true;
            }
            else
            {
                throw new ArgumentException("Password does not match.");
            }
        }
        else
        {
            throw new ArgumentException("Password must be at least 5 characters.");
        }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
