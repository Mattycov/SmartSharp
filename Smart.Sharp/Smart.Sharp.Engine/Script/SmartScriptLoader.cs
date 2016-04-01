using System.IO;
using System.Linq;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace Smart.Sharp.Engine.Script
{
  internal class SmartScriptLoader: ScriptLoaderBase
  {

    #region constructor

    public SmartScriptLoader(string[] modulePaths)
    {
      ModulePaths = modulePaths.Select(path => Path.Combine(path, "?.lua")).ToArray();
    }

    #endregion

    #region ScriptLoaderBase impl
    
    public override bool ScriptFileExists(string name)
    {
      return File.Exists(name);
    }

    public override object LoadFile(string file, Table globalContext)
    {
      return new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }

    #endregion

  }
}
