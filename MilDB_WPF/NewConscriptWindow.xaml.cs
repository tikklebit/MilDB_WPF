using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MilDB_WPF
{
    /// <summary>
    /// Interaction logic for NewConscriptWindow.xaml
    /// </summary>
    public partial class NewConscriptWindow : Window
    {
        public NewConscriptWindow()
        {
            InitializeComponent();
            this.StateChanged += MainWindow_StateChanged;
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

        private async void ExitButton_Click(object sender, EventArgs e)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
            this.BeginAnimation(OpacityProperty, fadeOut);
            await Task.Delay(100);
            this.Hide();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string name = FullNameTextBox.Text;
            DatePicker birthDate = BirthDatePicker;
            string address = AddressTextBox.Text;
            string healthStatus = HealthStatusTextBox.Text;
            string fitnessCategory = FitnessCategoryComboBox.Text;
            string status = StatusComboBox.Text;


            if (birthDate.SelectedDate.HasValue)
            {
                Conscript? conscript = new Conscript(name, birthDate.SelectedDate.Value, address, healthStatus, fitnessCategory, status);
                if (conscript.Valid())
                {
                    ConscriptList conscriptList = new ConscriptList();
                    conscriptList.AddConscript(conscript);
                    string greatMessage = conscript.GetCheck();
                    ErrorLabel.Content = Visibility.Visible;
                }
                else
                {
                    string errorMessage = conscript.GetCheck();
                    ErrorLabel.Content = errorMessage;
                    ErrorLabel.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show("Оберіть дату народження!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
