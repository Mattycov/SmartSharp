using Smart.Sharp.Engine.Api.Image;
using Smart.Sharp.Engine.Api.Primitives;
using Smart.Sharp.Engine.Script;
using Smart.Sharp.Native.OpenGL;

namespace Smart.Sharp.Engine.Api
{
  public class GLXWrapper : MethodProvider
  {

    private readonly GLX glx;

    public GLXWrapper(Session session, GLX glx) : base(session)
    {
      this.glx = glx;
      int processId = session.SmartRemote.GetClientPID(session.SmartHandle);
      this.glx.Setup(processId);
      this.glx.MapHooks(processId);
      this.glx.SetColourCapture(true);
      this.glx.SetFontCapture(true);
      this.glx.Debug(GLX.DebugMode.None, 0, 0, 0, 0, 0, 0, 800, 600);
    }

    public SmartImage Image()
    {
      return new SmartImage(glx.Image());
    }

    public SmartImage DebugImage()
    {
      return new SmartImage(glx.DebugImage());
    }

    public SmartRectangle Viewport()
    {
      return new SmartRectangle(glx.Viewport());
    }

    public SmartRectangle Viewport(int x, int y, int width, int height)
    {
      return new SmartRectangle(glx.Viewport(x, y, width, height));
    }

    public GLTexture[] Textures()
    {
      return glx.Textures();
    }

    public GLModel[] Models()
    {
      return glx.Models();
    }

    public GLFont[] Fonts()
    {
      return glx.Fonts();
    }

    public GLMatrices Matrices()
    {
      return glx.Matrices();
    }

    public SmartImage MapImage()
    {
      return new SmartImage(glx.Map(new float[4], new float[4]));
    }

    public SmartRectangle MapCoordinates()
    {
      return new SmartRectangle(glx.MapCoordinates());
    }

    public bool SetColourCapture(bool enabled)
    {
      return glx.SetColourCapture(enabled);
    }

    public bool SetFontCapture(bool enabled)
    {
      return glx.SetFontCapture(enabled);
    }

    public bool SaveTexture()
    {
      return glx.SaveTexture();
    }

    public void SetTimout(uint timeout)
    {
      glx.SetTimeout(timeout);
    }

  }
}
