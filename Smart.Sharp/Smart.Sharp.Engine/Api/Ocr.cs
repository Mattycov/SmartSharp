using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smart.Sharp.Engine.Script;
using Tesseract;

namespace Smart.Sharp.Engine.Api
{
  public class Ocr : MethodProvider
  {
    public Ocr(Session session) : base(session)
    {
    }

    public string Text(SmartImage image)
    {
      string result = string.Empty;
      image.BeginFilter();
      // Convert other colours
      image.EuclideanColorFilter(222, 222, 0, 15, false);
      image.EuclideanColorFilter(217, 217, 217, 15, false);
      // Black out everything but white
      image.EuclideanColorFilter(255, 255, 255, 15, true, 0, 0, 0);
      image.EndFilter();
      image.Save("ocr.png");

      using (TesseractEngine engine = new TesseractEngine(Session.Settings.TesseractPath, "eng", EngineMode.CubeOnly))
      {
        using (Page page = engine.Process(image.image))
        {
          result = page.GetText();
        }
      }
      return result;
    }

  }
}
