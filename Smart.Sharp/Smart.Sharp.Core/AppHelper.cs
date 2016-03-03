using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Smart.Sharp.Core.Controller;
using Smart.Sharp.Core.Views;

namespace Smart.Sharp.Core
{
  internal static class AppHelper
  {

    internal static string SmartSharpFolderPath { get; private set; }

    internal static string SettingsFolderPath { get; private set; }

    internal static string ScreenshotFolderPath { get; private set; }

    internal static string SmartFolderPath { get; private set; }
    

    internal static void PreStartUp()
    {
      // Check SmartSharp directories exist
      CheckAndMakeDirectory((SmartSharpFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".SmartSharp")));

      CheckAndMakeDirectory((SettingsFolderPath = Path.Combine(SmartSharpFolderPath, "Settings")));

      CheckAndMakeDirectory((ScreenshotFolderPath = Path.Combine(SmartSharpFolderPath, "Screenshots")));

      CheckAndMakeDirectory((SmartFolderPath = Path.Combine(SmartSharpFolderPath, "Smart")));

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
