using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TravelPal.Interfaces;
using TravelPal.Managers;
using TravelPal.Models;

namespace TravelPal;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private UserManager userManager = new();
    private TravelManager travelManager = new();

    public MainWindow()
    {
        InitializeComponent();

        userManager.LoadDefaultUsers(travelManager);
    }

    //public MainWindow(UserManager userManager)
    //{
    //    InitializeComponent();

    //    this.userManager = userManager;
    //}

    public MainWindow(UserManager userManager, TravelManager travelManager)
    {
        InitializeComponent();

        this.userManager = userManager;
        this.travelManager = travelManager;
    }

    private void btnSignIn_Click(object sender, RoutedEventArgs e)
    {
        List<IUser> users = userManager.Users;

        string username = txtUsername.Text;
        string password = pwPassword.Password;

        bool isSignInSuccessful = userManager.SignInUser(username, password);

        if (isSignInSuccessful)
        {
            TravelsWindow travelsWindow = new(this.userManager, this.travelManager);
            travelsWindow.Show();
            Close();
        }
        else
        {
            lblSignInErrorMessage.Content = "Username or password is incorrect";
        }
    }

    private void btnRegister_Click(object sender, RoutedEventArgs e)
    {
        RegisterWindow registerWindow = new(this.userManager, this.travelManager);
        registerWindow.Show();
        Close();
    }
}
