using System;
using Smart.Sharp.Engine.ScriptSystem;
using Smart.Sharp.Native;

namespace Smart.Sharp.Engine
{
  public class Session
  {

    #region fields

    private readonly ScriptController scriptController;
    private readonly SessionSettings sessionSettings;

    #endregion

    #region properties
    
    internal IntPtr SmartHandle { get; private set; }

    internal SmartRemote SmartRemote { get; private set; }
    
    #endregion

    #region constructor

    public Session(SmartRemote smartRemote, SessionSettings settings)
    {
      SmartRemote = smartRemote;
      sessionSettings = settings;
      scriptController = new ScriptController(this);
      SmartHandle = InitSmart();
    }

    #endregion

    #region private methods

    private IntPtr InitSmart()
    {
      string url = sessionSettings.SessionType == SessionType.RS3 ? "http://world37.runescape.com/" : "http://oldschool81.runescape.com/";

      return SmartRemote.SpawnClient(sessionSettings.JavaPath, sessionSettings.SmartPath, url, "", 800, 600, null, null, null, null);
    }

    #endregion

    #region public methods

    public void StartScript(Script script)
    {
      scriptController.Start(script);
    }

    public void StopScript()
    {
      scriptController.Stop();
    }

    public bool Stop()
    {
      return SmartRemote.KillClient(SmartRemote.GetClientPID(SmartHandle));
    }

    #endregion

  }
}
