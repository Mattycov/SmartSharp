using System.Drawing;

namespace Smart.Sharp.Engine.Api
{
  public struct SmartPixel
  {

    #region properties

    public int X { get; set; }

    public int Y { get; set; }

    public byte Red { get; set; }

    public byte Green { get; set; }

    public byte Blue { get; set; }

    #endregion

    #region constructors

    internal SmartPixel(int x, int y, byte red, byte green, byte blue)
    {
      X = x;
      Y = y;
      Red = red;
      Green = green;
      Blue = blue;
    }

    internal SmartPixel(int x, int y) : this(x, y, 0, 0, 0) {}

    internal SmartPixel(Point point)
    {
      X = point.X;
      Y = point.Y;
      Red = 0;
      Green = 0;
      Blue = 0;
    }

    public static SmartPixel New(int x, int y)
    {
      return new SmartPixel(x, y);
    }

    public static SmartPixel New(int x, int y, byte red, byte green, byte blue)
    {
      return new SmartPixel(x, y, red, green, blue);
    }

    #endregion

  }
}
