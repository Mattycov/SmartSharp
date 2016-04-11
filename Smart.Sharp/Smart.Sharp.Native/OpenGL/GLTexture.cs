using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Sharp.Native.OpenGL
{
  [StructLayout(LayoutKind.Sequential)]
  public struct GLTexture
  {
    public uint Id;
    public uint ColourId;
    public uint FullColourId;
    public int X;
    public int Y;
    // Bounding box
    public int X1;
    public int Y1;
    public int X2;
    public int Y2;

  }
}
