using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Sharp.Engine.Api
{
  public class SmartFont : IEnumerable<SmartCharacter>
  {

    #region variables

    private readonly Dictionary<int, SmartCharacter> charactersDictionary;

    #endregion

    #region properties

    public SmartCharacter this[char c]
    {
      get { return !charactersDictionary.ContainsKey(c) ? null : charactersDictionary[c]; }
      private set { charactersDictionary[c] = value; }
    }

    public string Name { get; private set; }

    #endregion

    #region constructors

    public static SmartFont LoadFont(string path)
    {
      if (!Directory.Exists(path))
        return null;

      string[] files = Directory.GetFiles(path);
      SmartFont font = new SmartFont();
      font.Name = Path.GetFileNameWithoutExtension(path);
      foreach (string file in files)
      {
        string character = Path.GetFileNameWithoutExtension(file);
        SmartImage characterImage = new SmartImage(new Bitmap(file));
        char c = (char) int.Parse(character);
        font[c] = SmartCharacter.LoadCharacter(c, characterImage);
      }
      return font;
    }

    private SmartFont()
    {
      charactersDictionary = new Dictionary<int, SmartCharacter>();
    }

    #endregion

    #region IEnumerable impl

    public IEnumerator<SmartCharacter> GetEnumerator()
    {
      return charactersDictionary.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion

  }
}
