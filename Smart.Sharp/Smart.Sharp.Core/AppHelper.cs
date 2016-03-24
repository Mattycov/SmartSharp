using System;
using System.IO;
using Smart.Sharp.Core.Controller;
using Smart.Sharp.Core.Views;

namespace Smart.Sharp.Core
{
  internal static class AppHelper
  {
    
    internal static void PreStartUp()
    {
      // Check SmartSharp directories exist
      CheckAndMakeDirectory((Properties.Settings.Default.SmartSharpFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SmartSharp")));

      CheckAndMakeDirectory((Properties.Settings.Default.ScreenshotsFolder = Path.Combine(Properties.Settings.Default.SmartSharpFolder, "Screenshots")));
      
      CheckAndMakeDirectory((Properties.Settings.Default.ScriptsFolder = Path.Combine(Properties.Settings.Default.SmartSharpFolder, "Scripts")));

      CheckAndMakeDirectory((Properties.Settings.Default.SmartFolder = Path.Combine(Properties.Settings.Default.SmartSharpFolder, "Smart")));

      CheckAndMakeDirectory((Properties.Settings.Default.PluginFolder = Path.Combine(Properties.Settings.Default.SmartSharpFolder, "Plugin")));

      Properties.Settings.Default.Save();

    }

    internal static void PostStartUp()
    {
      IAppController controller = new AppController();

      AppWindow appWindow = new AppWindow(controller);
      appWindow.Show();
    }

    internal static void Exit()
    {
      
    }

    private static void CheckAndMakeDirectory(string directory)
    {
      if (!Directory.Exists(directory))
        Directory.CreateDirectory(directory);
    }

  }
}
