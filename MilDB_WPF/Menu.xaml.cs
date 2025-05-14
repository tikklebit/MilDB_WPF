using System.Windows;

namespace MilDB_WPF;

public partial class Menu : Window
{
    public Menu()
    {
        InitializeComponent();
    }

    private void AdminButton_Click(object sender, EventArgs e)
    {
        this.Hide();
        MainWindow main = new MainWindow();
        main.Show();
    }
}