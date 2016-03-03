using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Smart.Sharp.Core
{
  public class Boot
  {
    [STAThread]
    public static void Main(string[] args)
    {
      App app = new App();
      app.InitializeComponent();
      app.Run();
    }

  }
}
