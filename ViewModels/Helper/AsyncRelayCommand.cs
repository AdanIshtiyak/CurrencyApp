using System.Windows.Input;

namespace CurrencyApp.ViewModels.Helper
{
  public class AsyncRelayCommand : ICommand
  {
    private readonly Func<Task> _execute;

    public AsyncRelayCommand(Func<Task> execute)
    {
      _execute = execute;
    }

    public async void Execute(object parameter)
    {
      await _execute();
    }

    public bool CanExecute(object parameter) => true;

    public event EventHandler? CanExecuteChanged;
  }

  public class AsyncRelayCommand<T> : ICommand
  {
    private readonly Func<T, Task> _execute;

    public AsyncRelayCommand(Func<T, Task> execute)
    {
      _execute = execute;
    }

    public async void Execute(object parameter)
    {
      if (parameter is T param)
        await _execute(param);
    }

    public bool CanExecute(object parameter) => true;

    public event EventHandler CanExecuteChanged;
  }
}