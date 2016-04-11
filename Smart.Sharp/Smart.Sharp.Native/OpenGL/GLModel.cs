using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Sharp.Native.OpenGL
{
  [StructLayout(LayoutKind.Sequential)]
  public struct GLModel
  {

    public uint Id;
    public uint Triangles;
    public int X;
    public int Y;

  }
}
