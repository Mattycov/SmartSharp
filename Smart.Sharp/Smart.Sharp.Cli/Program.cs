using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Smart.Sharp.Engine;
using Smart.Sharp.Engine.Api;
using Smart.Sharp.Engine.Script;
using Smart.Sharp.Native;
using Smart.Sharp.Native.OpenGL;

namespace Smart.Sharp.Cli
{
  internal class Program
  {
    private static void Main(string[] args)
    {

      /*GLX glx = new GLX(@"C:\Users\mcollinge\SmartSharp\Smart\GLX.dll");
      Process[] processes = Process.GetProcessesByName("HotlineGL");
      Process dodProcess = processes.FirstOrDefault();
      if (dodProcess == null)
      {
        Console.WriteLine("Could not find process");
        Console.WriteLine("Press any key to quite...");
        Console.ReadKey();
        return;
      }

      glx.Setup(dodProcess.Id);
      glx.MapHooks(dodProcess.Id);

      Rectangle view = glx.Viewport();
      Console.WriteLine(glx.Textures().Length);
      Console.ReadKey();*/




      string javaPath = Environment.GetEnvironmentVariable("JAVA_HOME");
      if (string.IsNullOrEmpty(javaPath))
      {
        Console.WriteLine("Could not find JAVA_HOME in environment variables");
        Console.WriteLine("Press and key to continue...");
        Console.ReadKey();
        return;
      }
      javaPath = Path.Combine(javaPath, "bin");
      string smartRemotePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SmartSharp", "Smart");
      string modulePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SmartSharp", "Modules");
      string tesseractPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SmartSharp", "tessdata");

      Native.SmartRemote remote = new Native.SmartRemote(smartRemotePath);
      GLX glx = new GLX(@"C:\Users\mcollinge\SmartSharp\Smart\GLX.dll");

      string[] fonts = Directory.GetDirectories(@"C:\Users\mcollinge\SmartSharp\Fonts");

      SessionSettings settings = new SessionSettings();
      settings.SessionType = SessionType.RS3;
      settings.JavaPath = javaPath;
      settings.SmartPath = smartRemotePath;
      settings.ModulePaths = new []{modulePath};
      settings.TesseractPath = tesseractPath;
      settings.FontPaths = fonts;
      settings.ShowConsole = true;
      settings.Plugins = "OpenGL32.dll";

      if (!File.Exists("testscript.lua"))
      {
        Console.WriteLine("Can't find testscript.lua press and key to continue...");
        Console.ReadKey();
        return;
      }

      Session session = new Session(remote, settings);
      session.SessionStarted += (sender, eventArgs) =>
      {
        string scriptFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SmartSharp", "Scripts");
        Console.WriteLine("Select a script");
        string[] scripts = Directory.GetFiles(scriptFolder);
        for (int i = 0; i < scripts.Length; i++)
        {
          Console.WriteLine($"{i}: {Path.GetFileNameWithoutExtension(scripts[i])}");
        }
        string line = Console.ReadLine();
        int scriptNumber;
        int.TryParse(line, out scriptNumber);
        
        Script script = new Script();
        script.Uri = scripts[scriptNumber];
        script.Name = Path.GetFileNameWithoutExtension(scripts[scriptNumber]);

        session.ScriptController.Start(script);
      };

      session.Start();

      Console.WriteLine("Press any key to stop script...");
      Console.ReadKey();

      session.ScriptController.Stop();

      Console.WriteLine("Press any key to quit...");
      Console.ReadKey();

      glx.Image().Save("test.png");
      
      session.Stop();

      //Console.WriteLine($"Session ended: {quitSafely}");
      Console.WriteLine("Press any key to continue...");
      Console.ReadKey();

    }

  }
}
