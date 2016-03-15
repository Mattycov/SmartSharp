using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smart.Sharp.Core.Controller;
using Smart.Sharp.Core.ViewModels.SessionViewModels;
using Smart.Sharp.Core.ViewModels.SettingsViewModels;

namespace Smart.Sharp.Core.ViewModels
{
  internal class AppViewModel : ViewModel
  {

    #region properties

    public SettingsViewModel SettingsViewModel { get; private set; }

    public SessionsViewModel SessionsViewModel { get; private set; }

    #endregion

    public AppViewModel(IAppController controller) : base(controller)
    {
      SettingsViewModel = new SettingsViewModel(controller);
      SessionsViewModel = new SessionsViewModel(controller);
    }

    public override void Dispose()
    {
      
    }
  }
}
