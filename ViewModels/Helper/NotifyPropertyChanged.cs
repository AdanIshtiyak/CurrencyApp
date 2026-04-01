using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CurrencyApp.ViewModels.Helper
{
  public class NotifyPropertyChanged : INotifyPropertyChanged
  {
    #region INotifyPropertyChanged implementation

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}
