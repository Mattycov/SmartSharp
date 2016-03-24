using System.Drawing;
using Smart.Sharp.Engine.Script;

namespace Smart.Sharp.Engine.Api
{
  public class Mouse : MethodProvider
  {
    
    #region constructor

    public Mouse(Session session) : base(session)
    {
    }

    #endregion

    #region public methods
    
    public void MoveMouse(int x, int y)
    {
      Session.SmartRemote.WindMouse(Session.SmartHandle, x, y);
    }

    public void ClickMouse(bool left)
    {
      Point p = GetPosition();
      Session.SmartRemote.ClickMouse(Session.SmartHandle, p.X, p.Y, left);
    }

    public void HoldButton(bool left)
    {
      Point p = GetPosition();
      Session.SmartRemote.HoldMouse(Session.SmartHandle, p.X, p.Y, left);
    }

    public void ReleaseButton(bool left)
    {
      Point p = GetPosition();
      Session.SmartRemote.ReleaseMouse(Session.SmartHandle, p.X, p.Y, left);
    }

    #endregion

    #region private methods

    private Point GetPosition()
    {
      int x = 0;
      int y = 0;
      Session.SmartRemote.GetMousePos(Session.SmartHandle, ref x, ref y);
      return new Point(x, y);
    }

    #endregion
  }
}
