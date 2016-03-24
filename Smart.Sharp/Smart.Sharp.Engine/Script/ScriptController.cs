using System;
using System.IO;
using System.Threading;
using MoonSharp.Interpreter;
using Smart.Sharp.Engine.Api;
using LuaScript = MoonSharp.Interpreter.Script;

namespace Smart.Sharp.Engine.Script
{
  public class ScriptController
  {

    #region fields

    private readonly Session session;
    private Thread scriptThread;
    private volatile bool running;

    #endregion

    #region constructor

    internal ScriptController(Session session)
    {
      this.session = session;
      running = false;
    }

    #endregion

    #region private methods

    private void RunScript(string scriptString)
    {
      // Register Types
      UserData.RegisterType<Mouse>();
      UserData.RegisterType<Keyboard>();
      UserData.RegisterType<Screen>();
      UserData.RegisterType<SmartImage>();
      UserData.RegisterType<SmartRectangle>();

      // Create Script
      LuaScript script = new LuaScript();

      // Create script instances
      DynValue mouseObject = UserData.Create(new Mouse(session));
      DynValue keyboardObject = UserData.Create(new Keyboard(session));
      DynValue screenObject = UserData.Create(new Screen(session));

      script.Globals.Set("mouse", mouseObject);
      script.Globals.Set("keyboard", keyboardObject);
      script.Globals.Set("screen", screenObject);
      script.Globals["sleep"] = (Action<int>) Thread.Sleep;

      bool executeScript = true;

      while (running && executeScript)
      {
        DynValue result = script.DoString(scriptString);
        executeScript = result.Boolean;
      }
    }

    #endregion

    #region public methods

    public void Start(Engine.Script.Script script)
    {
      string scriptString = File.ReadAllText(script.Uri);
      
      Stop();
      running = true;

      scriptThread = new Thread(() => RunScript(scriptString));
      scriptThread.Name = $"Script-{script.Name}";
      scriptThread.IsBackground = true;
      scriptThread.Start();
    }

    public void Stop()
    {
      running = false;
      scriptThread?.Join();
    }

    #endregion
  }
}
