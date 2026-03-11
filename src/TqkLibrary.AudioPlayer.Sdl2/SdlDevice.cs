using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using TqkLibrary.AudioPlayer.Sdl2.Enums;

namespace TqkLibrary.AudioPlayer.Sdl2
{
    public class SdlDevice : IDisposable
    {
        IntPtr _pointer = IntPtr.Zero;
        internal IntPtr Pointer { get { return _pointer; } }
        public SdlDevice(int freq, byte channels, SdlAudioFormat format, string? deviceName = null)
        {
            byte[]? deviceNameBytes = null;
            if (!string.IsNullOrEmpty(deviceName))
            {
                deviceNameBytes = Encoding.UTF8.GetBytes(deviceName + "\0");
            }
            _pointer = NativeWrapper.SdlDevice_Alloc(deviceNameBytes, freq, channels, (ushort)format);
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

        public SdlSourceQueueResult QueueAudio(byte[] data, float volume = 1.0f)
        {
            return NativeWrapper.SdlDevice_QueueAudio(_pointer, data, (uint)data.Length, volume);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag">0 for remuse</param>
        public void Pause(int flag = 0)
        {
            NativeWrapper.SdlDevice_Pause(_pointer, flag);
        }

        public AudioStatus GetStatus()
        {
            return NativeWrapper.SdlDevice_GetStatus(_pointer);
        }

        public uint GetQueuedAudioSize()
        {
            return NativeWrapper.SdlDevice_GetQueuedAudioSize(_pointer);
        }

        public void ClearQueuedAudio()
        {
            NativeWrapper.SdlDevice_ClearQueuedAudio(_pointer);
        }

        public static IEnumerable<string> GetAudioDeviceNames()
        {
            int count = NativeWrapper.SdlDevice_GetNumAudioDevices();
            for (int i = 0; i < count; i++)
            {
                IntPtr ptr = NativeWrapper.SdlDevice_GetAudioDeviceName(i);
                if (ptr != IntPtr.Zero)
                {
                    List<byte> bytes = new List<byte>();
                    int offset = 0;
                    byte b;
                    while ((b = Marshal.ReadByte(ptr, offset++)) != 0)
                    {
                        bytes.Add(b);
                    }
                    yield return Encoding.UTF8.GetString(bytes.ToArray());
                }
            }
        }


        public async Task WaitUntilPlayCompleteAsync(int interval = 100, CancellationToken cancellationToken = default)
        {
            while (GetStatus() == AudioStatus.Playing && GetQueuedAudioSize() > 0)
            {
                await Task.Delay(interval, cancellationToken);
            }
        }

        public async Task<bool> WaitUntilPlayCompleteAsync(TimeSpan waitTime, int interval = 100, CancellationToken cancellationToken = default)
        {
            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource((int)waitTime.TotalMilliseconds);
            while (GetStatus() == AudioStatus.Playing && GetQueuedAudioSize() > 0)
            {
                await Task.Delay(interval, cancellationToken);
                if (cancellationTokenSource.IsCancellationRequested)
                    return false;
            }
            return true;
        }
    }
}
