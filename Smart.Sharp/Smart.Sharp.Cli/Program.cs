using System;
using System.IO;

namespace Smart.Sharp.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            string smartRemotePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "smart");

            SmartRemote smartRemote = new SmartRemote(smartRemotePath);

            IntPtr smart = smartRemote.SpawnClient(@"C:\Program Files (x86)\Java\jre1.8.0_65\bin\java.exe", smartRemotePath, "http://world37.runescape.com/", "", 800, 600, null, null, null, null);

            Console.WriteLine("Press any key to kill SMART");
            Console.ReadKey();

            bool ret = smartRemote.KillClient(smartRemote.GetClientPID(smart));

            Console.WriteLine("SNART shutdown: " + ret);
            Console.ReadKey();
        }
    }
}
