using System.Windows;
using WarehouseManager.App.ViewModels;

namespace WarehouseManager.App.Views;

public partial class ConfirmDeleteDialogView : Window
{
    public ConfirmDeleteDialogView() => InitializeComponent();

    private void OnConfirm(object sender, RoutedEventArgs e)
    {
        if (DataContext is ConfirmDeleteViewModel vm && vm.IsMatch)
            DialogResult = true;
    }

    private void OnCancel(object sender, RoutedEventArgs e) => DialogResult = false;
}