using System.Windows;
using System.Windows.Input;
using WarehouseManager.App.ViewModels;

namespace WarehouseManager.App.Views;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void OnClose(object sender, RoutedEventArgs e) => Close();

    private void OnItemDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.OpenCommand.CanExecute(null))
            vm.OpenCommand.Execute(null);
    }
}