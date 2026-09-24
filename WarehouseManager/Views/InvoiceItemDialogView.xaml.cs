using System.Windows;
using WarehouseManager.App.ViewModels;

namespace WarehouseManager.App.Views;

public partial class InvoiceItemDialogView : Window
{
    public InvoiceItemDialogView() => InitializeComponent();

    private void OnOk(object sender, RoutedEventArgs e)
    {
        if (DataContext is InvoiceItemDialogViewModel vm)
        {
            if (vm.TryBuild(out var error))
                DialogResult = true;
            else
                MessageBox.Show(error, "Проверка данных",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void OnCancel(object sender, RoutedEventArgs e) => DialogResult = false;
}