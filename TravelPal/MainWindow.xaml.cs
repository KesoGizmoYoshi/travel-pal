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

    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(UserManager userManager)
    {
        InitializeComponent();

        this.userManager = userManager;
    }

    private void btnSignIn_Click(object sender, RoutedEventArgs e)
    {
        List<IUser> users = userManager.Users;

        string username = txtUsername.Text;
        string password = pbPassword.Password;

        bool isSignInSuccessful = userManager.SignInUser(username, password);

        if (isSignInSuccessful)
        {
            TravelsWindow travelsWindow = new(userManager);
            travelsWindow.Show();
            Close();
        }
        else
        {
            // label istället
            MessageBox.Show("Username or password is incorrect", "Warning");
        }

        //foreach (IUser user in users)
        //{
        //    if (user.Username == username && user.Password == password)
        //    {
        //        isFoundUser = true;

        //        userManager.SignedInUser = user;

        //        TravelsWindow travelsWindow = new(userManager);
        //        travelsWindow.Show();
        //    }
        //}

    }

    private void btnRegister_Click(object sender, RoutedEventArgs e)
    {
        RegisterWindow registerWindow = new(userManager);
        registerWindow.Show();
        Close();
    }
}
