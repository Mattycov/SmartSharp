using System.Windows.Input;
using Smart.Sharp.Core.Controller;
using Smart.Sharp.Core.Helpers;
using Smart.Sharp.Engine;

namespace Smart.Sharp.Core.ViewModels.SessionViewModels
{
  internal class SessionInstanceViewModel : ViewModel
  {

    #region properties

    public string Id { get; private set; }

    public Session Session { get; private set; }

    #endregion

    #region commands

    public ICommand StopSessionCommand { get; private set; }

    #endregion

    #region constructor

    public SessionInstanceViewModel(IAppController controller, Session session) : base(controller)
    {
      Session = session;
      Id = session.Id;
      StopSessionCommand = new DelegateCommand(StopSessionCommandImpl);
    }

    #endregion

    #region private methods

    private void StopSessionCommandImpl()
    {
      Session.Stop();
    }

    #endregion

    public override void Dispose()
    {
      Session.Stop();
    }
  }
}
