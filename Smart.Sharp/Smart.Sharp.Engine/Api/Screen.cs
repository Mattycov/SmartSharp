using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

    private Bitmap GetBitmapFromSession()
    {
      
      int width = 800;
      int height = 600;
      IntPtr ptr = Session.SmartRemote.GetImageArray(Session.SmartHandle);

      int length = ((width * 32 + 31) / 32) * 4 * height;
      byte[] bytes = new byte[length];
      Marshal.Copy(ptr, bytes, 0, length);

      Bitmap bmp = new Bitmap(width, height);

      int i = 0;
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          int blue = bytes[i++];
          int green = bytes[i++];
          int red = bytes[i++];
          i++;
          bmp.SetPixel(x, y, Color.FromArgb(red, green, blue));
        }
      }
      return bmp;
    }

    public Bitmap FixPixelFormat(Bitmap image)
    {
      return image.Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format24bppRgb);
    }

    #endregion

    #region public methods

    public SmartImage GetScreen()
    {
      return new SmartImage(FixPixelFormat(GetBitmapFromSession()));
    }

    #endregion

  }
}
