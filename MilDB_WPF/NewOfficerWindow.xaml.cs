using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MilDB_WPF;

public partial class NewOfficerWindow : Window
{
    private ConscriptList _conscriptList = new ConscriptList();
    public NewOfficerWindow()
    {
        InitializeComponent();
        LoadComboBoxData();
        StateChanged += NOW_StateChanged;
    }
    
    private void LoadComboBoxData()
    {
        // 1. Список звань
        // Ти можеш отримати цей список звідкись (наприклад, з класу Officer, якщо він їх зберігає)
        // Або створити фіксований список, як тут:
        List<string> ranks = new List<string>
        {
            "Солдат",
            "Старший солдат",
            "Молодший сержант",
            "Сержант",
            "Старший сержант",
            "Старшина",
            "Прапорщик", // або Головний старшина, Молодший лейтенант і т.д.
            "Молодший лейтенант",
            "Лейтенант",
            "Старший лейтенант",
            "Капітан",
            "Майор",
            "Підполковник",
            "Полковник",
            "Генерал-майор",
            "Генерал-лейтенант",
            "Генерал-полковник",
            "Генерал армії України"
        };

        // Призначаємо список звань до RankComboBox
        RankComboBox.ItemsSource = ranks;

        // 2. Список призовників
        // Список призовників беремо з нашого ConscriptList
        List<Conscript> conscripts = _conscriptList.Conscripts;

        // Призначаємо список призовників до ConscriptComboBox
        ConscriptComboBox.ItemsSource = conscripts;
        // Вказуємо, яку властивість об'єкта Conscript відображати у ComboBox
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
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
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

        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
    }

    public void AddButton_Click(object sender, EventArgs e)
    {
        string name = FullNameTextBox.Text;
        string rank = RankComboBox.Text;
        int yearsOfService = int.Parse(YearsOfServiceTextBox.Text);

        Officer officer = new Officer(name, rank, yearsOfService);
        if (officer.Valid())
        {
            OfficerList officerList = new OfficerList();
            officerList.AddOfficer(officer);
            string greatMessage = "Успішно додано!";
            ErrorLabel.Content = greatMessage;
            ErrorLabel.Foreground = new SolidColorBrush(Colors.Green);
            ErrorLabel.Visibility = Visibility.Visible;
        }
        else
        {
            string errorMessage = officer.GetCheck();
            ErrorLabel.Content = errorMessage;
            ErrorLabel.Visibility = Visibility.Visible;
        }
    }
}