using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Image = System.Drawing.Image;

namespace Smart.Sharp.Engine.Api
{
  public class SmartImage
  {

    #region variables

    internal Bitmap image;
    private bool filtering;

    private readonly FiltersSequence filterSequence;
    private readonly Crop cropper;
    private readonly BlobCounter counter;


    #endregion

    #region properties

    public int Width
    {
      get { return image.Width; }
    }

    public int Height
    {
      get { return image.Height; }
    }

    #endregion

    #region constructor

    public SmartImage(Bitmap image)
    {
      this.image = image;

      filterSequence = new FiltersSequence();
      cropper = new Crop(new Rectangle(0, 0, 0, 0));
      counter = new BlobCounter();
    }

    #endregion

    #region private methods

    private void ReadImage(Bitmap bitmap, Action<SmartPixel> action)
    {
      BitmapData bitmapData =
        bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
          ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
      
      int byteCount = bitmapData.Stride * bitmap.Height;
      byte[] pixels = new byte[byteCount];
      IntPtr ptrFirstPixel = bitmapData.Scan0;
      Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
      
      int bytesPerPixel = Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
      int widthInBytes = bitmapData.Width * bytesPerPixel;
      
      for (int y = 0; y < bitmapData.Height; y++)
      {
        int currentLine = y * bitmapData.Stride;
        int x = 0;
        for (int xInBytes = 0; xInBytes < widthInBytes; xInBytes += bytesPerPixel, x++)
        {
          byte blue = pixels[currentLine + xInBytes];
          byte green = pixels[currentLine + xInBytes + 1];
          byte red = pixels[currentLine + xInBytes + 2];
          SmartPixel pixel = new SmartPixel(x, y, red, green, blue);
          action(pixel);
        }
      }
      bitmap.UnlockBits(bitmapData);
    }

    private void ReadImage(Bitmap bitmap, Func<SmartPixel, SmartPixel> func)
    {
      BitmapData bitmapData =
        bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
          ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

      int byteCount = bitmapData.Stride * bitmap.Height;
      byte[] pixels = new byte[byteCount];
      IntPtr ptrFirstPixel = bitmapData.Scan0;
      Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);

      int bytesPerPixel = Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
      int widthInBytes = bitmapData.Width * bytesPerPixel;

      for (int y = 0; y < bitmapData.Height; y++)
      {
        int currentLine = y * bitmapData.Stride;
        int x = 0;
        for (int xInBytes = 0; xInBytes < widthInBytes; xInBytes += bytesPerPixel, x++)
        {
          byte blue = pixels[currentLine + xInBytes];
          byte green = pixels[currentLine + xInBytes + 1];
          byte red = pixels[currentLine + xInBytes + 2];
          SmartPixel pixel = new SmartPixel(x, y, red, green, blue);
          pixel = func(pixel);
          pixels[currentLine + xInBytes] = pixel.Blue;
          pixels[currentLine + xInBytes + 1] = pixel.Green;
          pixels[currentLine + xInBytes + 2] = pixel.Red;
        }
      }
      Marshal.Copy(pixels, 0, ptrFirstPixel, byteCount);
      bitmap.UnlockBits(bitmapData);
    }

    private SmartPixel[] PointsWithColor(Bitmap bitmap, byte red, byte green, byte blue)
    {
      List<SmartPixel> points = new List<SmartPixel>();
      ReadImage(bitmap, pixel =>
      {
        if (pixel.Blue != blue || pixel.Green != green || pixel.Red != red)
          return;
        points.Add(pixel);
      });
      return points.ToArray();
    }

    #endregion

    #region public methods

    public void BeginFilter()
    {
      filtering = true;
      filterSequence.Clear();
    }

    public void EndFilter()
    {
      image = filterSequence.Apply(image);
      filterSequence.Clear();
      filtering = false;
    }

