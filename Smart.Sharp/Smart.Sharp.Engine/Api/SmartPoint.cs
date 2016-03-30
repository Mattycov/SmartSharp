using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Sharp.Engine.Api
{
  public class SmartPoint
  {

    public int X { get; set; }

    public int Y { get; set; }

    internal SmartPoint(int x, int y)
    {
      X = x;
      Y = y;
    }

    internal SmartPoint(SmartPoint point)
    {
      X = point.X;
      Y = point.Y;
    }

    public static SmartPoint New(int x, int y)
    {
      return new SmartPoint(x, y);
    }

  }
}
