using CurrencyApp.Interfaces;
using System.Windows.Input;

namespace CurrencyApp.ViewModels.Helper
{
  public class AsyncRelayCommand : ICommand
  {
    private readonly Func<Task> _execute;
    private readonly ISessionServices _sessionServices;

    public AsyncRelayCommand(Func<Task> execute, ISessionServices sessionService)
    {
      _execute = execute;
      _sessionServices = sessionService;
    }

    public async void Execute(object parameter)
    {
      await _sessionServices.UpdateLastActivityAt();

      await _execute();
    }

    public bool CanExecute(object parameter) => true;

    public event EventHandler? CanExecuteChanged;
  }

  public class AsyncRelayCommand<T> : ICommand
  {
    private readonly Func<T, Task> _execute;
    private readonly ISessionServices _sessionServices;

    public AsyncRelayCommand(Func<T, Task> execute, ISessionServices sessionService)
    {
      _execute = execute;
      _sessionServices = sessionService;
    }

    public async void Execute(object parameter)
    {
      await _sessionServices.UpdateLastActivityAt();

      if (parameter is T param)
        await _execute(param);
    }

    public bool CanExecute(object parameter) => true;

    public event EventHandler CanExecuteChanged;
  }
}