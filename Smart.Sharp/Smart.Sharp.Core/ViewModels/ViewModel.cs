using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Smart.Sharp.Core.Annotations;
using Smart.Sharp.Core.Controller;

namespace Smart.Sharp.Core.ViewModels
{
  internal class ViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected IAppController Controller { get; private set; }

    public ViewModel(IAppController controller)
    {
      Controller = controller;
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
