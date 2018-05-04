﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRVIU.BitVrPlayer.Video
{
    public enum MEDIAPLAYER_STATE
    {
        NOT_READY = 0,
        INITIALIZED = 1,
        PREPARING = 2,
        READY = 3,
        PLAYING = 4,
        PAUSED = 5,
        STOPPED = 6,
        ERROR = 7,
        END = 8
    }
}
