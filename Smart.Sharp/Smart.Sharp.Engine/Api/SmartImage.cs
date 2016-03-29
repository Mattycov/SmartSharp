using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace Smart.Sharp.Engine.Api
{
  public class SmartImage
  {



    #region variables

    private Bitmap image;
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

    public void EuclideanColorFilter(byte red, byte green, byte blue, short radius)
    {
      if (!filtering)
        return;

      EuclideanColorFiltering filter = new EuclideanColorFiltering();
      filter.CenterColor = new RGB(red, green, blue);
      filter.Radius = radius;
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

    public SmartRectangle[] Animation(SmartImage smartImage, int pixelSize = 2, int threshold = 10)
    {
      FiltersSequence sequence = new FiltersSequence();
      sequence.Add(Grayscale.CommonAlgorithms.BT709);
      sequence.Add(new Pixellate(pixelSize));

      Bitmap thisImage = sequence.Apply(image);
      Bitmap thatImage = sequence.Apply(smartImage.image);

      Difference difference = new Difference();
      difference.OverlayImage = thatImage;
      Bitmap differenceBitmap = difference.Apply(thisImage);

      Threshold thresholdFilter = new Threshold(threshold);
      Bitmap thresholdBitmap = thresholdFilter.Apply(differenceBitmap);

      thresholdBitmap.Save("threshold.png");

      counter.MinWidth = pixelSize * 2;
      counter.MinHeight = pixelSize * 2;
      counter.CoupledSizeFiltering = true;
      counter.ProcessImage(thresholdBitmap);

      return counter.GetObjectsRectangles().Select(rect => new SmartRectangle(rect)).ToArray();
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
