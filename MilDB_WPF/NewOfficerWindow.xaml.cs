using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MilDB_WPF;

public partial class NewOfficerWindow : Window
{
    public NewOfficerWindow()
    {
        InitializeComponent();
        StateChanged += NOW_StateChanged;
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
        
    }
}