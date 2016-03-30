using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Smart.Sharp.Engine.Event;
using Smart.Sharp.Engine.Script;
using Smart.Sharp.Native;

namespace Smart.Sharp.Engine
{

  public delegate void SessionStartedEvenHandler(object sender, EventArgs args);
  public delegate void SessionStoppedEvenHandler(object sender, EventArgs args);

  public class Session
  {
    
    #region events

    public event SessionStartedEvenHandler SessionStarted;

    public event SessionStoppedEvenHandler SessionStopped;

    #endregion

    #region variables

    private static int sessionCount = 0;

    private readonly ManualResetEvent resetEvent;

    private Thread smartThread;

    #endregion

    #region properties

    public ScriptController ScriptController { get; private set; }

    public string Id { get { return SmartHandle.ToString(); } }

    public bool Alive
    {
      get { return SmartHandle != IntPtr.Zero; }
    }

    public SmartRemote SmartRemote { get; private set; }

    public IntPtr SmartHandle { get; private set; }
    
    public SessionSettings Settings { get; private set; }

    #endregion

    #region constructor

    public Session(SmartRemote smartRemote, SessionSettings settings)
    {
      SmartRemote = smartRemote;
      Settings = settings;
      resetEvent = new ManualResetEvent(false);
      ScriptController = new ScriptController(this);
      
    }

    #endregion

    #region private methods

    private void StartSmart()
    {
      string url = Settings.SessionType == SessionType.RS3 ? "http://world37.runescape.com/" : "http://oldschool81.runescape.com/";

      string javaPath = Path.Combine(Settings.JavaPath, Settings.ShowConsole ? "java.exe" : "javaw.exe");

      int availableClients = SmartRemote.GetClients(true);
      if (availableClients > 0)
      {
        for (int i = 0; i < availableClients; i++)
        {
          int availableClient = SmartRemote.GetAvailablePID(i);
          IntPtr handle = SmartRemote.PairClient(availableClient);
          if (handle != IntPtr.Zero)
          {
            SmartHandle = handle;
            break;
          }
        }
      }
      if (SmartHandle == IntPtr.Zero)
        SmartHandle = SmartRemote.SpawnClient(javaPath, Settings.SmartPath, url, "", 765, 503, null, null, null, null);

      OnSessionStarted(EventArgs.Empty);

      resetEvent.WaitOne();

      SmartRemote.KillClient(SmartRemote.GetClientPID(SmartHandle));

      OnSessionStopped(EventArgs.Empty);
    }

    #endregion

    #region protected methods

    protected virtual void OnSessionStarted(EventArgs args)
    {
      SessionStarted?.Invoke(this, args);
    }

    protected virtual void OnSessionStopped(EventArgs args)
    {
      SessionStopped?.Invoke(this, args);
    }

    #endregion

    #region public methods

    public void Start()
    {
      smartThread = new Thread(StartSmart);
      Interlocked.Increment(ref sessionCount);
      smartThread.Name = $"SessionThread-{sessionCount}";
      smartThread.IsBackground = true;
      smartThread.Start();
    }

    public void Stop()
    {
      resetEvent.Set();
      smartThread?.Join();
    }

    #endregion

  }
}
