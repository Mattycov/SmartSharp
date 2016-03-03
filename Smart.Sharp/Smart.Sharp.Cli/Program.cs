using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Smart.Sharp.Native;

namespace Smart.Sharp.Cli
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      string javaPath = Environment.GetEnvironmentVariable("JAVA_HOME");
      if (string.IsNullOrEmpty(javaPath))
      {
        Console.WriteLine("Could not find JAVA_HOME in environment variables");
        Console.WriteLine("Press and key to continue...");
        Console.ReadKey();
        return;
      }
      javaPath = Path.Combine(javaPath, "bin", "java.exe");
      string smartRemotePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "smart");

      SmartRemote smartRemote = new SmartRemote(smartRemotePath);

      //IntPtr smart = smartRemote.SpawnClient(javaPath, smartRemotePath, "http://world37.runescape.com/", "", 800, 600, null, null, null, null);
      IntPtr smart = smartRemote.SpawnClient(javaPath, smartRemotePath,
        "http://oldschool81.runescape.com/", "", 800, 600, null, null, null, null);

      bool loop = true;
      while (loop)
      {

        Console.WriteLine("Press any key to save client image, or Space to quit...");
        ConsoleKeyInfo info = Console.ReadKey();
        if (info.Key == ConsoleKey.Spacebar)
        {
          loop = false;
          continue;
        }

        IntPtr bitmapPtr = smartRemote.GetImageArray(smart);
        if (bitmapPtr == IntPtr.Zero)
        {
          Console.WriteLine("Could not grad client image, exiting!");
          loop = false;
          continue;
        }

        SaveIntPtrToBitmap(bitmapPtr);
      }

      bool ret = smartRemote.KillClient(smartRemote.GetClientPID(smart));

      Console.WriteLine("SMART shutdown: " + ret);
      Console.WriteLine("Press any key to continue...");
      Console.ReadKey();
    }

    private static void SaveIntPtrToBitmap(IntPtr ptr)
    {
      int width = 800;
      int height = 600;

      int length = ((width * 32 + 31) / 32) * 4 * height;
      byte[] bytes = new byte[length];
      Marshal.Copy(ptr, bytes, 0, length);

      Bitmap bmp = new Bitmap(width, height);

      int i = 0;
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          int blue = bytes[i++];
          int green = bytes[i++];
          int red = bytes[i++];
          i++;
          bmp.SetPixel(x, y, Color.FromArgb(red, green, blue));
        }
      }

      bmp.Save($"smart-{DateTime.Now.ToFileTime()}.png");
    }

  }
}
