using System;
using System.Windows;
using System.Windows.Media;

namespace MilDB_WPF;

public partial class NewMilitaryOfficeWindow : Window
{
    public event EventHandler<MilitaryOffice>? OfficeCreated;

    public NewMilitaryOfficeWindow()
    {
        InitializeComponent();
        ErrorLabel.Visibility = Visibility.Collapsed;
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        string name = NameTextBox.Text.Trim();
        string address = AddressTextBox.Text.Trim();
        string serviceArea = ServiceAreaTextBox.Text.Trim();

        var office = new MilitaryOffice(name, address, serviceArea);

        if (office.Valid())
        {
            OfficeCreated?.Invoke(this, office);
            DialogResult = true;
            Close();
        }
        else
        {
            ErrorLabel.Content = "Введені дані некоректні!";
            ErrorLabel.Foreground = new SolidColorBrush(Colors.Red);
            ErrorLabel.Visibility = Visibility.Visible;
        }
    }
}
