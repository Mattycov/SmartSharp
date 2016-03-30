﻿using System;
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

    private void RunScript(Script script)
    {

      string scriptString = File.ReadAllText(script.Uri);

      // Register Types
      UserData.RegisterType<Mouse>();
      UserData.RegisterType<Keyboard>();
      UserData.RegisterType<Screen>();
      UserData.RegisterType<Ocr>();
      UserData.RegisterType<SmartImage>();
      UserData.RegisterType<SmartRectangle>();
      UserData.RegisterType<SmartPoint>();

      // Create Script
      LuaScript luaScript = new LuaScript();

      SmartScriptLoader loader = new SmartScriptLoader(session.Settings.ModulePaths);
      luaScript.Options.ScriptLoader = loader;

      // Create script instances
      DynValue mouseObject = UserData.Create(new Mouse(session));
      DynValue keyboardObject = UserData.Create(new Keyboard(session));
      DynValue screenObject = UserData.Create(new Screen(session));
      DynValue ocrObject = UserData.Create(new Ocr(session));

      luaScript.Globals.Set("mouse", mouseObject);
      luaScript.Globals.Set("keyboard", keyboardObject);
      luaScript.Globals.Set("screen", screenObject);
      luaScript.Globals.Set("ocr", ocrObject);
      luaScript.Globals["sleep"] = (Action<int>) Thread.Sleep;
      luaScript.Globals["SmartRectangle"] = typeof(SmartRectangle);
      luaScript.Globals["SmartPoint"] = typeof(SmartPoint);
      
      bool executeScript = true;

      while (running && executeScript)
      {
        DynValue result = luaScript.DoString(scriptString);
        executeScript = result.Boolean;
      }
    }

    #endregion

    #region public methods

    public void Start(Script script)
    {
      Stop();
      running = true;

      scriptThread = new Thread(() => RunScript(script));
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
