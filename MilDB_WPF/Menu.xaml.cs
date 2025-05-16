using System.Collections.ObjectModel;
using System.Windows;

namespace MilDB_WPF;

public partial class Menu : Window
{
    private ObservableCollection<MilitaryOffice> _offices = new ObservableCollection<MilitaryOffice>();
    private readonly string _officesFile;

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

    private void LoadOffices()
    {
        _offices = MilitaryOfficeService.LoadAll(_officesFile);
        OfficeComboBox.ItemsSource = _offices;
        OfficeComboBox.DisplayMemberPath = "Name";
    }

    private void SaveOffices()
    {
        MilitaryOfficeService.SaveAll(_offices);
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
