using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smart.Sharp.Engine.Api.Image;
using Smart.Sharp.Engine.Api.Primitives;
using Smart.Sharp.Engine.Api.Text;
using Smart.Sharp.Engine.Script;
using Tesseract;

namespace Smart.Sharp.Engine.Api
{
  public class Ocr : MethodProvider
  {

    private static readonly SmartPixel White = SmartPixel.New(0, 0, 255, 255, 255);
    private static readonly SmartPixel Cyan = SmartPixel.New(0, 0, 0, 255, 255);
    private static readonly SmartPixel Yellow = SmartPixel.New(0, 0, 255, 255, 0);
    private static readonly SmartPixel Red = SmartPixel.New(0, 0, 255, 0, 0);
    private static readonly SmartPixel Green = SmartPixel.New(0, 0, 0, 255, 0);
    private static readonly SmartPixel Black = SmartPixel.New(0, 0, 0, 0, 0);

    private SmartFont[] fonts;

    public Ocr(Session session) : base(session)
    {
      fonts = new SmartFont[0];
    }

    public void Init()
    {
      fonts = new SmartFont[Session.Settings.FontPaths.Length];
      for (int i = 0; i < Session.Settings.FontPaths.Length; i++)
      {
        fonts[i] = SmartFont.LoadFont(Session.Settings.FontPaths[i]);
      }
    }

    public string Tesseract(SmartImage image)
    {
      string result;
      image.ReadImage(pixel => NormalisePixel(pixel));
      SmartPixel[] pointsWithColour = image.PointsWithColor(255, 255, 255);
      int left = int.MaxValue;
      int right = int.MinValue;
      foreach (SmartPixel pixel in pointsWithColour)
      {
        if (pixel.X < left)
          left = pixel.X;
        if (pixel.X > right)
          right = pixel.X;
      }
      SmartRectangle rect = SmartRectangle.New(left - 1, 0, right - left + 3, image.Height);
      SmartImage words = image.Child(rect);

      using (TesseractEngine engine = new TesseractEngine(Session.Settings.TesseractPath, "eng", EngineMode.CubeOnly))
      using (Page page = engine.Process(words.image))
      { 
        result = page.GetText();
      }
      return result;
    }

    public string UpText(SmartImage image, string fontName)
    {
      image.ReadImage(pixel => NormalisePixel(pixel));
      SmartPixel[] whitePoints = image.PointsWithColor(255, 255, 255);
      SmartPixel[] redPoints = image.PointsWithColor(255, 0, 0);
      int left = int.MaxValue;
      int right = int.MinValue;
      foreach (SmartPixel pixel in whitePoints)
      {
        if (pixel.X < left)
          left = pixel.X;
        if (pixel.X > right)
          right = pixel.X;
      }
      foreach (SmartPixel pixel in redPoints)
      {
        if (pixel.X < left)
          left = pixel.X;
        if (pixel.X > right)
          right = pixel.X;
      }
      SmartRectangle rect = SmartRectangle.New(left - 1, 0, right - left + 3, image.Height);
      SmartImage words = image.Child(rect);
      StringBuilder resultBuilder = new StringBuilder();
      SmartFont font = fonts.FirstOrDefault(fnt => fnt.Name == fontName);
      if (font == null)
        return string.Empty;

      int x = 0;
      int y = 0;
      bool found = false;
      // Find first letter
      for (; y < image.Height; y++)
      {
        foreach (SmartCharacter character in font)
        {
          SmartImage charImage = words.Child(x, y, character.Width, character.Height);
          if (!character.Match(charImage))
            continue;

          resultBuilder.Append(character.Character);
          x += character.Width;
          found = true;
          break;
        }
        if (found)
          break;
      }

      // Find rest of the letters
      while (x < words.Width)
      {
        found = false;
        foreach (SmartCharacter character in font)
        {
          SmartImage charImage = words.Child(x, y, character.Width, character.Height);
          if (!character.Match(charImage))
            continue;

          resultBuilder.Append(character.Character);
          x += character.Width;
          found = true;
          break;
        }
        if (!found)
          x++;
      }
      return resultBuilder.ToString();
    }

    private int DistanceSquared(SmartPixel one, SmartPixel two)
    {
      int red = one.Red - two.Red;
      int green = one.Red - two.Red;
      int blue = one.Red - two.Red;
      return red * red + green * green + blue * blue;
    }

    private SmartPixel NormalisePixel(SmartPixel pixel)
    {
      if (DistanceSquared(pixel, Black) <= 10000)
      {
        pixel.Red = 255;
        pixel.Green = 0;
        pixel.Blue = 0;
        return pixel;
      }
      if (DistanceSquared(pixel, White) <= 10000 || DistanceSquared(pixel, Cyan) <= 10000
          || DistanceSquared(pixel, Yellow) <= 10000 || DistanceSquared(pixel, Red) <= 10000
          || DistanceSquared(pixel, Green) <= 10000)
      {
        pixel.Red = 255;
        pixel.Green = 255;
        pixel.Blue = 255;
        return pixel;
      }
      pixel.Red = 0;
      pixel.Green = 0;
      pixel.Blue = 0;
      return pixel;
    }

  }
}
