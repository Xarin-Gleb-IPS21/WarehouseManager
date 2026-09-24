using System.Windows;
using WarehouseManager.App.ViewModels;

namespace WarehouseManager.App.Views;

public partial class OrganizationDialogView : Window
{
    public OrganizationDialogView() => InitializeComponent();

    private void OnSave(object sender, RoutedEventArgs e)
    {
        if (DataContext is OrganizationDialogViewModel vm &&
            !string.IsNullOrWhiteSpace(vm.OrganizationName))
        {
            DialogResult = true;
        }
        else
        {
            MessageBox.Show("Наименование не может быть пустым.",
                "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void OnCancel(object sender, RoutedEventArgs e) => DialogResult = false;
}