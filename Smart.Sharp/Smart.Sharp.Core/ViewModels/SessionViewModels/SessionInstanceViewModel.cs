using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Smart.Sharp.Core.Controller;
using Smart.Sharp.Engine;

namespace Smart.Sharp.Core.ViewModels.SessionViewModels
{
  internal class SessionInstanceViewModel : ViewModel, IDisposable
  {

    #region variables

    private Session session;

    #endregion

    #region properties

    public string Id { get; private set; }
    
    #endregion

    #region constructor

    public SessionInstanceViewModel(IAppController controller, Session session) : base(controller)
    {
      this.session = session;
      Id = session.Id.ToString();
    }

    #endregion

    public void Dispose()
    {
      session.Stop();
    }
  }
}
