using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Smart.Sharp.Engine;
using Smart.Sharp.Engine.ScriptSystem;
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
      string smartRemotePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SmartSharp", "Smart");

      SmartRemote remote = new SmartRemote(smartRemotePath);

      SessionSettings settings = new SessionSettings();
      settings.SessionType = SessionType.OldSchool;
      settings.JavaPath = javaPath;
      settings.SmartPath = smartRemotePath;

      if (!File.Exists("testscript.js"))
      {
        Console.WriteLine("Can't find testscript.js press and key to continue...");
        Console.ReadKey();
        return;
      }

      Session session = new Session(remote, settings);
      session.SessionStarted += (sender, eventArgs) =>
      {
        Script script = new Script();
        script.Uri = ".\\testscript.js";
        script.Name = "script";

        session.ScriptController.Start(script);
      };

      session.Start();

      Console.WriteLine("Press any key to stop script...");
      Console.ReadKey();

      session.ScriptController.Stop();

      Console.WriteLine("Press any key to quit...");
      Console.ReadKey();
      
      session.Stop();

      //Console.WriteLine($"Session ended: {quitSafely}");
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
