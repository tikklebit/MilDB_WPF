using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MilDB_WPF;

public partial class NewConscriptWindow : Window
{
    public Conscript? CreatedConscript { get; private set; }
    private readonly MilitaryOffice currentOffice;

    // Додавання конструктора з office для коректної навігації
    public NewConscriptWindow(MilitaryOffice office)
    {
        InitializeComponent();
        currentOffice = office;
        StateChanged += NCW_StateChanged;
    }

    // Для редагування
    public NewConscriptWindow(Conscript conscript, MilitaryOffice office)
    {
        InitializeComponent();
        currentOffice = office;
        // Заповнення полів
        FullNameTextBox.Text = conscript.FullName;
        BirthDatePicker.SelectedDate = conscript.BirthDate;
        AddressTextBox.Text = conscript.Address;
        HealthStatusTextBox.Text = conscript.HealthStatus;
        FitnessCategoryComboBox.Text = conscript.FitnessCategory;
        StatusComboBox.Text = conscript.Status;
        StateChanged += NCW_StateChanged;
    }

    // Для сумісності зі старим кодом (не рекомендується використовувати)
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

        if (birthDate == null)
        {
            ErrorLabel.Content = "Вкажіть дату народження!";
            ErrorLabel.Foreground = new SolidColorBrush(Colors.Red);
            ErrorLabel.Visibility = Visibility.Visible;
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
            ErrorLabel.Content = "Введені дані некоректні!";
            ErrorLabel.Foreground = new SolidColorBrush(Colors.Red);
            ErrorLabel.Visibility = Visibility.Visible;
        }
    }
}
