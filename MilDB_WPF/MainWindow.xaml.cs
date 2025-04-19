using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace MilDB_WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ConscriptList conscriptList;
    private Conscript conscript;

    public MainWindow()
    {
        InitializeComponent();
        conscriptList = new ConscriptList();
        conscript = new Conscript();
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

    private void OpenSelectWindow_Click(object sender, EventArgs e)
    {
        Select selectWindow = new Select();
        selectWindow.Show();
        this.Hide();
    }

    private void UpdateTableButton_Click(object sender, EventArgs e)
    {
        conscriptList.Deserialize();
        foreach (Conscript conscript in conscriptList.Conscripts)
        {
            string birthdate = $"{conscript.BirthDate.Year}.{conscript.BirthDate.Month}.{conscript.BirthDate.Day}";
            DataGrid.ItemsSource = conscriptList.Conscripts;
        }   
    }

    private void ChangeInfoButton_Click(object sender, EventArgs e)
    {
        if (DataGrid.SelectedItem is Conscript selectedConscript)
        {
            int index = conscriptList.Conscripts.IndexOf(selectedConscript);
            conscriptList.EditConscript(conscript, index);
            DataGrid.Items.Refresh();
        }
    }
}