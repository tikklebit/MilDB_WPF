using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MilDB_WPF
{
    /// <summary>
    /// Interaction logic for Select.xaml
    /// </summary>
    public partial class Select : Window
    {
        private readonly MilitaryOffice currentOffice;
        public Select()
        {
            InitializeComponent();
            this.StateChanged += Select_StateChanged;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private async void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
            this.BeginAnimation(OpacityProperty, fadeOut);
            await Task.Delay(300);

            this.Hide();
            MainWindow mainWindow = new MainWindow(currentOffice);
            mainWindow.Show();
        }

        private async void HideButton_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
            this.BeginAnimation(OpacityProperty, fadeOut);
            await Task.Delay(100);
            WindowState = WindowState.Minimized;
        }
        private void Select_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));
                this.BeginAnimation(OpacityProperty, fadeIn);
            }
        }

        public void NewConscriptButton_Click(object sender, RoutedEventArgs e)
        {
            NewConscriptWindow newConscriptWindow = new NewConscriptWindow();
            newConscriptWindow.Show();
            this.Hide();
        }

        public void NewOfficerButton_Click(object sender, RoutedEventArgs e)
        {
            NewOfficerWindow newOfficerWindow = new NewOfficerWindow(currentOffice);
            newOfficerWindow.Show();
            this.Hide();
        }
    }
}
