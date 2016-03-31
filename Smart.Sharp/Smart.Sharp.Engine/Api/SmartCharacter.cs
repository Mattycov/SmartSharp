using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Sharp.Engine.Api
{
  public class SmartCharacter
  {

    #region variables

    private readonly int[,] colours;

    #endregion

    #region properties

    public int this[int x, int y]
    {
      get { return colours[x, y]; } 
    }

    public int Width
    {
      get { return colours.GetLength(0); }
    }

    public int Height
    {
      get { return colours.GetLength(1); }
    }

    public char Character { get; private set; }

    #endregion

    #region constructors

    public static SmartCharacter LoadCharacter(char c, SmartImage image)
    {
      int[,] pixelColours = new int[image.Width, image.Height];
      image.ReadImage(pixel =>
      {
        pixelColours[pixel.X, pixel.Y] = ((pixel.Red << 16) | (pixel.Green << 8) | pixel.Blue);
      });
      return new SmartCharacter(c, pixelColours);
    }

    private SmartCharacter(char c, int[,] colours)
    {
      Character = c;
      this.colours = colours;
      Console.WriteLine(colours.Rank);
    }

    #endregion

  }
}
