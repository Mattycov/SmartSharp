using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
  internal class SessionsViewModel : ViewModel, IDisposable
  {

    #region variables

    private ObservableCollection<SessionInstanceViewModel> sessions;
    private SessionInstanceViewModel selectedSessionViewModel;

    #endregion

    #region properties

    public ObservableCollection<SessionInstanceViewModel> Sessions
    {
      get { return sessions ?? (sessions = new ObservableCollection<SessionInstanceViewModel>()); }
    }

    public SessionInstanceViewModel SelectedSessionViewModel
    {
      get { return selectedSessionViewModel; }
      set
      {
        if (selectedSessionViewModel == value)
          return;
        selectedSessionViewModel = value;
        OnPropertyChanged(nameof(SelectedSessionViewModel));
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

    private void OnSessionStarted(object sender, EventArgs args)
    {
      Session session = sender as Session;
      if (session == null)
        return;

      Application.Current.Dispatcher.BeginInvoke((Action) (() => Sessions.Add(new SessionInstanceViewModel(Controller, session))));
      session.SessionStarted -= OnSessionStarted;
    }

    private void OnSessionStopped(object sender, EventArgs args)
    {
      Session session = sender as Session;
      if (session == null)
        return;

      SessionInstanceViewModel sessionViewModel = Sessions.FirstOrDefault(vm => vm.Id == session.Id);
      Application.Current.Dispatcher.BeginInvoke((Action)(() => Sessions.Remove(sessionViewModel)));
      session.SessionStopped -= OnSessionStopped;
    }

    #endregion

    #region public methods

    public void CreateSession()
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

      Session session = new Session(Controller.SmartRemote, settings);
      session.SessionStarted += OnSessionStarted;
      session.SessionStopped += OnSessionStopped;
      session.Start();
    }


    public override void Dispose()
    {
    }

    #endregion

  }
}
