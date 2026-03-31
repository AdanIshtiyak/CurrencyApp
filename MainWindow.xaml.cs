using CurrencyApp.ViewModels;
using System.Windows;

namespace CurrencyApp
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow(MainWindowViewModel viewModel)
    {
      InitializeComponent();

      DataContext = viewModel;
    }
  }
}