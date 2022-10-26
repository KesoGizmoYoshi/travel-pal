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
    private UserManager userManager = new();
    public RegisterWindow(UserManager userManager)
    {
        InitializeComponent();

        this.userManager = userManager;

        cbCountries.ItemsSource = Enum.GetNames(typeof(Countries));
    }

    private void btnRegister_Click(object sender, RoutedEventArgs e)
    {
        string username = txtUsername.Text;
        string password = pbPassword.Password;
        string confirmPassword = pbConfirmPassword.Password;
        string selectedLocation = cbCountries.SelectedItem.ToString();

        try
        {

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Type in a usernam");
            }
            else if(username.Length < 3)
            {
                throw new ArgumentException("Atleast 3 chars");
            }
            else if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword)) 
            {
                throw new ArgumentException("Type in a password");
            }
            else if (!password.Equals(confirmPassword))
            {
                throw new ArgumentException("Password is not matching");
            }
            else if(string.IsNullOrEmpty(selectedLocation))
            {
                throw new ArgumentException("No location selected");
            }

            Countries location = (Countries)Enum.Parse(typeof(Countries), selectedLocation);

            User newUser = new(username, password, location);

            bool isUserAdded = this.userManager.AddUser(newUser);

            if(isUserAdded)
            {
                MainWindow mainWindow = new MainWindow(this.userManager);
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
