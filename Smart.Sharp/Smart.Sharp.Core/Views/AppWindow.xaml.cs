using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Smart.Sharp.Core.Controller;
using Smart.Sharp.Core.ViewModels;

namespace Smart.Sharp.Core.Views
{
  /// <summary>
  /// Interaction logic for AppWindow.xaml
  /// </summary>
  internal partial class AppWindow : Window
  {
    
    internal AppWindow(IAppController controller)
    {
      DataContext = new AppViewModel(controller);
      InitializeComponent();
    }
  }
}
