using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;

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

    private int ManhattanHeuristic(SmartPoint current, SmartPoint other)
    {
      int dx = Math.Abs(current.X - other.X);
      int dy = Math.Abs(current.Y - other.Y);
      return dx + dy;
    }

    private SmartPoint[][] Cluster(SmartPoint[] points, int distance)
    {
      List<SmartPoint[]> result = new List<SmartPoint[]>();
      bool[] checkedPoints = new bool[points.Length];
      Stack<SmartPoint> active = new Stack<SmartPoint>();
      int index = 0;
      int distanceSqrd = distance * distance;

      while (index < points.Length)
      {
        if (!checkedPoints[index])
        {
          List<SmartPoint> cluster = new List<SmartPoint>();
          SmartPoint p = points[index];
          cluster.Add(p);
          checkedPoints[index] = true;
          active.Push(p);
          while (active.Count != 0)
          {
            p = active.Pop();
            int checkedIndex = index + 1;
            while (checkedIndex < points.Length)
            {
              if (!checkedPoints[checkedIndex])
              {
                SmartPoint pCheck = points[checkedIndex];
                if ((pCheck.X - p.X) > distance)
                  break;

                if (ManhattanHeuristic(p, pCheck) <= distanceSqrd)
                {
                  active.Push(pCheck);
                  cluster.Add(pCheck);
                  checkedPoints[checkedIndex] = true;
                  if (checkedIndex == (index + 1))
                    index++;
                }
              }
              checkedIndex++;
            }
          }
          result.Add(cluster.ToArray());
        }
        index++;
      }
      return result.ToArray();
    }

    private SmartPoint[] PointsWithColor(Bitmap bitmap, byte red, byte green, byte blue)
    {
      List<SmartPoint> points = new List<SmartPoint>();

      BitmapData bitmapData =
        bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
          ImageLockMode.ReadWrite, bitmap.PixelFormat);

      int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
      int byteCount = bitmapData.Stride * bitmap.Height;
      byte[] pixels = new byte[byteCount];
      IntPtr ptrFirstPixel = bitmapData.Scan0;
      Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
      int heightInPixels = bitmapData.Height;
      int widthInBytes = bitmapData.Width * bytesPerPixel;

      for (int y = 0; y < heightInPixels - 1; y++)
      {
        int currentLine = y * bitmapData.Stride;
        for (int x = 0; x < widthInBytes - 1; x = x + bytesPerPixel)
        {
          int pixelBlue = pixels[currentLine + x];
          int pixelGreen = pixels[currentLine + x + 1];
          int pixelRed = pixels[currentLine + x + 2];
          if (pixelBlue != blue && pixelGreen != green && pixelRed != red)
            continue;

          points.Add(new SmartPoint(x, y));
        }
      }

      bitmap.UnlockBits(bitmapData);
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

    public void EuclideanColorFilter(byte red, byte green, byte blue, short radius, bool replace, byte replaceRed = 255, byte replaceGreen = 255, byte replaceBlue = 255)
    {
      if (!filtering)
        return;

      EuclideanColorFiltering filter = new EuclideanColorFiltering();
      filter.CenterColor = new RGB(red, green, blue);
      filter.Radius = radius;
      filter.FillColor = new RGB(replaceRed, replaceGreen, replaceBlue);
      filter.FillOutside = replace;
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

    public SmartPoint[] PointsWithColor(byte red, byte green, byte blue)
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

      SmartPoint[] points = PointsWithColor(thresholdBitmap, 255, 255, 255);

      SmartPoint[][] clusters = Cluster(points, distance);
      SmartRectangle[] result = new SmartRectangle[clusters.Length];
      
      for (int i = 0; i < clusters.Length; i++)
      {
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        foreach (SmartPoint point in clusters[i])
        {
          if (point.X > maxX)
            maxX = point.X;
          if (point.X < minX)
            minX = point.X;
          if (point.Y > maxY)
            maxY = point.Y;
          if (point.Y < minY)
            minY = point.Y;
        }
        result[i] = new SmartRectangle(minX, minY, maxX - minX, maxY - minY);
      }
      return result;
    }

    public SmartRectangle[] Blobs()
    {
      counter.MinWidth = 0;
      counter.MinHeight = 0;
      counter.CoupledSizeFiltering = true;
      counter.ProcessImage(image);
      return counter.GetObjectsRectangles().Select(obj => new SmartRectangle(obj)).ToArray();
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