    public SmartImage Child(int x, int y, int width, int height)
    {
      cropper.Rectangle = new Rectangle(x, y, width, height);
      Bitmap clone = image.Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format24bppRgb);
      return new SmartImage(cropper.Apply(clone));
    }

    public SmartImage Child(SmartRectangle rectangle)
    {
      return Child(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    }

    public void ColorFilter(int rLo, int rHi, int gLo, int gHi, int bLo, int bHi)
    {
      if (!filtering)
        return;

      ColorFiltering filter = new ColorFiltering();
      filter.Red = new IntRange(rLo, rHi);
      filter.Green = new IntRange(gLo, gHi);
      filter.Blue = new IntRange(bLo, bHi);
      filterSequence.Add(filter);
    }

    public void EuclideanColorFilter(byte red, byte green, byte blue, short radius, bool fillOutside, byte replaceRed = 255, byte replaceGreen = 255, byte replaceBlue = 255)
    {
      if (!filtering)
        return;

      EuclideanColorFiltering filter = new EuclideanColorFiltering();
      filter.CenterColor = new RGB(red, green, blue);
      filter.Radius = radius;
      filter.FillColor = new RGB(replaceRed, replaceGreen, replaceBlue);
      filter.FillOutside = fillOutside;
      filterSequence.Add(filter);
    }

    public void HSLFilter(int hLo, int hHi, float sLo, float sHi, float lLo, float lHi)
    {
      if (!filtering)
        return;

      HSLFiltering filter = new HSLFiltering();
      filter.Hue = new IntRange(hLo, hHi);
      filter.Saturation = new Range(sLo, sHi);
      filter.Luminance = new Range(lLo, lHi);
      filterSequence.Add(filter);
    }

    public void ChannelFilter(int rLo, int rHi, int gLo, int gHi, int bLo, int bHi)
    {
      if (!filtering)
        return;

      ChannelFiltering filter = new ChannelFiltering();
      filter.Red = new IntRange(rLo, rHi);
      filter.Green = new IntRange(gLo, gHi);
      filter.Blue = new IntRange(bLo, bHi);
      filterSequence.Add(filter);
    }

    public void GrayScale()
    {
      if (!filtering)
        return;

      filterSequence.Add(Grayscale.CommonAlgorithms.BT709);
    }


    public void Threshold(int intensity)
    {
      if (!filtering)
        return;

      filterSequence.Add(new Threshold(intensity));
    }

    public SmartPixel[] PointsWithColor(byte red, byte green, byte blue)
    {
      return PointsWithColor(image, red, green , blue);
    }

    public SmartRectangle[] PixelShift(SmartImage smartImage, int distance)
    {
      FiltersSequence sequence = new FiltersSequence();
      sequence.Add(Grayscale.CommonAlgorithms.BT709);
      sequence.Add(new Pixellate(2));

      Bitmap thisImage = sequence.Apply(image);
      Bitmap thatImage = sequence.Apply(smartImage.image);

      Difference difference = new Difference();
      difference.OverlayImage = thatImage;
      Bitmap differenceBitmap = difference.Apply(thisImage);

      Threshold thresholdFilter = new Threshold(15);
      Bitmap thresholdBitmap = thresholdFilter.Apply(differenceBitmap);

      SmartPixel[] pixels = PointsWithColor(thresholdBitmap, 255, 255, 255);

      SmartPixel[][] clusters = SmartPixel.Cluster(pixels, distance);
      
      return SmartPixel.Group(clusters);
    }

    public SmartRectangle[] Blobs()
    {
      counter.MinWidth = 0;
      counter.MinHeight = 0;
      counter.CoupledSizeFiltering = true;
      counter.ProcessImage(image);
      return counter.GetObjectsRectangles().Select(obj => new SmartRectangle(obj)).ToArray();
    }

    public void ReadImage(Action<SmartPixel> action)
    {
      ReadImage(image, action);
    }

    public void ReadImage(Func<SmartPixel, SmartPixel> func)
    {
      ReadImage(image, func);
    }

    public void Save(string fileName)
    {
      image.Save(fileName);
    }

    public void DrawRectangle(SmartRectangle rectangle)
    {
      using (Graphics g = Graphics.FromImage(image))
      using (Pen p = new Pen(Color.Red, 1))
      {
        g.DrawRectangle(p, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height));
      }
    }

    #endregion

  }
}
