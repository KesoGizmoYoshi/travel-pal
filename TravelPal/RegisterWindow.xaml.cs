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
using TravelPal.Models;

namespace TravelPal;

/// <summary>
/// Interaction logic for RegisterWindow.xaml
/// </summary>
public partial class RegisterWindow : Window
{
    private UserManager userManager;
    private TravelManager travelManager;
    public RegisterWindow(UserManager userManager, TravelManager travelManager)
    {
        InitializeComponent();

        this.userManager = userManager;
        this.travelManager = travelManager;

        cbLocation.ItemsSource = Enum.GetNames(typeof(Countries));
    }

    private void btnRegister_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string username = txtUsername.Text;
            string password = pwPassword.Password;
            string confirmPassword = pwConfirmPassword.Password;
            string selectedLocation = (string)cbLocation.SelectedItem;

            if (username.Length < 3)
            {
                throw new ArgumentException("At least 3 chars (Username)");
            }
            else if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword)) 
            {
                throw new ArgumentException("Type in a password");
            }
            else if (password.Length < 5)
            {
                throw new ArgumentException("At least 5 chars (Password)");
            }
            else if (!password.Equals(confirmPassword) || password.Length < 5)
            {
                throw new ArgumentException("Password is not matching");
            }
            else if(selectedLocation is null)
            {
                throw new ArgumentException("No location selected");
            }

            Countries location = (Countries)Enum.Parse(typeof(Countries), selectedLocation);

            bool isUserAdded = this.userManager.AddUser(new User(username, password, location));

            if(isUserAdded)
            {
                MainWindow mainWindow = new MainWindow(this.userManager, this.travelManager);
                mainWindow.Show();
                Close();
            }
            else
            {
                throw new ArgumentException("Username does already exist!");
            }
        }
        catch (ArgumentException ex) 
        {
            lblRegisterErrorMessage.Visibility = Visibility.Visible;
            lblRegisterErrorMessage.Content = ex.Message;
        }
    }
}
