using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

namespace MilDB_WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ConscriptList inFileConscriptList;
    private string _file = "conscripts.json";
    static string? filePath;
    static string? openFile;
    static int index = 0;
    private List<Conscript> originalConscripts;

    // Конструктор
    public MainWindow()
    {
        InitializeComponent();
        inFileConscriptList = new ConscriptList();
        StateChanged += MainWindow_StateChanged;
        UpdateTable(this, EventArgs.Empty);
    }

    // Методи для роботи з вікном
    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }

    private async void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        File.WriteAllText(_file, string.Empty);
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

    // Методи для роботи з таблицею
    private void SaveDataButton_Click(object sender, RoutedEventArgs e)
    {
        inFileConscriptList.Serialize();
        DataGrid.ItemsSource = inFileConscriptList.Conscripts;
        DataGrid.Items.Refresh();
    }

    private void UpdateTable(object sender, EventArgs e)
    {
        inFileConscriptList.Deserialize();
        originalConscripts = new List<Conscript>(inFileConscriptList.Conscripts);
        DataGrid.ItemsSource = inFileConscriptList.Conscripts;
        DataGrid.Items.Refresh();
    }

    private void ChangeInfoButton_Click(object sender, EventArgs e)
    {
        if (DataGrid.SelectedItem is Conscript selectedConscript)
        {
            index = inFileConscriptList.Conscripts.IndexOf(selectedConscript);
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
            inFileConscriptList.RemoveConscript(selectedConscript);
            UpdateTable(this, EventArgs.Empty);
            DataGrid.Items.Refresh();
        }
        else
        {
            MessageBox.Show("Оберіть призовника!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    // Методи сортування
    public void SortByNameButton_Click(object sender, RoutedEventArgs e)
    {
        originalConscripts.Sort((x, y) => string.Compare(x.FullName, y.FullName));
        DataGrid.ItemsSource = originalConscripts;
        DataGrid.Items.Refresh();
    }

    public void SortByAdress_Click(object sender, RoutedEventArgs e)
    {
        originalConscripts.Sort((x, y) => string.Compare(x.Address, y.Address));
        DataGrid.ItemsSource = originalConscripts;
        DataGrid.Items.Refresh();
    }

    public void SortByDate_Click(object sender, RoutedEventArgs e)
    {
        originalConscripts.Sort((x, y) => DateTime.Compare(x.BirthDate, y.BirthDate));
        DataGrid.ItemsSource = originalConscripts;
        DataGrid.Items.Refresh();
    }
    


    public void SetDefault_Click(object sender, RoutedEventArgs e)
    {
        if (originalConscripts != null)
        {
            originalConscripts = new List<Conscript>(inFileConscriptList.Conscripts); 
            DataGrid.ItemsSource = inFileConscriptList.Conscripts;
            DataGrid.Items.Refresh();
        }
        else
        {
            MessageBox.Show("Дані за замовчуванням недоступні.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void SortMenu(object sender, RoutedEventArgs e)
    {
        var sortAndFiltredWindow = new dialoges.SortAndFiltred();

        sortAndFiltredWindow.SortByNameAndSurname.Click += (s, args) => { SortByNameButton_Click(sender, args); sortAndFiltredWindow.Close(); };
        sortAndFiltredWindow.SortByAddress.Click += (s, args) => { SortByAdress_Click(sender, args); sortAndFiltredWindow.Close(); };
        sortAndFiltredWindow.SortByBDate.Click += (s, args) => { SortByDate_Click(sender, args); sortAndFiltredWindow.Close(); };
        sortAndFiltredWindow.DefoltInfoS.Click += (s, args) => { SetDefault_Click(sender, args); sortAndFiltredWindow.Close(); };

        sortAndFiltredWindow.ShowDialog();
    }

    // Методи фільтрації
    

    // Методи збереження та відкриття файлу
    private void SaveToFileButton_Click(object sender, RoutedEventArgs e)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "JSON файли (*.json)|*.json|Усі файли (*.*)|*.*",
            DefaultExt = "json",
            Title = "Зберегти файл"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                string filePath = saveFileDialog.FileName;
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(inFileConscriptList.Conscripts, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
                MessageBox.Show("Файл успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час збереження файлу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void SaveToOpenFile(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            MessageBox.Show("Спочатку відкрийте файл!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        try
        {
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(inFileConscriptList.Conscripts, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
            MessageBox.Show("Файл успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка під час збереження файлу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OpenFromFileButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "JSON файли (*.json)|*.json|Усі файли (*.*)|*.*",
            DefaultExt = "json",
            Title = "Відкрити файл"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                filePath = openFileDialog.FileName;
                openFile = Path.GetFileName(filePath);
                Title.Content = $"ТАБЛИЦЯ - {openFile}";

                string jsonData = File.ReadAllText(filePath);
                var conscripts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Conscript>>(jsonData);

                if (conscripts != null)
                {
                    inFileConscriptList.Conscripts = conscripts;
                    DataGrid.ItemsSource = inFileConscriptList.Conscripts;
                    DataGrid.Items.Refresh();

                    string bufferFileJson = File.ReadAllText(_file);
                    inFileConscriptList.Serialize();
                    UpdateTable(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("Файл не містить коректних даних.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час відкриття файлу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
