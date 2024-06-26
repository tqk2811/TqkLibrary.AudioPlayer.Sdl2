﻿using System;
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
