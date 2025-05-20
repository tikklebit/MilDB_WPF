using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Win32;

namespace MilDB_WPF;

public partial class Menu : Window
{
    private ObservableCollection<MilitaryOffice> _offices = new ObservableCollection<MilitaryOffice>();
    private string _officesFile;

    public Menu() 
    {
        InitializeComponent();
        _officesFile = "offices.json";
    }

    public Menu(string officesFile)
    {
        InitializeComponent();
        _officesFile = officesFile;
        LoadOffices();
    }

    private void OpenOfficeButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "JSON файли (*.json)|*.json|Усі файли (*.*)|*.*",
            DefaultExt = "json",
            Title = "Відкрити ТЦК"
        };
        if (dialog.ShowDialog() == true)
        {
            _officesFile = dialog.FileName;
            var offices = MilitaryOfficeService.LoadAll(_officesFile);
            if (offices.Count == 0)
            {
                MessageBox.Show("Файл не містить жодного ТЦК!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Menu menu = new Menu(_officesFile);
            menu.Show();
            this.Close();
        }
    }

    private void LoadOffices()
    {
        _offices = MilitaryOfficeService.LoadAll(_officesFile);
        OfficeComboBox.ItemsSource = _offices;
        OfficeComboBox.DisplayMemberPath = "Name";
    }

    private void SaveOffices()
    {
        MilitaryOfficeService.SaveAll(_offices, _officesFile);
    }

    private void SelectOfficeButton_Click(object sender, RoutedEventArgs e)
    {
        if (OfficeComboBox.SelectedItem is MilitaryOffice selectedOffice)
        {
            MainWindow main = new MainWindow(selectedOffice, _officesFile);
            main.Show();
            this.Hide();
        }
        else
        {
            MessageBox.Show("Оберіть ТЦК зі списку!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void CreateOfficeButton_Click(object sender, RoutedEventArgs e)
    {
        var newOfficeWindow = new NewMilitaryOfficeWindow();
        newOfficeWindow.OfficeCreated += (s, office) =>
        {
            _offices.Add(office);
            SaveOffices();
            LoadOffices();
            OfficeComboBox.SelectedItem = office;
        };
        newOfficeWindow.ShowDialog();
    }
}
