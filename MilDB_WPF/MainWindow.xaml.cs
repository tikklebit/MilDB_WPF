using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Microsoft.Win32;

namespace MilDB_WPF;

public partial class MainWindow : Window
{
    private readonly MilitaryOffice currentOffice;
    private bool showingConscripts = true;

    public MainWindow(MilitaryOffice office)
    {
        InitializeComponent();
        currentOffice = office;
        Title.Content = $"ТАБЛИЦЯ - {currentOffice.Name}";
        ShowConscripts();
    }

    private void ShowConscripts()
    {
        DataGrid.ItemsSource = currentOffice.Conscripts;
        showingConscripts = true;
        SearchTextBox.Text = string.Empty;
    }

    private void ShowOfficers()
    {
        DataGrid.ItemsSource = currentOffice.Officers;
        showingConscripts = false;
        SearchTextBox.Text = string.Empty;
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
            }
        }
        else
        {
            var window = new NewOfficerWindow(currentOffice);
            if (window.ShowDialog() == true && window.CreatedOfficer != null && window.CreatedOfficer.Valid())
            {
                currentOffice.Officers.Add(window.CreatedOfficer);
            }
        }
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataGrid.SelectedItem == null)
        {
            MessageBox.Show("Оберіть елемент для редагування!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (showingConscripts && DataGrid.SelectedItem is Conscript conscript)
        {
            var window = new NewConscriptWindow(conscript, currentOffice);
            if (window.ShowDialog() == true && window.CreatedConscript != null && window.CreatedConscript.Valid())
            {
                int idx = currentOffice.Conscripts.IndexOf(conscript);
                if (idx >= 0)
                    currentOffice.Conscripts[idx] = window.CreatedConscript;
            }
        }
        else if (!showingConscripts && DataGrid.SelectedItem is Officer officer)
        {
            var window = new NewOfficerWindow(officer, currentOffice);
            if (window.ShowDialog() == true && window.CreatedOfficer != null && window.CreatedOfficer.Valid())
            {
                int idx = currentOffice.Officers.IndexOf(officer);
                if (idx >= 0)
                    currentOffice.Officers[idx] = window.CreatedOfficer;
            }
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataGrid.SelectedItem == null)
        {
            MessageBox.Show("Оберіть елемент для видалення!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (showingConscripts && DataGrid.SelectedItem is Conscript conscript)
        {
            currentOffice.Conscripts.Remove(conscript);
        }
        else if (!showingConscripts && DataGrid.SelectedItem is Officer officer)
        {
            currentOffice.Officers.Remove(officer);
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
                DataGrid.ItemsSource = currentOffice.Officers;
            else
                DataGrid.ItemsSource = new ObservableCollection<Officer>(
                    currentOffice.Officers.Where(o => o.FullName.ToLower().Contains(search)));
        }
    }

    private void SortButton_Click(object sender, RoutedEventArgs e)
    {
        if (SortComboBox.SelectedItem is ComboBoxItem selected)
        {
            string tag = selected.Tag as string ?? "";
            if (showingConscripts)
            {
                var list = currentOffice.Conscripts.ToList();
                switch (tag)
                {
                    case "Name":
                        list = list.OrderBy(c => c.FullName).ToList();
                        break;
                    case "Address":
                        list = list.OrderBy(c => c.Address).ToList();
                        break;
                    case "BirthDate":
                        list = list.OrderBy(c => c.BirthDate).ToList();
                        break;
                }
                DataGrid.ItemsSource = new ObservableCollection<Conscript>(list);
            }
            else
            {
                var list = currentOffice.Officers.ToList();
                switch (tag)
                {
                    case "Name":
                        list = list.OrderBy(o => o.FullName).ToList();
                        break;
                    case "YearsOfService":
                        list = list.OrderBy(o => o.YearsOfService).ToList();
                        break;
                    case "Rank":
                        list = list.OrderBy(o => o.Rank).ToList();
                        break;
                }
                DataGrid.ItemsSource = new ObservableCollection<Officer>(list);
            }
        }
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
            MilitaryOfficeService.SaveAll(dialog.FileName, new ObservableCollection<MilitaryOffice> { currentOffice });
            MessageBox.Show("ТЦК збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
        this.BeginAnimation(OpacityProperty, fadeOut);
        System.Threading.Thread.Sleep(100);
        Application.Current.Shutdown();
    }

    private void HideButton_Click(object sender, RoutedEventArgs e)
    {
        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
        this.BeginAnimation(OpacityProperty, fadeOut);
        System.Threading.Thread.Sleep(100);
        WindowState = WindowState.Minimized;
    }
}
