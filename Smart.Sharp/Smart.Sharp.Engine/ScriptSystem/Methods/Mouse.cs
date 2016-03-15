using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Sharp.Engine.ScriptSystem.Methods
{
  public class Mouse : MethodProvider
  {
    public Mouse(Session session) : base(session)
    {
    }

    public void WindMouse(int x, int y)
    {
      Session.SmartRemote.WindMouse(Session.SmartHandle, x, y);
    }
  }
}
