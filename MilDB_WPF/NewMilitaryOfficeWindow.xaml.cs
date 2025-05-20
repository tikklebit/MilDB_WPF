using System;
using System.Text.RegularExpressions;
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

        if (string.IsNullOrWhiteSpace(name))
        {
            ShowError("Назва ТЦК не може бути порожньою!");
            return;
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            ShowError("Адреса не може бути порожньою!");
            return;
        }

        if (string.IsNullOrWhiteSpace(serviceArea))
        {
            ShowError("Район обслуговування не може бути порожнім!");
            return;
        }

        if (ContainsDigits(name))
        {
            ShowError("Назва ТЦК не може містити цифри!");
            return;
        }

        if (ContainsDigits(serviceArea))
        {
            ShowError("Район обслуговування не може містити цифри!");
            return;
        }

        var office = new MilitaryOffice(name, address, serviceArea);

        if (office.Valid())
        {
            OfficeCreated?.Invoke(this, office);
            DialogResult = true;
            Close();
        }
        else
        {
            ShowError("Введені дані некоректні!");
        }
    }

    private void ShowError(string message)
    {
        ErrorLabel.Content = message;
        ErrorLabel.Foreground = new SolidColorBrush(Colors.Red);
        ErrorLabel.Visibility = Visibility.Visible;
    }

    private static bool ContainsDigits(string text)
    {
        return Regex.IsMatch(text, @"\d");
    }
}