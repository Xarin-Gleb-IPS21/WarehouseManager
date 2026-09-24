using System.Windows;
using WarehouseManager.App.ViewModels;

namespace WarehouseManager.App.Views;

public partial class WarehouseDialogView : Window
{
    public WarehouseDialogView() => InitializeComponent();

    private void OnSave(object sender, RoutedEventArgs e)
    {
        if (DataContext is WarehouseDialogViewModel vm &&
            !string.IsNullOrWhiteSpace(vm.WarehouseName))
        {
            DialogResult = true;
        }
        else
        {
            MessageBox.Show("Наименование склада не может быть пустым.",
                "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void OnCancel(object sender, RoutedEventArgs e) => DialogResult = false;
}