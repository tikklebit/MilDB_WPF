using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace MilDB_WPF;

public partial class MainWindow : Window
{
    private ConscriptList inFileConscriptList;
    private OfficerList officerList; // Додано: екземпляр OfficerList

    private string _file = "conscripts.json"; // Файл для призовників
    private string _officersFile = "officers.json"; // Додано: файл для офіцерів

    static string? filePath;
    static string? openFile;
    static int index = 0;
    private List<Conscript> originalConscripts;

    private List<DataGridColumn> originalDataGridColumns; // Зберігає стовпці для призовників

    public MainWindow()
    {
        InitializeComponent();
        inFileConscriptList = new ConscriptList(); // Завантажує призовників з conscripts.json

        // Додано: Ініціалізуємо та завантажуємо список офіцерів
        officerList = new OfficerList(); // Конструктор OfficerList має викликати Deserialize() з officers.json

        StateChanged += MainWindow_StateChanged;
        UpdateTable(this, EventArgs.Empty); // Спочатку відображаємо таблицю призовників
        originalDataGridColumns = new List<DataGridColumn>(DataGrid.Columns); // Зберігаємо стовпці призовників
    }

    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }

    private async void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        // Можливо, тут теж потрібно серіалізувати officersList перед виходом?
        // officerList.Serialize();

        File.WriteAllText(_file, string.Empty); // Очищення файлу призовників? Можливо, це не завжди бажано.
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

    private void SaveDataButton_Click(object sender, RoutedEventArgs e)
    {
        // Ця кнопка, ймовірно, зберігає поточні дані в таблиці.
        // Якщо в таблиці зараз призовники, зберігаємо призовників.
        // Якщо офіцери, потрібно мати логіку для їх збереження.
        // Припустимо, це кнопка для збереження призовників.
        inFileConscriptList.Serialize();
        // DataGrid.ItemsSource = inFileConscriptList.Conscripts; // Цей рядок не потрібен тут, ItemsSource вже встановлено
        DataGrid.Items.Refresh(); // Оновлення відображення
    }

    private void UpdateTable(object sender, EventArgs e)
    {
        // Цей метод завантажує призовників і оновлює таблицю
        inFileConscriptList.Deserialize();
        originalConscripts = new List<Conscript>(inFileConscriptList.Conscripts); // Зберігаємо для пошуку/сортування
        DataGrid.ItemsSource = inFileConscriptList.Conscripts; // Встановлюємо ItemsSource на список призовників
        DataGrid.Items.Refresh();

        if (openFile != null) Title.Content = $"ТАБЛИЦЯ - {openFile}"; // Оновлюємо заголовок для призовників
        else Title.Content = "ТАБЛИЦЯ - Новий файл (Призовники)";
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Важливо: Пошук працює лише для призовників
        // Якщо відображаються офіцери, цей пошук не спрацює коректно
        string searchText = SearchTextBox.Text.Trim();

        // Перевіряємо, чи зараз у таблиці відображаються призовники
        // Це можна зробити, наприклад, перевіривши тип об'єктів у ItemsSource
        if (DataGrid.ItemsSource is List<Conscript> currentConscripts)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                // Показуємо оригінальний список призовників
                DataGrid.ItemsSource = originalConscripts ?? inFileConscriptList.Conscripts;
            }
            else
            {
                // Фільтруємо список призовників
                var filteredList = (originalConscripts ?? inFileConscriptList.Conscripts)
                    .Where(conscript =>
                        conscript.FullName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                DataGrid.ItemsSource = filteredList;
            }
            DataGrid.Items.Refresh();
        }
        // Якщо в таблиці офіцери, пошук за іменем призовника не має сенсу
        // Можна додати тут логіку пошуку офіцерів, якщо потрібно
    }


    private void ChangeInfoButton_Click(object sender, EventArgs e)
    {
        // Ця кнопка, ймовірно, призначена для редагування призовника
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
        else if (DataGrid.SelectedItem is Officer selectedOfficer) // Додано: обробка вибору офіцера
        {
            // Якщо вибрано офіцера, можливо, потрібно відкрити вікно для редагування офіцера
            // Або показати його дані. Це залежить від твоїх потреб.
            MessageBox.Show($"Вибрано офіцера: {selectedOfficer.FullName}", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
            // Тут може бути код для відкриття вікна редагування офіцера
        }
        else
        {
            MessageBox.Show("Оберіть елемент для редагування!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    // Приклад методу EditButton_Click, який отримує вікно як параметр
    // Цей метод, ймовірно, знаходиться у MainWindow
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
                // Важливо: Редагуємо в тому ж списку, який відображається в таблиці
                // Використовуємо індекс, який був збережений при відкритті вікна редагування
                inFileConscriptList.EditConscript(conscript, index); // Редагуємо через OfficerList

                string greatMessage = "Відредаговано!";
                newConscript.ErrorLabel.Content = greatMessage;
                newConscript.ErrorLabel.Foreground = new SolidColorBrush(Colors.Green);
                newConscript.ErrorLabel.Visibility = Visibility.Visible;

                // Оновлюємо таблицю після редагування
                UpdateTable(this, EventArgs.Empty); // Перезавантажуємо дані та оновлюємо таблицю
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
        // Ця кнопка, ймовірно, призначена для видалення призовника
        if (DataGrid.SelectedItem is Conscript selectedConscript)
        {
            inFileConscriptList.RemoveConscript(selectedConscript);
            UpdateTable(this, EventArgs.Empty); // Оновлюємо таблицю призовників
            // DataGrid.Items.Refresh(); // UpdateTable вже оновлює ItemsSource, цей рядок зайвий
        }
        else if (DataGrid.SelectedItem is Officer selectedOfficer) // Додано: обробка видалення офіцера
        {
            // Якщо вибрано офіцера, можливо, потрібно видалити офіцера
            officerList.RemoveOfficer(selectedOfficer); // Видаляємо офіцера
                                                        // Якщо зараз відображаються офіцери, потрібно оновити таблицю офіцерів:
            if (DataGrid.ItemsSource is List<Officer>)
            {
                DataGrid.ItemsSource = officerList.Officers;
                DataGrid.Items.Refresh();
            }
            MessageBox.Show($"Офіцера {selectedOfficer.FullName} видалено!", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Оберіть елемент для видалення!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    // Методи сортування (працюють з originalConscripts)
    public void SortByNameButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataGrid.ItemsSource is List<Conscript>) // Сортування тільки якщо відображаються призовники
        {
            // originalConscripts потрібно оновлювати перед сортуванням, якщо дані змінювались
            // Або сортувати поточний ItemsSource (якщо це ObservableCollection або List)
            List<Conscript> currentList = DataGrid.ItemsSource as List<Conscript> ?? originalConscripts;
            if (currentList != null)
            {
                currentList.Sort((x, y) => string.Compare(x.FullName, y.FullName));
                DataGrid.ItemsSource = null; // Скидаємо ItemsSource
                DataGrid.ItemsSource = currentList; // Призначаємо назад для оновлення
                DataGrid.Items.Refresh();
            }
        }
        // Можна додати сортування для офіцерів, якщо потрібно
    }

    public void SortByAdress_Click(object sender, RoutedEventArgs e)
    {
        if (DataGrid.ItemsSource is List<Conscript>) // Сортування тільки якщо відображаються призовники
        {
            List<Conscript> currentList = DataGrid.ItemsSource as List<Conscript> ?? originalConscripts;
            if (currentList != null)
            {
                currentList.Sort((x, y) => string.Compare(x.Address, y.Address));
                DataGrid.ItemsSource = null;
                DataGrid.ItemsSource = currentList;
                DataGrid.Items.Refresh();
            }
        }
    }

    public void SortByDate_Click(object sender, RoutedEventArgs e)
    {
        if (DataGrid.ItemsSource is List<Conscript>) // Сортування тільки якщо відображаються призовники
        {
            List<Conscript> currentList = DataGrid.ItemsSource as List<Conscript> ?? originalConscripts;
            if (currentList != null)
            {
                currentList.Sort((x, y) => DateTime.Compare(x.BirthDate, y.BirthDate));
                DataGrid.ItemsSource = null;
                DataGrid.ItemsSource = currentList;
                DataGrid.Items.Refresh();
            }
        }
    }

    public void SetDefault_Click(object sender, RoutedEventArgs e)
    {
        if (DataGrid.ItemsSource is List<Conscript>) // Повернення до дефолту тільки якщо відображаються призовники
        {
            // Перезавантажуємо оригінальний список призовників з файлу
            inFileConscriptList.Deserialize();
            originalConscripts = new List<Conscript>(inFileConscriptList.Conscripts); // Оновлюємо збережену копію
            DataGrid.ItemsSource = inFileConscriptList.Conscripts; // Встановлюємо ItemsSource на свіжозавантажений список
            DataGrid.Items.Refresh();
        }
        // Можна додати логіку для повернення таблиці офіцерів до дефолту, якщо потрібно
    }


    private void SortMenu(object sender, RoutedEventArgs e)
    {
        var sortAndFiltredWindow = new dialoges.SortAndFiltred();

        // Важливо: Обробники подій сортування/фільтрації прив'язані до кнопок в SortAndFiltred
        // Переконайся, що методи SortByNameButton_Click, ApplyFilters тощо
        // коректно працюють з поточним ItemsSource, який може бути List<Conscript> або List<Officer>

        // Поточна логіка сортування/фільтрації в SortAndFiltred прив'язана до призовників (originalConscripts)
        // Якщо ти хочеш сортувати/фільтрувати офіцерів, потрібно додати відповідну логіку та, можливо,
        // передавати тип даних або поточний ItemsSource до SortAndFiltred.

        sortAndFiltredWindow.SortByNameAndSurname.Click += (s, args) => { SortByNameButton_Click(sender, args); sortAndFiltredWindow.Close(); };
        sortAndFiltredWindow.SortByAddress.Click += (s, args) => { SortByAdress_Click(sender, args); sortAndFiltredWindow.Close(); };
        sortAndFiltredWindow.SortByBDate.Click += (s, args) => { SortByDate_Click(sender, args); sortAndFiltredWindow.Close(); };
        sortAndFiltredWindow.DefoltInfoS.Click += (s, args) => { SetDefault_Click(sender, args); sortAndFiltredWindow.Close(); };

        bool? dialogResult = sortAndFiltredWindow.ShowDialog();

        if (dialogResult == true)
        {
            // Логіка фільтрації застосовується до призовників
            int? filterYear = sortAndFiltredWindow.FilterYear;
            string? filterCategory = sortAndFiltredWindow.FilterCategoryText;
            string? filterStatus = sortAndFiltredWindow.FilterStatusText;

            ApplyFilters(filterYear, filterCategory, filterStatus);
        }
    }

    // Метод ApplyFilters (працює з originalConscripts)
    private void ApplyFilters(int? year, string? category, string? status)
    {
        // Важливо: Фільтрація працює лише для призовників
        if (DataGrid.ItemsSource is List<Conscript>) // Фільтруємо тільки якщо відображаються призовники
        {
            IEnumerable<Conscript> filteredConscripts = originalConscripts ?? inFileConscriptList.Conscripts; // Фільтруємо на основі збереженого або поточного списку

            if (year.HasValue)
            {
                filteredConscripts = filteredConscripts.Where(c => c.BirthDate.Year == year.Value);
            }

            if (!string.IsNullOrEmpty(category))
            {
                filteredConscripts = filteredConscripts.Where(c => c.FitnessCategory == category);
            }

            if (!string.IsNullOrEmpty(status))
            {
                filteredConscripts = filteredConscripts.Where(c => c.Status == status);
            }

            DataGrid.ItemsSource = filteredConscripts.ToList();
            DataGrid.Items.Refresh();
        }
        // Можна додати логіку фільтрації для офіцерів, якщо потрібно
    }

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
                string currentFilePath = saveFileDialog.FileName;

                // Визначаємо, які дані зараз в таблиці, і серіалізуємо їх
                if (DataGrid.ItemsSource is List<Conscript> conscriptsToSave)
                {
                    string jsonData = JsonConvert.SerializeObject(conscriptsToSave, Formatting.Indented);
                    File.WriteAllText(currentFilePath, jsonData);
                    MessageBox.Show("Файл призовників успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (DataGrid.ItemsSource is List<Officer> officersToSave)
                {
                    string jsonData = JsonConvert.SerializeObject(officersToSave, Formatting.Indented);
                    File.WriteAllText(currentFilePath, jsonData);
                    MessageBox.Show("Файл офіцерів успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Немає даних для збереження або тип даних не підтримується.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час збереження файлу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void SaveToOpenFile(object sender, RoutedEventArgs e)
    {
        // Ця функція зберігає в раніше відкритий файл.
        // Вона прив'язана до inFileConscriptList, тобто зберігає призовників.
        // Якщо зараз відображаються офіцери і їх потрібно зберегти,
        // то логіка має бути іншою (з officerList).
        if (string.IsNullOrEmpty(filePath))
        {
            MessageBox.Show("Спочатку відкрийте файл!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        try
        {
            // Зберігаємо поточні дані в таблиці до раніше відкритого файлу
            // Потрібно визначити, який тип даних зараз в таблиці
            if (DataGrid.ItemsSource is List<Conscript> conscriptsToSave)
            {
                string jsonData = JsonConvert.SerializeObject(conscriptsToSave, Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
                MessageBox.Show("Файл призовників успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (DataGrid.ItemsSource is List<Officer> officersToSave)
            {
                // Якщо потрібно зберігати офіцерів до файлу filePath,
                // переконайся, що filePath вказує на файл офіцерів
                // Або реалізуй окрему логіку збереження для офіцерів
                MessageBox.Show("Збереження офіцерів до раніше відкритого файлу не реалізовано.", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Немає даних для збереження або тип даних не підтримується.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
                string selectedFilePath = openFileDialog.FileName;
                string fileContent = File.ReadAllText(selectedFilePath);

                // Спробуємо десеріалізувати як список призовників
                try
                {
                    var conscripts = JsonConvert.DeserializeObject<List<Conscript>>(fileContent);
                    if (conscripts != null)
                    {
                        filePath = selectedFilePath; // Зберігаємо шлях до відкритого файлу
                        openFile = Path.GetFileName(filePath);
                        Title.Content = $"ТАБЛИЦЯ - {openFile} (Призовники)";

                        inFileConscriptList.Conscripts = conscripts; // Оновлюємо список призовників
                        originalConscripts = new List<Conscript>(inFileConscriptList.Conscripts); // Оновлюємо збережену копію

                        // Встановлюємо стовпці та дані для призовників
                        DataGrid.Columns.Clear();
                        foreach (var column in originalDataGridColumns) // Використовуємо збережені стовпці призовників
                        {
                            DataGrid.Columns.Add(column);
                        }
                        DataGrid.ItemsSource = inFileConscriptList.Conscripts;
                        DataGrid.Items.Refresh();
                        return; // Успішно відкрито як призовників
                    }
                }
                catch (JsonException)
                {
                    // Не вдалося десеріалізувати як призовників, спробуємо як офіцерів
                }

                // Спробуємо десеріалізувати як список офіцерів
                try
                {
                    var officers = JsonConvert.DeserializeObject<List<Officer>>(fileContent);
                    if (officers != null)
                    {
                        filePath = selectedFilePath; // Зберігаємо шлях до відкритого файлу
                        openFile = Path.GetFileName(filePath);
                        Title.Content = $"ТАБЛИЦЯ - {openFile} (Офіцери)";

                        officerList.Officers = officers; // Оновлюємо список офіцерів

                        // Встановлюємо стовпці та дані для офіцерів
                        DataGrid.Columns.Clear();
                        // Створюємо стовпці для офіцерів
                        DataGridTextColumn fullNameColumn = new DataGridTextColumn { Header = "Повне Ім'я Офіцера", Binding = new System.Windows.Data.Binding("FullName"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) };
                        DataGridTextColumn rankColumn = new DataGridTextColumn { Header = "Звання", Binding = new System.Windows.Data.Binding("Rank") };
                        DataGridTextColumn yearsColumn = new DataGridTextColumn { Header = "Стаж (років)", Binding = new System.Windows.Data.Binding("YearsOfService") };

                        DataGrid.Columns.Add(fullNameColumn);
                        DataGrid.Columns.Add(rankColumn);
                        DataGrid.Columns.Add(yearsColumn);

                        DataGrid.ItemsSource = officerList.Officers; // Встановлюємо ItemsSource на список офіцерів
                        DataGrid.Items.Refresh();
                        return; // Успішно відкрито як офіцерів
                    }
                }
                catch (JsonException)
                {
                    // Не вдалося десеріалізувати ні як призовників, ні як офіцерів
                }


                MessageBox.Show("Вибраний файл не містить коректних даних призовників або офіцерів.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час відкриття файлу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }


    // Метод для перемикання на таблицю призовників
    private void ConscriptTableButton_Click(object sender, RoutedEventArgs e)
    {
        // Відновлюємо оригінальні стовпці (для призовників)
        DataGrid.Columns.Clear();
        // originalDataGridColumns зберігає стовпці, які були в XAML для призовників
        foreach (var column in originalDataGridColumns)
        {
            DataGrid.Columns.Add(column);
        }

        // Встановлюємо ItemsSource на список призовників
        // Використовуємо список, який вже є в inFileConscriptList
        DataGrid.ItemsSource = inFileConscriptList.Conscripts;

        DataGrid.Items.Refresh();

        // Оновлюємо заголовок вікна
        if (openFile != null) Title.Content = $"ТАБЛИЦЯ - {openFile} (Призовники)";
        else Title.Content = "ТАБЛИЦЯ - Новий файл (Призовники)";
    }

    // Метод для перемикання на таблицю офіцерів
    private void OfficerTableButton_Click(object sender, RoutedEventArgs e)
    {
        // Очищаємо всі поточні стовпці
        DataGrid.Columns.Clear();

        // Створюємо та додаємо стовпці спеціально для офіцерів
        DataGridTextColumn fullNameColumn = new DataGridTextColumn();
        fullNameColumn.Header = "Повне Ім'я Офіцера";
        fullNameColumn.Binding = new System.Windows.Data.Binding("FullName");
        fullNameColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star); // Займає весь доступний простір

        DataGridTextColumn rankColumn = new DataGridTextColumn();
        rankColumn.Header = "Звання";
        rankColumn.Binding = new System.Windows.Data.Binding("Rank");
        // Залиш ширину за замовчуванням або встанови DataGridLength.Auto

        DataGridTextColumn yearsColumn = new DataGridTextColumn();
        yearsColumn.Header = "Стаж (років)";
        yearsColumn.Binding = new System.Windows.Data.Binding("YearsOfService");
        // Залиш ширину за замовчуванням або встанови DataGridLength.SizeToHeader

        DataGrid.Columns.Add(fullNameColumn);
        DataGrid.Columns.Add(rankColumn);
        DataGrid.Columns.Add(yearsColumn);

        // Встановлюємо ItemsSource таблиці на список офіцерів
        // Використовуємо список, який вже є в officerList
        DataGrid.ItemsSource = officerList.Officers;

        DataGrid.Items.Refresh();

        // Оновлюємо заголовок вікна
        Title.Content = "ТАБЛИЦЯ - Офіцери";
    }
}