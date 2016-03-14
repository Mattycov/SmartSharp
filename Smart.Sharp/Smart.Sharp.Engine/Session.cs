using System;
using System.Drawing;
using System.Runtime.InteropServices;
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

    public int Id { get { return SmartHandle.ToInt32(); } }

    public bool IsAlive { get { return SmartRemote.IsActive(SmartHandle); } }
    
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

    public Bitmap GetClientBitmap()
    {
      int width = 800;
      int height = 600;

      int length = ((width * 32 + 31) / 32) * 4 * height;
      byte[] bytes = new byte[length];
      Marshal.Copy(SmartHandle, bytes, 0, length);

      Bitmap bmp = new Bitmap(width, height);
      int i = 0;
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          int blue = bytes[i++];
          int green = bytes[i++];
          int red = bytes[i++];
          i++;
          bmp.SetPixel(x, y, Color.FromArgb(red, green, blue));
        }
      }
      return bmp;
    }

    #endregion

  }
}
