using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.AudioPlayer.Sdl2
{
    public class SdlDevice : IDisposable
    {
        IntPtr _pointer = IntPtr.Zero;
        internal IntPtr Pointer { get { return _pointer; } }
        public SdlDevice(IntPtr pFrame)
        {
            _pointer = NativeWrapper.SdlDevice_Alloc(pFrame);
            if (_pointer == IntPtr.Zero)
                throw new ApplicationException($"Create and load {nameof(SdlDevice)} failed (last error : {NativeWrapper.GetLastError()})");
        }
        ~SdlDevice()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        void Dispose(bool disposing)
        {
            if (_pointer != IntPtr.Zero)
                NativeWrapper.SdlDevice_Free(ref _pointer);
        }

        public SdlSourceQueueResult QueueAudio(IntPtr pFrame)
        {
            return NativeWrapper.SdlDevice_QueueAudio(_pointer, pFrame);
        }
    }
}
