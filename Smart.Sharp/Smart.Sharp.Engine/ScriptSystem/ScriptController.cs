using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClearScript.Manager;
using Microsoft.ClearScript.V8;

namespace Smart.Sharp.Engine.ScriptSystem
{
  internal class ScriptController
  {

    #region fields

    private readonly Session session;
    private readonly RuntimeManager runtimeManager;
    private Thread scriptThread;
    private volatile bool running;

    #endregion

    #region constructor

    internal ScriptController(Session session)
    {
      this.session = session;
      runtimeManager = new RuntimeManager(new ManagerSettings());
      running = false;

      // Init api
    }

    #endregion

    #region private methods

    private async void Run(List<IncludeScript> script, ExecutionOptions options)
    {
      while (running)
      {
        V8ScriptEngine engine = await runtimeManager.ExecuteAsync(script, options);
        Thread.Sleep(100);
      }
    }

    #endregion

    #region public methods

    public void Start(Script script)
    {
      // Create script
      List<IncludeScript> scriptToRun = new List<IncludeScript> { new IncludeScript { ScriptId = script.Name, Uri = script.Uri } };

      // Add api as HostObjects
      List<HostObject> api = new List<HostObject>();

      // api.Add(color);


      // Generate includes
      List<IncludeScript> includes =
        script.IncludeScripts.Select(include => new IncludeScript() {ScriptId = include.Name, Uri = include.Uri})
          .ToList();

      ExecutionOptions options = new ExecutionOptions
      {
        HostObjects = api,
        Scripts = includes
      };

      Stop();

      scriptThread = new Thread(() => Run(scriptToRun, options));
      scriptThread.Start();
      
    }

    public void Stop()
    {
      if (scriptThread == null)
        return;

      running = false;
      scriptThread.Join();
    }

    #endregion
  }
}
