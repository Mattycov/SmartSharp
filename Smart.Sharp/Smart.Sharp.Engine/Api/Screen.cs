using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Smart.Sharp.Engine.Api.Image;
using Smart.Sharp.Engine.Script;

namespace Smart.Sharp.Engine.Api
{
  public class Screen : MethodProvider
  {

    #region constructor

    public Screen(Session session) : base(session)
    {
    }

    #endregion

    #region private methods

    private Bitmap GetImage(IntPtr ptr)
    {
      int width = 800;
      int height = 600;

      int length = ((width * 32 + 31) / 32) * 4 * height;
      byte[] bytes = new byte[length];
      Marshal.Copy(ptr, bytes, 0, length);

      Bitmap bmp = new Bitmap(width, height);
      BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
      Marshal.Copy(bytes, 0, data.Scan0, length);
      bmp.UnlockBits(data);
      return bmp;
    }

    private void SetImage(Bitmap bmp, IntPtr ptr)
    {
      int width = 800;
      int height = 600;

      int length = ((width * 32 + 31) / 32) * 4 * height;
      byte[] bytes = new byte[length];
      BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
      Marshal.Copy(data.Scan0, bytes, 0, length);
      
      Marshal.Copy(bytes, 0, ptr, length);
      bmp.UnlockBits(data);
    }

    private Bitmap FixPixelFormat(Bitmap image, PixelFormat format)
    {
      return image.Clone(new Rectangle(0, 0, image.Width, image.Height), format);
    }

    #endregion

    #region public methods

    public SmartImage GetScreen()
    {
      return new SmartImage(FixPixelFormat(GetImage(Session.SmartRemote.GetImageArray(Session.SmartHandle)), PixelFormat.Format24bppRgb));
    }

    public SmartImage GetDebug()
    {
      return new SmartImage(FixPixelFormat(GetImage(Session.SmartRemote.GetDebugArray(Session.SmartHandle)), PixelFormat.Format24bppRgb));
    }

    public void SetDebug(SmartImage image)
    {
      SetImage(FixPixelFormat(image.image, PixelFormat.Format32bppArgb), Session.SmartRemote.GetDebugArray(Session.SmartHandle));
    }

    #endregion

  }
}
