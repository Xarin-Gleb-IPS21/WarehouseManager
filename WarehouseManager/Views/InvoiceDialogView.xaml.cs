using System.Windows;
using WarehouseManager.App.ViewModels;

namespace WarehouseManager.App.Views;

public partial class InvoiceDialogView : Window
{
    public InvoiceDialogView() => InitializeComponent();

    private void OnSave(object sender, RoutedEventArgs e)
    {
        if (DataContext is InvoiceDialogViewModel vm)
        {
            if (vm.TrySave(out var error))
                DialogResult = true;
            else
                MessageBox.Show(error, "Проверка данных",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void OnCancel(object sender, RoutedEventArgs e) => DialogResult = false;
}