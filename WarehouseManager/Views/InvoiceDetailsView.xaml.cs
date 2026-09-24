using System.Windows;

namespace WarehouseManager.App.Views;

public partial class InvoiceDetailsView : Window
{
    public InvoiceDetailsView() => InitializeComponent();
    private void OnClose(object sender, RoutedEventArgs e) => Close();
}