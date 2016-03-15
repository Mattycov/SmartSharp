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
  internal abstract class ViewModel : INotifyPropertyChanged, IDisposable
  {
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected IAppController Controller { get; private set; }

    protected ViewModel(IAppController controller)
    {
      Controller = controller;
    }

    ~ViewModel()
    {
      Dispose();
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public abstract void Dispose();

  }
}
