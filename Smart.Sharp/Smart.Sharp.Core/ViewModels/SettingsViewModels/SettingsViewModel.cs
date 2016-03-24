using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Smart.Sharp.Core.Controller;
using Smart.Sharp.Core.Helpers;
using MessageBox = System.Windows.MessageBox;

namespace Smart.Sharp.Core.ViewModels.SettingsViewModels
{
  internal class SettingsViewModel : ViewModel
  {

    #region variables

    private string javaPath;
    private string smartPath;

    #endregion

    #region properties

    public string JavaPath
    {
      get { return javaPath; }
      set
      {
        if (javaPath == value)
          return;
        javaPath = value;
        OnPropertyChanged(nameof(JavaPath));
      }
    }

    public string SmartPath
    {
      get { return smartPath; }
      set
      {
        if (smartPath == value)
          return;
        smartPath = value;
        OnPropertyChanged(nameof(SmartPath));
      }
    }

    #endregion

    #region commands

    public ICommand SaveSettingsCommand { get; private set; }
    public ICommand SetJavaPathCommand { get; private set; }
    public ICommand BrowseJavaPathCommand { get; private set; }
    public ICommand BrowseSmartPathCommand { get; private set; }

    #endregion

    #region constructor

    public SettingsViewModel(IAppController controller) : base(controller)
    {
      SmartPath = Properties.Settings.Default.SmartFolder;
      JavaPath = Properties.Settings.Default.JavaPath;
      SaveSettingsCommand = new DelegateCommand(SaveSettingsCommandImpl);
      SetJavaPathCommand = new DelegateCommand(SetJavaPathCommandImpl);
      BrowseJavaPathCommand = new DelegateCommand(BrowseJavaPathCommandImpl);
      BrowseSmartPathCommand = new DelegateCommand(BrowseSmartPathCommandImpl);
    }

    #endregion

    #region private methods

    private void SaveSettingsCommandImpl()
    {
      Properties.Settings.Default.JavaPath = JavaPath;
      Properties.Settings.Default.SmartFolder = SmartPath;
      Properties.Settings.Default.Save();
    }

    private void SetJavaPathFromEnvironmentVariables(bool suppress)
    {
      string path = Environment.GetEnvironmentVariable("JAVA_HOME");
      if (string.IsNullOrEmpty(path) && !suppress)
      {
        MessageBox.Show("Could not find JAVA_HOME Environment Variable", "Warning!", MessageBoxButton.OK);
        return;
      }
      path = Path.Combine(path, "bin");
      if (!File.Exists(path))
      {
        MessageBox.Show($"Could not find {path}", "Warning!", MessageBoxButton.OK);
        return;
      }
      JavaPath = path;
    }

    private void SetJavaPathCommandImpl()
    {
      SetJavaPathFromEnvironmentVariables(false);
    }

    private void BrowseJavaPathCommandImpl()
    {
      FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
      if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
        return;
      JavaPath = folderBrowserDialog.SelectedPath;
    }

    private void BrowseSmartPathCommandImpl()
    {
      FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
      if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
        return;
      SmartPath = folderBrowserDialog.SelectedPath;
    }

    #endregion

    public override void Dispose()
    {
      
    }
  }
}
