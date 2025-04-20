using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace MilDB_WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ConscriptList conscriptList;
    private Conscript conscript;
    static int index = 0;

    public MainWindow()
    {
        InitializeComponent();
        conscriptList = new ConscriptList();
        conscript = new Conscript();
        this.StateChanged += MainWindow_StateChanged;
        UpdateTableButton_Click(this, EventArgs.Empty);
    }

    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }

    private async void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
        this.BeginAnimation(OpacityProperty, fadeOut);
        await Task.Delay(100);
        Application.Current.Shutdown();
    }

    private async void HideButton_Click(object sender, RoutedEventArgs e)
    {
        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
        this.BeginAnimation(OpacityProperty, fadeOut);
        await Task.Delay(100);
        WindowState = WindowState.Minimized;
    }
    private void MainWindow_StateChanged(object sender, EventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));
            this.BeginAnimation(OpacityProperty, fadeIn);
        }
    }

    private void OpenSelectWindow_Click(object sender, EventArgs e)
    {
        Select selectWindow = new Select();
        selectWindow.Show();
        this.Hide();
    }

    private void UpdateTableButton_Click(object sender, EventArgs e)
    {
        conscriptList.Deserialize();
        foreach (Conscript conscript in conscriptList.Conscripts)
        {
            DataGrid.ItemsSource = conscriptList.Conscripts;
        }   
    }

    private void ChangeInfoButton_Click(object sender, EventArgs e)
    {
        if (DataGrid.SelectedItem is Conscript selectedConscript)
        {
            index = conscriptList.Conscripts.IndexOf(selectedConscript);
            NewConscriptWindow newConscript = new NewConscriptWindow();

            newConscript.FullNameTextBox.Text = selectedConscript.FullName;
            newConscript.BirthDatePicker.SelectedDate = selectedConscript.BirthDate;
            newConscript.AddressTextBox.Text = selectedConscript.Address;
            newConscript.HealthStatusTextBox.Text = selectedConscript.HealthStatus;
            newConscript.FitnessCategoryComboBox.Text = selectedConscript.FitnessCategory;
            newConscript.StatusComboBox.Text = selectedConscript.Status;

            newConscript.CancelButton.Visibility = Visibility.Collapsed;

            newConscript.SaveButton.Content = "РЕДАГУВАТИ";
            newConscript.SaveButton.Width = 150; 
            newConscript.SaveButton.Height = 50;
            newConscript.SaveButton.HorizontalAlignment = HorizontalAlignment.Center;
            newConscript.SaveButton.Click -= newConscript.AddButton_Click;
            newConscript.SaveButton.Click += (s, e) => EditButton_Click(newConscript, e);

            this.Hide();
            newConscript.Show();
        }
        else
        {
            MessageBox.Show("Оберіть призовника!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void EditButton_Click(NewConscriptWindow newConscript, RoutedEventArgs e)
    {
        string name = newConscript.FullNameTextBox.Text;
        DatePicker birthDate = newConscript.BirthDatePicker;
        string address = newConscript.AddressTextBox.Text;
        string healthStatus = newConscript.HealthStatusTextBox.Text;
        string fitnessCategory = newConscript.FitnessCategoryComboBox.Text;
        string status = newConscript.StatusComboBox.Text;

        if (birthDate.SelectedDate.HasValue)
        {
            Conscript? conscript = new Conscript(name, birthDate.SelectedDate.Value, address, healthStatus, fitnessCategory, status);
            if (conscript.Valid())
            {
                ConscriptList conscriptList = new ConscriptList();
                conscriptList.EditConscript(conscript, index);
                string greatMessage = "Відредаговано!";
                newConscript.ErrorLabel.Content = greatMessage;
                newConscript.ErrorLabel.Foreground = new SolidColorBrush(Colors.Green);
                newConscript.ErrorLabel.Visibility = Visibility.Visible;
            }
            else
            {
                string errorMessage = conscript.GetCheck();
                newConscript.ErrorLabel.Content = errorMessage;
                newConscript.ErrorLabel.Visibility = Visibility.Visible;
            }
        }
        else
        {
            MessageBox.Show("Вкажіть дату народження!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataGrid.SelectedItem is Conscript selectedConscript)
        {
            conscriptList.RemoveConscript(selectedConscript);
            UpdateTableButton_Click(this, EventArgs.Empty);
            DataGrid.Items.Refresh();
        }
        else
        {
            MessageBox.Show("Оберіть призовника!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

}