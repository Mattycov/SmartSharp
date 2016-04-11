using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smart.Sharp.Native;

namespace Smart.Sharp.Core.Controller
{
  public class AppController : IAppController
  {

    public Native.SmartRemote SmartRemote { get; }

    public AppController()
    {
      SmartRemote = new Native.SmartRemote(Properties.Settings.Default.SmartFolder);
    }
    
  }
}
