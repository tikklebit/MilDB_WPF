using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;

namespace MilDB_WPF.dialoges
{
    public partial class SortAndFiltred : Window
    {
        public int? FilterYear { get; private set; }
        public string? FilterCategoryText { get; private set; }
        public string? FilterStatusText { get; private set; }

        public SortAndFiltred()
        {
            InitializeComponent();
            this.StateChanged += SortAndFiltred_StateChanged;
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
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
            this.BeginAnimation(OpacityProperty, fadeOut);
            await Task.Delay(300);

            this.DialogResult = false;
            this.Close();
        }

        private async void HideButton_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
            this.BeginAnimation(OpacityProperty, fadeOut);
            await Task.Delay(100);
            WindowState = WindowState.Minimized;
        }

        private void SortAndFiltred_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1));
                this.BeginAnimation(OpacityProperty, fadeIn);
            }
        }

        private void AcceptF_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(FilterBDate.Text) && int.TryParse(FilterBDate.Text.Trim(), out int year))
            {
                FilterYear = year;
            }
            else
            {
                FilterYear = null;
            }

            FilterCategoryText = (FilterCategory.SelectedItem as ComboBoxItem)?.Content?.ToString();
            FilterStatusText = (FilterStatus.SelectedItem as ComboBoxItem)?.Content?.ToString();

            this.DialogResult = true;
            this.Close();
        }

        private void FilterBDate_PriviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}