using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Sharp.Engine
{
  public class SessionSettings
  {

    public string JavaPath { get; set; }

    public string SmartPath { get; set; }

    public string[] ModulePaths { get; set; }

    public string TesseractPath { get; set; }

    public SessionType SessionType { get; set; }

    public bool ShowConsole { get; set; }

  }
}
