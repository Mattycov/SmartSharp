using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
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

    private Bitmap GetBitmap()
    {
      int width = 765;
      int height = 503;
      IntPtr ptr = Session.SmartRemote.GetImageArray(Session.SmartHandle);

      int length = ((width * 32 + 31) / 32) * 4 * height;
      byte[] bytes = new byte[length];
      Marshal.Copy(ptr, bytes, 0, length);

      Bitmap bmp = new Bitmap(width, height);
      BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
      Marshal.Copy(bytes, 0, data.Scan0, length);
      bmp.UnlockBits(data);
      return bmp;
    }

    private Bitmap FixPixelFormat(Bitmap image)
    {
      return image.Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format24bppRgb);
    }

    #endregion

    #region public methods

    public SmartImage GetScreen()
    {
      return new SmartImage(FixPixelFormat(GetBitmap()));
    }

    #endregion

  }
}
