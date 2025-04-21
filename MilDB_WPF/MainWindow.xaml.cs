using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;

namespace MilDB_WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ConscriptList conscriptList;
    private Conscript conscript;
    static int index = 0;

    // Конструктор
    public MainWindow()
    {
        InitializeComponent();
        conscriptList = new ConscriptList();
        conscript = new Conscript();
        this.StateChanged += MainWindow_StateChanged;
        UpdateTableButton_Click(this, EventArgs.Empty);
    }

    // Методи для роботи з вікном
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

    // Методи для роботи з даними
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

    // Методи сортування
    private void SortByNameButton_Click(object sender, RoutedEventArgs e)
    {
        ConscriptList sorted = new ConscriptList();
        sorted.Conscripts.Sort((x, y) => string.Compare(x.FullName, y.FullName));
        DataGrid.ItemsSource = sorted.Conscripts;
        DataGrid.Items.Refresh();
    }

    private void SortByAdress_Click(object sender, RoutedEventArgs e)
    {
        ConscriptList sorted = new ConscriptList();
        sorted.Conscripts.Sort((x, y) => string.Compare(x.Address, y.Address));
        DataGrid.ItemsSource = sorted.Conscripts;
        DataGrid.Items.Refresh();
    }

    private void SortByDate_Click(object sender, RoutedEventArgs e)
    {
        ConscriptList sorted = new ConscriptList();
        sorted.Conscripts.Sort((x, y) => DateTime.Compare(x.BirthDate, y.BirthDate));
        DataGrid.ItemsSource = sorted.Conscripts;
        DataGrid.Items.Refresh();
    }

    private void SortMenu(object sender, RoutedEventArgs e)
    {
        SortComboBox.Items.Clear();
        SortComboBox.Visibility = Visibility.Visible;
        SortTitle.Visibility = Visibility.Visible;
        SortComboBox.Items.Add("Сортувати...");
        SortComboBox.Items.Add("Сортувати за іменем та прізвищем");
        SortComboBox.Items.Add("Сортувати за адресою");
        SortComboBox.Items.Add("Сортувати за датою народження");

        SortComboBox.SelectedIndex = 0;
        SortComboBox.SelectionChanged += (s, e) =>
        {
            switch (SortComboBox.SelectedIndex)
            {
                case 1:
                    SortByNameButton_Click(sender, e);
                    SortComboBox.Visibility = Visibility.Collapsed;
                    SortTitle.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    SortByAdress_Click(sender, e);
                    SortComboBox.Visibility = Visibility.Collapsed;
                    SortTitle.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    SortByDate_Click(sender, e);
                    SortComboBox.Visibility = Visibility.Collapsed;
                    SortTitle.Visibility = Visibility.Collapsed;
                    break;
            }
        };
    }
}
