using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Smart.Sharp
{
    public class SmartRemote : IDisposable
    {

        #region delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GetClientsDelegate(bool paired);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GetAvailablePIDDelegate(int pid);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool KillClientDelegate(int pid);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr SpawnClientDelegate(
          string javaPath, string remotePath, string root, string parameters,
          int width, int height, string initSeq, string useragent,
          string javaArgs, string plugins);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr PairClientDelegate(int pid);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GetClientPIDDelegate(IntPtr target);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FreeClientDelegate(IntPtr target);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetImageArrayDelegate(IntPtr target);

        #endregion

        #region fields

        private readonly IntPtr _hModule;
        private readonly string _path;

        #endregion

        #region delegates

        public GetClientsDelegate GetClients;
        public GetAvailablePIDDelegate GetAvailablePID;
        public KillClientDelegate KillClient;
        public SpawnClientDelegate SpawnClient;
        public PairClientDelegate PairClient;
        public GetClientPIDDelegate GetClientPID;
        public FreeClientDelegate FreeClient;

        #endregion

        #region constructor

        public SmartRemote(string path)
        {
            _path = path;
            string dllPath = Path.Combine(_path, "libsmartremote32.dll");
            if (!File.Exists(Path.Combine(dllPath)))
                throw new Exception("Could not locate libsmartremote32.dll");

            _hModule = Kernel32.LoadLibrary(dllPath);
            if (_hModule == IntPtr.Zero)
                throw new Exception("Could not load libsmartremote32.dll");

            LoadSmartMethods();
        }

        #endregion

        #region private methods

        private void LoadSmartMethods()
        {
            IntPtr functionAddress = IntPtr.Zero;

            functionAddress = Kernel32.GetProcAddress(_hModule, "exp_getClients");
            GetClients = (GetClientsDelegate)
              Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(GetClientsDelegate));

            functionAddress = Kernel32.GetProcAddress(_hModule, "exp_getAvailablePID");
            GetAvailablePID = (GetAvailablePIDDelegate)
              Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(GetAvailablePIDDelegate));

            functionAddress = Kernel32.GetProcAddress(_hModule, "exp_killClient");
            KillClient = (KillClientDelegate)
              Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(KillClientDelegate));

            functionAddress = Kernel32.GetProcAddress(_hModule, "exp_spawnClient");
            SpawnClient = (SpawnClientDelegate)
              Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(SpawnClientDelegate));

            functionAddress = Kernel32.GetProcAddress(_hModule, "exp_pairClient");
            PairClient = (PairClientDelegate)
              Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(PairClientDelegate));

            functionAddress = Kernel32.GetProcAddress(_hModule, "exp_getClientPID");
            GetClientPID = (GetClientPIDDelegate)
              Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(GetClientPIDDelegate));

            functionAddress = Kernel32.GetProcAddress(_hModule, "exp_freeClient");
            FreeClient = (FreeClientDelegate)
              Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(FreeClientDelegate));

        }

        #endregion

        #region public methods

        public void Dispose()
        {
            Kernel32.FreeLibrary(_hModule);
        }

        #endregion
    }
}