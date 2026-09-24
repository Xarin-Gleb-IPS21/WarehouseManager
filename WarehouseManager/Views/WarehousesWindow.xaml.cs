using System.Windows;

namespace WarehouseManager.App.Views;

public partial class WarehousesWindow : Window
{
    public WarehousesWindow() => InitializeComponent();
    private void OnClose(object sender, RoutedEventArgs e) => Close();
}