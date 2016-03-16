using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.ClearScript.V8;
using Smart.Sharp.Engine.ScriptSystem.Methods;

namespace Smart.Sharp.Engine.ScriptSystem
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
    
    #region public methods

    public void Start(Script script)
    {
      string scriptString = File.ReadAllText(script.Uri);
      
      Stop();
      running = true;

      scriptThread = new Thread(() =>
      {
        using (V8ScriptEngine engine = new V8ScriptEngine())
        {
          engine.AddHostObject("mouse", new Mouse(session));
          while (running)
          {
            engine.Execute(scriptString);
            Thread.Sleep(100);
          }
        }
      });
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
