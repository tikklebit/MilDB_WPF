using System;
using System.Text.RegularExpressions;
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
    private List<CheckBox> conscriptCheckBoxes = new();

    public NewOfficerWindow(MilitaryOffice office)
    {
        InitializeComponent();
        currentOffice = office;
        LoadRanks();
        LoadConscriptsList();
        StateChanged += NOW_StateChanged;
    }

    public NewOfficerWindow(Officer officer, MilitaryOffice office)
    {
        InitializeComponent();
        currentOffice = office;
        LoadRanks();
        LoadConscriptsList();
        FullNameTextBox.Text = officer.FullName;
        RankComboBox.Text = officer.Rank;
        YearsOfServiceTextBox.Text = officer.YearsOfService.ToString();
        foreach (var cb in conscriptCheckBoxes)
        {
            if (officer.AssignedConscripts.Contains((Conscript)cb.Tag))
                cb.IsChecked = true;
        }
        StateChanged += NOW_StateChanged;
    }

    private void LoadRanks()
    {
        List<string> ranks = new()
    {
        "Солдат", "Старший солдат", "Молодший сержант", "Сержант", "Старший сержант",
        "Старшина", "Прапорщик", "Молодший лейтенант", "Лейтенант", "Старший лейтенант",
        "Капітан", "Майор", "Підполковник", "Полковник", "Генерал-майор",
        "Генерал-лейтенант", "Генерал-полковник", "Генерал армії України"
    };
        RankComboBox.ItemsSource = ranks;
    }

    private void LoadConscriptsList()
    {
        ConscriptsPanel.Children.Clear();
        conscriptCheckBoxes.Clear();

        foreach (var conscript in currentOffice.Conscripts)
        {
            var cb = new CheckBox
            {
                Content = conscript.FullName,
                Tag = conscript,
                Margin = new Thickness(2),
                Foreground = Brushes.White
            };
            conscriptCheckBoxes.Add(cb);
            ConscriptsPanel.Children.Add(cb);
        }
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

        if (string.IsNullOrWhiteSpace(name))
        {
            ShowError("ПІБ не може бути порожнім!");
            return;
        }

        if (string.IsNullOrWhiteSpace(rank))
        {
            ShowError("Виберіть звання!");
            return;
        }

        if (string.IsNullOrWhiteSpace(YearsOfServiceTextBox.Text))
        {
            ShowError("Вкажіть стаж служби!");
            return;
        }

        if (!IsValidFullName(name))
        {
            ShowError("ПІБ має містити лише літери, починатися з великої, слова розділені пробілом або дефісом.");
            return;
        }

        int yearsOfService;
        if (!int.TryParse(YearsOfServiceTextBox.Text, out yearsOfService))
        {
            ShowError("Некоректний стаж!");
            return;
        }

        var officer = new Officer(name, rank, yearsOfService);

        foreach (var cb in conscriptCheckBoxes)
        {
            if (cb.IsChecked == true && cb.Tag is Conscript conscript)
                officer.AssignedConscripts.Add(conscript);
        }

        if (officer.Valid())
        {
            CreatedOfficer = officer;
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

    private static bool IsValidFullName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
        var flexibleRegex = new Regex(@"^[А-ЯІЇЄҐ][а-яіїєґ'-]*(?:[ \-][А-ЯІЇЄҐ][а-яіїєґ'-]*)*$");

        return flexibleRegex.IsMatch(name);
    }
}