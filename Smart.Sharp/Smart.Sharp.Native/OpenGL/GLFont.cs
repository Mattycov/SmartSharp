﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Sharp.Native.OpenGL
{
  [StructLayout(LayoutKind.Sequential)]
  public struct GLFont
  {

    public uint TextureId;
    public uint Colour;
    public int X;
    public int Y;
    public char Letter;
    public bool Shadow;
    public float[] Translate;
    // Bounding Box
    public int X1;
    public int Y1;
    public int X2;
    public int Y2;

  }
}
