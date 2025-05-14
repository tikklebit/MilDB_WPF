using System.Windows;
using System.Windows.Controls;

namespace MilDB_WPF;

public partial class Menu : Window
{
    public Menu()
    {
        InitializeComponent();
    }

    private void AdminButton_Click(object sender, EventArgs e)
    {
        AdminButton.Visibility = Visibility.Collapsed;
        AdminButton.IsEnabled = false;
        UserButton.Visibility = Visibility.Collapsed;
        UserButton.IsEnabled = false;
        
        AdminPassword.Visibility = Visibility.Visible;
        AdminPassword.IsEnabled = true;
        ConfirmButton.Visibility = Visibility.Visible;
        ConfirmButton.IsEnabled = true;
    }

    private void AcceptButton_Click(object sender, EventArgs e)
    {
        if (AdminPassword.Password == "admin")
        {
            this.Hide(); 
            MainWindow main = new MainWindow();
            main.Show();
        }
        else
        {
            MessageBox.Show("Неправильний пароль!");
            AdminPassword.Password = String.Empty;
        }
    }
    
    private void UserButton_Click(object sender, EventArgs e)
    {
        this.Hide(); 
        MainWindow main = new MainWindow();
        main.Show();
    }
}