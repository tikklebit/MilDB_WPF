using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MilDB_WPF;

public partial class NewConscriptWindow : Window
{
    public Conscript? CreatedConscript { get; private set; }
    private readonly MilitaryOffice currentOffice;

    public NewConscriptWindow(MilitaryOffice office)
    {
        InitializeComponent();
        currentOffice = office;
        StateChanged += NCW_StateChanged;
    }

    public NewConscriptWindow(Conscript conscript, MilitaryOffice office)
    {
        InitializeComponent();
        currentOffice = office;
        FullNameTextBox.Text = conscript.FullName;
        BirthDatePicker.SelectedDate = conscript.BirthDate;
        AddressTextBox.Text = conscript.Address;
        HealthStatusTextBox.Text = conscript.HealthStatus;
        FitnessCategoryComboBox.Text = conscript.FitnessCategory;
        StatusComboBox.Text = conscript.Status;
        StateChanged += NCW_StateChanged;
    }

    public NewConscriptWindow()
    {
        InitializeComponent();
        StateChanged += NCW_StateChanged;
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
        await System.Threading.Tasks.Task.Delay(100);
        this.Hide();
    }

    private async void HideButton_Click(object sender, RoutedEventArgs e)
    {
        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
        this.BeginAnimation(OpacityProperty, fadeOut);
        await System.Threading.Tasks.Task.Delay(100);
        WindowState = WindowState.Minimized;
    }

    private void NCW_StateChanged(object sender, EventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));
            this.BeginAnimation(OpacityProperty, fadeIn);
        }
    }

    private async void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
        this.BeginAnimation(OpacityProperty, fadeOut);
        await System.Threading.Tasks.Task.Delay(100);
        this.Hide();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        string name = FullNameTextBox.Text.Trim();
        DateTime? birthDate = BirthDatePicker.SelectedDate;
        string address = AddressTextBox.Text.Trim();
        string healthStatus = HealthStatusTextBox.Text.Trim();
        string fitnessCategory = FitnessCategoryComboBox.Text;
        string status = StatusComboBox.Text;

        if (!IsValidFullName(name))
        {
            ShowError("ПІБ має містити лише літери, починатися з великої, слова розділені пробілом або дефісом.");
            return;
        }
        if (birthDate == null)
        {
            ShowError("Вкажіть дату народження!");
            return;
        }
        int age = GetAge(birthDate.Value, DateTime.Today);
        if (age < 18 || age > 27)
        {
            ShowError("Вік призовника має бути від 18 до 27 років.");
            return;
        }
        if (string.IsNullOrWhiteSpace(address))
        {
            ShowError("Вкажіть адресу!");
            return;
        }
        if (string.IsNullOrWhiteSpace(healthStatus))
        {
            ShowError("Вкажіть стан здоров'я!");
            return;
        }
        if (string.IsNullOrWhiteSpace(fitnessCategory))
        {
            ShowError("Вкажіть категорію придатності!");
            return;
        }
        if (string.IsNullOrWhiteSpace(status))
        {
            ShowError("Вкажіть статус!");
            return;
        }

        var conscript = new Conscript(name, birthDate.Value, address, healthStatus, fitnessCategory, status);
        if (conscript.Valid())
        {
            CreatedConscript = conscript;
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

    private static int GetAge(DateTime birthDate, DateTime now)
    {
        int age = now.Year - birthDate.Year;
        if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
            age--;
        return age;
    }

    private static bool IsValidFullName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
        var flexibleRegex = new Regex(@"^[А-ЯІЇЄҐ][а-яіїєґ'-]*(?:[ \-][А-ЯІЇЄҐ][а-яіїєґ'-]*)*$");

        return flexibleRegex.IsMatch(name);
    }
}
