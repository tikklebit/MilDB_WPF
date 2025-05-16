using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MilDB_WPF;

public partial class NewOfficerWindow : Window
{
    public Officer? CreatedOfficer { get; private set; }
    private readonly MilitaryOffice currentOffice;

    public NewOfficerWindow(MilitaryOffice office)
    {
        InitializeComponent();
        currentOffice = office;
        LoadComboBoxData();
        StateChanged += NOW_StateChanged;
    }

    public NewOfficerWindow(Officer officer, MilitaryOffice office)
    {
        InitializeComponent();
        currentOffice = office;
        LoadComboBoxData();
        FullNameTextBox.Text = officer.FullName;
        RankComboBox.Text = officer.Rank;
        YearsOfServiceTextBox.Text = officer.YearsOfService.ToString();
        StateChanged += NOW_StateChanged;
    }

    private void LoadComboBoxData()
    {
        List<string> ranks = new List<string>
        {
            "Солдат", "Старший солдат", "Молодший сержант", "Сержант", "Старший сержант",
            "Старшина", "Прапорщик", "Молодший лейтенант", "Лейтенант", "Старший лейтенант",
            "Капітан", "Майор", "Підполковник", "Полковник", "Генерал-майор",
            "Генерал-лейтенант", "Генерал-полковник", "Генерал армії України"
        };
        RankComboBox.ItemsSource = ranks;

        ConscriptComboBox.ItemsSource = currentOffice.Conscripts;
        ConscriptComboBox.DisplayMemberPath = "FullName";
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
        this.Hide();
    }

    private async void HideButton_Click(object sender, RoutedEventArgs e)
    {
        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
        this.BeginAnimation(OpacityProperty, fadeOut);
        await Task.Delay(100);
        WindowState = WindowState.Minimized;
    }

    private void NOW_StateChanged(object sender, EventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));
            this.BeginAnimation(OpacityProperty, fadeIn);
        }
    }

    private async void ExitButton_Click(object sender, EventArgs e)
    {
        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
        this.BeginAnimation(OpacityProperty, fadeOut);
        await Task.Delay(100);
        this.Hide();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        string name = FullNameTextBox.Text.Trim();
        string rank = RankComboBox.Text;
        int yearsOfService;
        if (!int.TryParse(YearsOfServiceTextBox.Text, out yearsOfService))
        {
            ErrorLabel.Content = "Некоректний стаж!";
            ErrorLabel.Foreground = new SolidColorBrush(Colors.Red);
            ErrorLabel.Visibility = Visibility.Visible;
            return;
        }

        var officer = new Officer(name, rank, yearsOfService);
        if (ConscriptComboBox.SelectedItem is Conscript selectedConscript)
        {
            officer.AssignedConscripts.Add(selectedConscript);
        }
        if (officer.Valid())
        {
            CreatedOfficer = officer;
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
