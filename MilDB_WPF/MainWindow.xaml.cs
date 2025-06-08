using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace MilDB_WPF;

public partial class MainWindow : Window
{
    private readonly MilitaryOffice currentOffice;
    private string officesFile;
    private bool showingConscripts = true;

    public MainWindow(MilitaryOffice office, string officesFile)
    {
        InitializeComponent();
        currentOffice = office;
        this.officesFile = officesFile;
        Title.Content = $"ТАБЛИЦЯ - {currentOffice.Name}";
        ShowConscripts();
        OfficersDataGrid.SelectionChanged += OfficersDataGrid_SelectionChanged;
        UpdateFilterComboBox();
        UpdateSortComboBox();
    }

    private void UpdateFilterComboBox()
    {
        if (showingConscripts)
        {
            foreach (ComboBoxItem item in FilterTypeComboBox.Items)
            {
                string tag = item.Tag as string;
                item.Visibility = (tag == "FullName" || tag == "Address" || tag == "FitnessCategory" || tag == "Status")
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            if (FilterTypeComboBox.SelectedItem is ComboBoxItem selected &&
                (selected.Tag as string == "Rank" || selected.Tag as string == "YearsOfService"))
            {
                FilterTypeComboBox.SelectedIndex = 0;
            }
        }
        else
        {
            foreach (ComboBoxItem item in FilterTypeComboBox.Items)
            {
                string tag = item.Tag as string;
                item.Visibility = (tag == "FullName" || tag == "Rank" || tag == "YearsOfService")
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            if (FilterTypeComboBox.SelectedItem is ComboBoxItem selected &&
                (selected.Tag as string == "Address" || selected.Tag as string == "FitnessCategory" || selected.Tag as string == "Status"))
            {
                FilterTypeComboBox.SelectedIndex = 0;
            }
        }
    }

    private void UpdateSortComboBox()
    {
        if (SortComboBox == null) return;

        if (showingConscripts)
        {
            foreach (ComboBoxItem item in SortComboBox.Items)
            {
                string tag = item.Tag as string;
                item.Visibility = (tag == "Name" || tag == "Address" || tag == "BirthDate")
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            if (SortComboBox.SelectedItem is ComboBoxItem selected &&
                (selected.Tag as string == "YearsOfService" || selected.Tag as string == "Rank"))
            {
                SortComboBox.SelectedIndex = 0;
            }
        }
        else
        {
            foreach (ComboBoxItem item in SortComboBox.Items)
            {
                string tag = item.Tag as string;
                item.Visibility = (tag == "Name" || tag == "YearsOfService" || tag == "Rank")
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            if (SortComboBox.SelectedItem is ComboBoxItem selected &&
                (selected.Tag as string == "Address" || selected.Tag as string == "BirthDate"))
            {
                SortComboBox.SelectedIndex = 0;
            }
        }
    }

    private void ShowConscripts()
    {
        DataGrid.Visibility = Visibility.Visible;
        DataGrid.IsEnabled = true;
        OfficersSP.Visibility = Visibility.Collapsed;
        OfficersSP.IsEnabled = false;
        DataGrid.ItemsSource = currentOffice.Conscripts;
        DataGrid.Columns.Clear();

        DataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "ПІБ",
            Binding = new System.Windows.Data.Binding("FullName"),
            Width = new DataGridLength(2, DataGridLengthUnitType.Star)
        });
        DataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Дата народження",
            Binding = new System.Windows.Data.Binding("BirthDate") { StringFormat = "d" },
            Width = new DataGridLength(1, DataGridLengthUnitType.Star)
        });
        DataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Адреса",
            Binding = new System.Windows.Data.Binding("Address"),
            Width = new DataGridLength(2, DataGridLengthUnitType.Star)
        });
        DataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Стан здоров'я",
            Binding = new System.Windows.Data.Binding("HealthStatus"),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star)
        });
        DataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Категорія придатності",
            Binding = new System.Windows.Data.Binding("FitnessCategory"),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star)
        });
        DataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Статус",
            Binding = new System.Windows.Data.Binding("Status"),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star)
        });

        showingConscripts = true;
        FilterTextBox.Text = string.Empty;
        DataGrid.AutoGenerateColumns = false;
        OpenedTable.Content = "ТАБЛИЦЯ ПРИЗОВНИКІВ";
        UpdateFilterComboBox();
        UpdateSortComboBox();
    }

    private void ShowOfficers()
    {
        DataGrid.Visibility = Visibility.Collapsed;
        DataGrid.IsEnabled = false;
        OfficersSP.Visibility = Visibility.Visible;
        OfficersSP.IsEnabled = true;
        OfficersDataGrid.ItemsSource = currentOffice.Officers;
        OfficersDataGrid.Columns.Clear();

        OfficersDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "ПІБ",
            Binding = new System.Windows.Data.Binding("FullName"),
            Width = new DataGridLength(2, DataGridLengthUnitType.Star)
        });
        OfficersDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Звання",
            Binding = new System.Windows.Data.Binding("Rank"),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star)
        });
        OfficersDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Стаж (роки)",
            Binding = new System.Windows.Data.Binding("YearsOfService"),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star)
        });

        CfODataGrid.ItemsSource = null;
        CfODataGrid.Columns.Clear();

        CfODataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "ПІБ",
            Binding = new System.Windows.Data.Binding("FullName"),
            Width = new DataGridLength(2, DataGridLengthUnitType.Star)
        });
        CfODataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Дата народження",
            Binding = new System.Windows.Data.Binding("BirthDate") { StringFormat = "d" },
            Width = new DataGridLength(1, DataGridLengthUnitType.Star)
        });
        CfODataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Адреса",
            Binding = new System.Windows.Data.Binding("Address"),
            Width = new DataGridLength(2, DataGridLengthUnitType.Star)
        });
        CfODataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Стан здоров'я",
            Binding = new System.Windows.Data.Binding("HealthStatus"),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star)
        });
        CfODataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Категорія придатності",
            Binding = new System.Windows.Data.Binding("FitnessCategory"),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star)
        });
        CfODataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Статус",
            Binding = new System.Windows.Data.Binding("Status"),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star)
        });

        showingConscripts = false;
        FilterTextBox.Text = string.Empty;
        OfficersDataGrid.AutoGenerateColumns = false;
        CfODataGrid.AutoGenerateColumns = false;
        OpenedTable.Content = "ТАБЛИЦЯ ОФІЦЕРІВ";
        UpdateFilterComboBox();
        UpdateSortComboBox();
    }

    private void OfficersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (OfficersDataGrid.SelectedItem is Officer officer)
        {
            CfODataGrid.ItemsSource = officer.AssignedConscripts;
        }
        else
        {
            CfODataGrid.ItemsSource = null;
        }
    }

    private void ConscriptTableButton_Click(object sender, RoutedEventArgs e) => ShowConscripts();

    private void OfficerTableButton_Click(object sender, RoutedEventArgs e) => ShowOfficers();

    private void AddConscriptButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new NewConscriptWindow(currentOffice);
        if (window.ShowDialog() == true && window.CreatedConscript != null && window.CreatedConscript.Valid())
        {
            currentOffice.Conscripts.Add(window.CreatedConscript);
            SaveAllOffices();
            FilterTextBox_TextChanged(null, null);
        }
    }

    private void AddOfficerButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new NewOfficerWindow(currentOffice);
        if (window.ShowDialog() == true && window.CreatedOfficer != null && window.CreatedOfficer.Valid())
        {
            currentOffice.Officers.Add(window.CreatedOfficer);
            SaveAllOffices();
            FilterTextBox_TextChanged(null, null);
        }
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (showingConscripts)
        {
            if (DataGrid.SelectedItem is not Conscript conscript)
            {
                MessageBox.Show("Оберіть елемент для редагування!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var window = new NewConscriptWindow(conscript, currentOffice);
            if (window.ShowDialog() == true && window.CreatedConscript != null && window.CreatedConscript.Valid())
            {
                int idx = currentOffice.Conscripts.IndexOf(conscript);
                if (idx >= 0)
                    currentOffice.Conscripts[idx] = window.CreatedConscript;
                SaveAllOffices();
                FilterTextBox_TextChanged(null, null);
            }
        }
        else
        {
            if (OfficersDataGrid.SelectedItem is not Officer officer)
            {
                MessageBox.Show("Оберіть елемент для редагування!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var window = new NewOfficerWindow(officer, currentOffice);
            if (window.ShowDialog() == true && window.CreatedOfficer != null && window.CreatedOfficer.Valid())
            {
                int idx = currentOffice.Officers.IndexOf(officer);
                if (idx >= 0)
                    currentOffice.Officers[idx] = window.CreatedOfficer;
                SaveAllOffices();
                FilterTextBox_TextChanged(null, null);
                CfODataGrid.ItemsSource = officer.AssignedConscripts;
            }
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (showingConscripts)
        {
            if (DataGrid.SelectedItem is not Conscript conscript)
            {
                MessageBox.Show("Оберіть елемент для видалення!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            currentOffice.Conscripts.Remove(conscript);
            SaveAllOffices();
            FilterTextBox_TextChanged(null, null);
        }
        else
        {
            if (OfficersDataGrid.SelectedItem is not Officer officer)
            {
                MessageBox.Show("Оберіть елемент для видалення!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            currentOffice.Officers.Remove(officer);
            SaveAllOffices();
            FilterTextBox_TextChanged(null, null);
            CfODataGrid.ItemsSource = null;
        }
    }

    private ObservableCollection<Conscript> SortConscripts(List<Conscript> conscriptsList, string sortCriteria)
    {
        if (string.IsNullOrEmpty(sortCriteria))
        {
            return new ObservableCollection<Conscript>(conscriptsList);
        }

        switch (sortCriteria)
        {
            case "Name":
                conscriptsList = conscriptsList.OrderBy(c => c.FullName).ToList();
                break;
            case "Address":
                conscriptsList = conscriptsList.OrderBy(c => c.Address).ToList();
                break;
            case "BirthDate":
                conscriptsList = conscriptsList.OrderBy(c => c.BirthDate).ToList();
                break;
        }
        return new ObservableCollection<Conscript>(conscriptsList);
    }

    private ObservableCollection<Officer> SortOfficers(List<Officer> officersList, string sortCriteria)
    {
        if (string.IsNullOrEmpty(sortCriteria))
        {
            return new ObservableCollection<Officer>(officersList);
        }

        switch (sortCriteria)
        {
            case "Name":
                officersList = officersList.OrderBy(o => o.FullName).ToList();
                break;
            case "YearsOfService":
                officersList = officersList.OrderBy(o => o.YearsOfService).ToList();
                break;
            case "Rank":
                officersList = officersList.OrderBy(o => o.Rank).ToList();
                break;
        }
        return new ObservableCollection<Officer>(officersList);
    }

    private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string sortCriteria = null;
        if (SortComboBox?.SelectedItem is ComboBoxItem sortSelectedItem && sortSelectedItem.Tag is string tag)
        {
            sortCriteria = tag;
        }

        if (FilterTypeComboBox == null || FilterTextBox == null || FilterTypeComboBox.SelectedItem is not ComboBoxItem selectedItem ||
            selectedItem.Tag == null || currentOffice == null || DataGrid == null || OfficersDataGrid == null)
        {
            if (showingConscripts && DataGrid != null)
            {
                var conscripts = currentOffice?.Conscripts ?? new ObservableCollection<Conscript>();
                DataGrid.ItemsSource = SortConscripts(conscripts.ToList(), sortCriteria);
            }
            else if (!showingConscripts && OfficersDataGrid != null)
            {
                var officers = currentOffice?.Officers ?? new ObservableCollection<Officer>();
                OfficersDataGrid.ItemsSource = SortOfficers(officers.ToList(), sortCriteria);
            }
            return;
        }

        string criteria = selectedItem.Tag as string;
        string filterValue = FilterTextBox.Text.Trim();

        if (showingConscripts)
        {
            var filteredConscripts = currentOffice?.Conscripts != null
                ? currentOffice.FilterConscripts(criteria, filterValue)
                : new ObservableCollection<Conscript>();
            DataGrid.ItemsSource = SortConscripts(filteredConscripts.ToList(), sortCriteria);
        }
        else
        {
            var filteredOfficers = currentOffice?.Officers != null
                ? currentOffice.FilterOfficers(criteria, filterValue)
                : new ObservableCollection<Officer>();
            OfficersDataGrid.ItemsSource = SortOfficers(filteredOfficers.ToList(), sortCriteria);
        }
    }

    private void FilterTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        FilterTextBox_TextChanged(sender, null);
    }

    private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        FilterTextBox_TextChanged(sender, null);
    }

    private void SortButton_Click(object sender, RoutedEventArgs e)
    {
        if (SortComboBox.SelectedItem is not ComboBoxItem selectedItem || selectedItem.Tag is not string sortCriteria || string.IsNullOrEmpty(sortCriteria))
        {
            MessageBox.Show("Оберіть критерій сортування.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        FilterTextBox_TextChanged(SortComboBox, null);
    }

    private void SaveAllOffices()
    {
        var offices = MilitaryOfficeService.LoadAll(officesFile);
        var idx = offices.ToList().FindIndex(o => o.Name == currentOffice.Name && o.Address == currentOffice.Address);
        if (idx >= 0)
            offices[idx] = currentOffice;
        else
            offices.Add(currentOffice);
        MilitaryOfficeService.SaveAll(offices, officesFile);
    }

    private void SaveOfficeButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "JSON файли (*.json)|*.json|Усі файли (*.*)|*.*",
            DefaultExt = "json",
            Title = "Зберегти ТЦК"
        };
        if (dialog.ShowDialog() == true)
        {
            MilitaryOfficeService.SaveAll(new ObservableCollection<MilitaryOffice> { currentOffice }, dialog.FileName);
            MessageBox.Show("ТЦК збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            MessageBox.Show(dialog.FileName);
        }
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
            var offices = MilitaryOfficeService.LoadAll(dialog.FileName);
            if (offices.Count == 0)
            {
                MessageBox.Show("Файл не містить жодного ТЦК!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var menu = new Menu(dialog.FileName);
            menu.Show();
            this.Close();
        }
    }

    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void HideButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
}