﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.AudioPlayer.Sdl2
{
    public enum AudioStatus : int
    {
        Failed = -1,
        Stopped = 0,
        Playing = 1,
        Paused = 2,
    }
}
