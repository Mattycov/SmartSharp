using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Smart.Sharp.Native.OpenGL
{
  public class GLX : IDisposable
  {

    #region extras

    [Flags]
    public enum DebugMode
    {
      None = 0,
      Textures = 1,
      Models = 2,
      Fonts = 3,
      Display = 4
    }

    #endregion

    #region delegates

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate bool GLXSetupDelegate(int processId);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate bool GLXMapHooksDelegate(int processId);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr GLXImagePointerDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr GLXDebugPointerDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate bool GLXViewPortDelegate(out int x, out int y, out int x1, out int x2);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr GLXTexturesDelegate(out uint size);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr GLXModelsDelegate(out uint size);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr GLXFontsDelegate(out uint size);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr GLXMatricesDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr GLXMapDelegate(int width, int height, float[] x, float[] y);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate bool GLXMapCoordinatesDelegate(out float[] x, out float[] y);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate bool GLXDebugDelegate(
      int mode, uint textureId, uint colourId, uint fullColourId, int tolerance, int x, int y, int x1, int y1);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate bool GLXSetFontCaptureDelegate(bool enabled);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate bool GLXSetColourCaptureDelegate(bool enabled);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate bool GLXSaveTexturesDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void GLXSetTimeoutDelegate(long timeout);

    #endregion

    #region variables

    private readonly IntPtr modulePtr;

    private GLXSetupDelegate GLXSetupImpl;
    private GLXMapHooksDelegate GLXMapHooksImpl;
    private GLXImagePointerDelegate GLXImagePointerImpl;
    private GLXDebugPointerDelegate GLXDebugPointerImpl;
    private GLXViewPortDelegate GLXViewPortImpl;
    private GLXTexturesDelegate GLXTexturesImpl;
    private GLXModelsDelegate GLXModelsImpl;
    private GLXFontsDelegate GLXFontsImpl;
    private GLXMatricesDelegate GLXMatricesImpl;
    private GLXMapDelegate GLXMapImpl;
    private GLXMapCoordinatesDelegate GLXMapCoordinatesImpl;
    private GLXDebugDelegate GLXDebugImpl;
    private GLXSetFontCaptureDelegate GLXSetFontCaptureImpl;
    private GLXSetColourCaptureDelegate GLXSetColourCaptureImpl;
    private GLXSaveTexturesDelegate GLXSaveTexturesImpl;
    private GLXSetTimeoutDelegate GLXSetTimeoutImpl;
    #endregion

    #region constructor

    public GLX(string dllPath)
    {
      string dll = Path.GetFileName(dllPath);
      if (string.IsNullOrEmpty(dll) || dll != "GLX.dll")
        throw new FileNotFoundException("GLX.dll");

      modulePtr = Kernel32.LoadLibrary(dllPath);
      if (modulePtr == IntPtr.Zero)
        throw new Exception("Could not load GLX.dll");

      string failedMethod;
      if (!LoadMethods(out failedMethod))
        throw new Exception($"Could not load method: {failedMethod}");
    }

    #endregion

    #region private methods

    private bool LoadMethods(out string errorMethod)
    {
      errorMethod = string.Empty;
      IntPtr functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXSetup");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXSetup";
        return false;
      }
      GLXSetupImpl = Marshal.GetDelegateForFunctionPointer<GLXSetupDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXMapHooks");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXMapHooks";
        return false;
      }
      GLXMapHooksImpl = Marshal.GetDelegateForFunctionPointer<GLXMapHooksDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXImagePointer");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXImagePointer";
        return false;
      }
      GLXImagePointerImpl = Marshal.GetDelegateForFunctionPointer<GLXImagePointerDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXDebugPointer");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXDebugPointer";
        return false;
      }
      GLXDebugPointerImpl = Marshal.GetDelegateForFunctionPointer<GLXDebugPointerDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXViewPort");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXViewPort";
        return false;
      }
      GLXViewPortImpl = Marshal.GetDelegateForFunctionPointer<GLXViewPortDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXTextures");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXTextures";
        return false;
      }
      GLXTexturesImpl = Marshal.GetDelegateForFunctionPointer<GLXTexturesDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXModels");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXModels";
        return false;
      }
      GLXModelsImpl = Marshal.GetDelegateForFunctionPointer<GLXModelsDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXFonts");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXFonts";
        return false;
      }
      GLXFontsImpl = Marshal.GetDelegateForFunctionPointer<GLXFontsDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXMatrices");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXMatrices";
        return false;
      }
      GLXMatricesImpl = Marshal.GetDelegateForFunctionPointer<GLXMatricesDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXMap");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXMap";
        return false;
      }
      GLXMapImpl = Marshal.GetDelegateForFunctionPointer<GLXMapDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXMapCoords");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXMapCoords";
        return false;
      }
      GLXMapCoordinatesImpl = Marshal.GetDelegateForFunctionPointer<GLXMapCoordinatesDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXDebug");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXDebug";
        return false;
      }
      GLXDebugImpl = Marshal.GetDelegateForFunctionPointer<GLXDebugDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXSetFontCapture");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXTGLXSetFontCaptureextures";
        return false;
      }
      GLXSetFontCaptureImpl = Marshal.GetDelegateForFunctionPointer<GLXSetFontCaptureDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXSetColourCapture");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXSetColourCapture";
        return false;
      }
      GLXSetColourCaptureImpl = Marshal.GetDelegateForFunctionPointer<GLXSetColourCaptureDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXSaveTextures");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXSaveTextures";
        return false;
      }
      GLXSaveTexturesImpl = Marshal.GetDelegateForFunctionPointer<GLXSaveTexturesDelegate>(functionPtr);

      functionPtr = Kernel32.GetProcAddress(modulePtr, "GLXSetTimeout");
      if (functionPtr == IntPtr.Zero)
      {
        errorMethod = "GLXSetTimeout";
        return false;
      }
      GLXSetTimeoutImpl = Marshal.GetDelegateForFunctionPointer<GLXSetTimeoutDelegate>(functionPtr);

      return true;
    }

    private Bitmap GetImage(IntPtr ptr, Rectangle area)
    {
      int width = area.Width;
      int height = area.Height;

      int length = ((width * 32 + 31) / 32) * 4 * height;
      byte[] bytes = new byte[length];
      Marshal.Copy(ptr, bytes, 0, length);

      Bitmap bmp = new Bitmap(width, height);
      BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
      Marshal.Copy(bytes, 0, data.Scan0, length);
      bmp.UnlockBits(data);
      return bmp;
    }

    private T[] StructArrayFromPointer<T>(IntPtr ptr, uint size) where T : struct
    {
      T[] result = new T[size];
      int structSize = Marshal.SizeOf(typeof(T));
      long longPtr = ptr.ToInt64();
      for (int i = 0; i < size; i++)
      {
        IntPtr structLocation = new IntPtr(longPtr + structSize * i);
        result[i] = Marshal.PtrToStructure<T>(structLocation);
      }
      return result;
    }

    #endregion

    #region public methods

    public bool Setup(int processId)
    {
      return GLXSetupImpl(processId);
    }

    public bool MapHooks(int processId)
    {
      return GLXMapHooksImpl(processId);
    }

    public Bitmap Image()
    {
      return GetImage(GLXImagePointerImpl(), Viewport());
    }

    public Bitmap DebugImage()
    {
      return GetImage(GLXDebugPointerImpl(), Viewport());
    }

    public Rectangle Viewport()
    {
      int x, y, width, height;
      GLXViewPortImpl(out x, out y, out width, out height);
      return new Rectangle(x, y, width, height);
    }

    public Rectangle Viewport(int x, int y, int width, int height)
    {
      GLXViewPortImpl(out x, out y, out width, out height);
      return new Rectangle(x, y, width, height);
    }

    public GLTexture[] Textures()
    {
      uint size;
      IntPtr texturesPtr = GLXTexturesImpl(out size);
      return StructArrayFromPointer<GLTexture>(texturesPtr, size);
    }

    public GLModel[] Models()
    {
      uint size;
      IntPtr modelsPtr = GLXModelsImpl(out size);
      return StructArrayFromPointer<GLModel>(modelsPtr, size);
    }

    public GLFont[] Fonts()
    {
      uint size;
      IntPtr fontsPtr = GLXFontsImpl(out size);
      return StructArrayFromPointer<GLFont>(fontsPtr, size);
    }

    public GLMatrices Matrices()
    {
      IntPtr matricesPtr = GLXMatricesImpl();
      return Marshal.PtrToStructure<GLMatrices>(matricesPtr);
    }

    public Bitmap Map(float[] x, float[] y)
    {
      // https://github.com/Brandon-T/SRL-GLX/blob/master/Minimap.simba#L227
      IntPtr mapPointer = GLXMapImpl(512, 512, x, y);
      return GetImage(mapPointer, new Rectangle(0, 0, 512, 512));
    }

    public RectangleF MapCoordinates()
    {
      float[] xArray;
      float[] yArray;
      GLXMapCoordinatesImpl(out xArray, out yArray);
      return new RectangleF(xArray[0], yArray[0], xArray[2] - xArray[0], yArray[2] - yArray[0]);
    }

    public bool Debug(DebugMode mode, uint textureId, uint colourId, uint fullColourId, int tolerance, int x, int y, int width,
      int height)
    {
      return GLXDebugImpl((int) mode, textureId, colourId, fullColourId, tolerance, x, y, width, height);
    }

    public bool SetFontCapture(bool enabled)
    {
      return GLXSetFontCaptureImpl(enabled);
    }

    public bool SetColourCapture(bool enabled)
    {
      return GLXSetColourCaptureImpl(enabled);
    }

    public bool SaveTexture()
    {
      return GLXSaveTexturesImpl();
    }

    public void SetTimeout(uint timeout)
    {
      GLXSetTimeoutImpl(timeout);
    }

    #endregion

    #region IDisposable impl

    public void Dispose()
    {
      Kernel32.FreeLibrary(modulePtr);
    }

    #endregion



  }
}
