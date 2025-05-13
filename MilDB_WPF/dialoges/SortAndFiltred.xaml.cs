using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

namespace MilDB_WPF.dialoges
{
    public partial class SortAndFiltred : Window
    {
        public SortAndFiltred()
        {
            InitializeComponent();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
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

        private void FilterBDate_PriviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach( char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }
        }
    }
}