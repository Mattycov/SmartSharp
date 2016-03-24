using System.Drawing;

namespace Smart.Sharp.Engine.Api
{
  public class SmartRectangle
  {

    public int X { get; private set; }

    public int Y { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    internal SmartRectangle(int x, int y, int width, int height)
    {
      X = x;
      Y = y;
      Width = width;
      Height = height;
    }

    internal SmartRectangle(Rectangle rectangle)
    {
      X = rectangle.X;
      Y = rectangle.Y;
      Width = rectangle.Width;
      Height = rectangle.Height;
    }

  }
}
