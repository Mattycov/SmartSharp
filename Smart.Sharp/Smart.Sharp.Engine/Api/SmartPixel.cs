using System;
using System.Collections.Generic;
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

    #region static methods

    private static int ManhattanHeuristic(SmartPixel current, SmartPixel other)
    {
      int dx = Math.Abs(current.X - other.X);
      int dy = Math.Abs(current.Y - other.Y);
      return dx + dy;
    }

    public static SmartPixel[][] Cluster(SmartPixel[] pixels, int distance)
    {
      List<SmartPixel[]> result = new List<SmartPixel[]>();
      bool[] checkedPoints = new bool[pixels.Length];
      Stack<SmartPixel> active = new Stack<SmartPixel>();
      int index = 0;
      int distanceSqrd = distance * distance;

      while (index < pixels.Length)
      {
        if (!checkedPoints[index])
        {
          List<SmartPixel> cluster = new List<SmartPixel>();
          SmartPixel p = pixels[index];
          cluster.Add(p);
          checkedPoints[index] = true;
          active.Push(p);
          while (active.Count != 0)
          {
            p = active.Pop();
            int checkedIndex = index + 1;
            while (checkedIndex < pixels.Length)
            {
              if (!checkedPoints[checkedIndex])
              {
                SmartPixel pCheck = pixels[checkedIndex];
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

    public static SmartRectangle[] Group(SmartPixel[][] clusters)
    {
      SmartRectangle[] result = new SmartRectangle[clusters.Length];
      for (int i = 0; i < clusters.Length; i++)
      {
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        foreach (SmartPixel point in clusters[i])
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
        int width = maxX - minX;
        int height = maxY - minY;
        result[i] = new SmartRectangle(minX, minY, width == 0 ? 1 : width, height == 0 ? 1 : height);
      }
      return result;
    }

    #endregion

  }
}
