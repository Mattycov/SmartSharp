using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Smart.Sharp.Core.Controller;
using Smart.Sharp.Core.Helpers;
using Smart.Sharp.Engine;

namespace Smart.Sharp.Core.ViewModels.SessionViewModels
{
  internal class SessionsViewModel : ViewModel
  {

    #region variables

    private ObservableCollection<SessionInstanceViewModel> sessions;
    private SessionInstanceViewModel selectedSession;

    #endregion

    #region properties

    public ObservableCollection<SessionInstanceViewModel> Sessions
    {
      get { return sessions ?? (sessions = new ObservableCollection<SessionInstanceViewModel>()); }
    }

    public SessionInstanceViewModel SelectedSession
    {
      get { return selectedSession; }
      set
      {
        if (selectedSession == value)
          return;
        selectedSession = value;
        OnPropertyChanged(nameof(SelectedSession));
      }
    }

    #endregion

    #region commands

    public ICommand AddSessionCommand { get; private set; }

    #endregion

    #region constructor

    public SessionsViewModel(IAppController controller) : base(controller)
    {
      AddSessionCommand = new DelegateCommand(AddSessionCommandImpl);
    }

    #endregion

    #region private methods

    private void AddSessionCommandImpl()
    {
      CreateSession();
    }

    public async void CreateSession()
    {
      string javaPath = Properties.Settings.Default.JavaPath;
      if (string.IsNullOrEmpty(javaPath))
      {
        MessageBox.Show("Warning", "Java Path has not been set in Settings");
        return;
      }

      string smartPath = Properties.Settings.Default.SmartFolder;
      if (string.IsNullOrEmpty(smartPath))
      {
        MessageBox.Show("Warning", "SMART Path has not been set in Settings");
        return;
      }
      SessionSettings settings = new SessionSettings();
      settings.SessionType = SessionType.OldSchool;
      settings.JavaPath = javaPath;
      settings.SmartPath = smartPath;

      Session session = await InitSession(settings);
      Dispatcher.CurrentDispatcher.BeginInvoke((Action) (() => Sessions.Add(new SessionInstanceViewModel(Controller, session))));
    }

    private Task<Session> InitSession(SessionSettings settings)
    {
      return Task.Run(() => new Session(Controller.SmartRemote, settings));
    }

    #endregion
  }
}
