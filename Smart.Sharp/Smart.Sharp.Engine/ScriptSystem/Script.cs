using System.Collections.Generic;

namespace Smart.Sharp.Engine.ScriptSystem
{
  public class Script
  {

    public string Name { get; set; }

    public string Uri { get; set; }

    public List<Script> IncludeScripts { get; set; } 

  }
}
