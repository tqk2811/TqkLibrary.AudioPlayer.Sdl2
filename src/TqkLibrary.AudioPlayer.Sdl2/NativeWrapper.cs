using System.Reflection;
using System.Runtime.InteropServices;

namespace TqkLibrary.AudioPlayer.Sdl2
{
    internal static class NativeWrapper
    {
#if DEBUG
#if NETFRAMEWORK
        static NativeWrapper()
        {
            string path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
                "runtimes",
                Environment.Is64BitProcess ? "win-x64" : "win-x86",
                "native"
                );
            bool r = SetDllDirectory(path);
            if (!r)
                throw new InvalidOperationException("Can't set Kernel32.SetDllDirectory");
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetDllDirectory(string PathName);
#endif
#endif
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetLastError();


        [DllImport("TqkLibrary.AudioPlayer.Sdl2.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SdlDevice_Alloc(IntPtr pFrame);


        [DllImport("TqkLibrary.AudioPlayer.Sdl2.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SdlDevice_Free(ref IntPtr ppSdlDevice);


        [DllImport("TqkLibrary.AudioPlayer.Sdl2.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SdlSourceQueueResult SdlDevice_QueueAudio(IntPtr pSdlDevice, IntPtr pFrame);


        [DllImport("TqkLibrary.AudioPlayer.Sdl2.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SdlSourceQueueResult SdlDevice_Pause(IntPtr pSdlDevice, int flag);


        [DllImport("TqkLibrary.AudioPlayer.Sdl2.Native.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern AudioStatus SdlDevice_GetStatus(IntPtr pSdlDevice);

    }
}