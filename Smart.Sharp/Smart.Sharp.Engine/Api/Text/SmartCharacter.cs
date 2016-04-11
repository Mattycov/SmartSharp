using Smart.Sharp.Engine.Api.Image;

namespace Smart.Sharp.Engine.Api.Text
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
        if (pixel.Red == 255)
          pixelColours[pixel.X, pixel.Y] = ((pixel.Red << 16) | (pixel.Green << 8) | pixel.Blue);
        else
          pixelColours[pixel.X, pixel.Y] = 0;
      });
      return new SmartCharacter(c, pixelColours);
    }

    private SmartCharacter(char c, int[,] colours)
    {
      Character = c;
      this.colours = colours;
    }

    #endregion

    #region public methods

    public bool Match(SmartImage image)
    {
      bool result = true;
      SmartCharacter self = this;
      image.ReadImage(pixel =>
      {
        if (self[pixel.X, pixel.Y] != ((pixel.Red << 16) | (pixel.Green << 8) | pixel.Blue))
          result = false;
      });

      return result;
    }

    #endregion

  }
}
