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
        SearchTextBox.Text = string.Empty;
        DataGrid.AutoGenerateColumns = false;
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
        SearchTextBox.Text = string.Empty;
        OfficersDataGrid.AutoGenerateColumns = false;
        CfODataGrid.AutoGenerateColumns = false;
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

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (showingConscripts)
        {
            var window = new NewConscriptWindow(currentOffice);
            if (window.ShowDialog() == true && window.CreatedConscript != null && window.CreatedConscript.Valid())
            {
                currentOffice.Conscripts.Add(window.CreatedConscript);
                SaveAllOffices();
                DataGrid.ItemsSource = currentOffice.Conscripts;
            }
        }
        else
        {
            var window = new NewOfficerWindow(currentOffice);
            if (window.ShowDialog() == true && window.CreatedOfficer != null && window.CreatedOfficer.Valid())
            {
                currentOffice.Officers.Add(window.CreatedOfficer);
                SaveAllOffices();
                OfficersDataGrid.ItemsSource = currentOffice.Officers;
            }
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
                DataGrid.ItemsSource = currentOffice.Conscripts;
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
                OfficersDataGrid.ItemsSource = currentOffice.Officers;
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
            DataGrid.ItemsSource = currentOffice.Conscripts;
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
            OfficersDataGrid.ItemsSource = currentOffice.Officers;
            CfODataGrid.ItemsSource = null;
        }
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string search = SearchTextBox.Text.Trim().ToLower();
        if (showingConscripts)
        {
            if (string.IsNullOrEmpty(search))
                DataGrid.ItemsSource = currentOffice.Conscripts;
            else
                DataGrid.ItemsSource = new ObservableCollection<Conscript>(
                    currentOffice.Conscripts.Where(c => c.FullName.ToLower().Contains(search)));
        }
        else
        {
            if (string.IsNullOrEmpty(search))
                OfficersDataGrid.ItemsSource = currentOffice.Officers;
            else
                OfficersDataGrid.ItemsSource = new ObservableCollection<Officer>(
                    currentOffice.Officers.Where(o => o.FullName.ToLower().Contains(search)));
        }
    }

    private void SortButton_Click(object sender, RoutedEventArgs e)
    {
        if (SortComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            string? sortCriteria = selectedItem.Tag as string;

            if (string.IsNullOrEmpty(sortCriteria))
            {
                MessageBox.Show("Оберіть критерій сортування.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (showingConscripts)
            {
                var conscriptsList = currentOffice.Conscripts.ToList();

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
                    default:
                        MessageBox.Show($"Непідтримуваний критерій сортування для призовників: {sortCriteria}", "Помилка сортування", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                DataGrid.ItemsSource = new ObservableCollection<Conscript>(conscriptsList);
            }
            else
            {
                var officersList = currentOffice.Officers.ToList();
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
                    default:
                        MessageBox.Show($"Непідтримуваний критерій сортування для офіцерів: {sortCriteria}", "Помилка сортування", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                OfficersDataGrid.ItemsSource = new ObservableCollection<Officer>(officersList);
            }

            if (showingConscripts) DataGrid.Items.Refresh();
            else OfficersDataGrid.Items.Refresh();
        }
        else
        {
            MessageBox.Show("Оберіть критерій сортування.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
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
            var offices = MilitaryOfficeService.LoadAll(officesFile);
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