using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRVIU.BitVRPlayer.BitVideo
{
    public class VideoCallback
    {
        public delegate void VideoEnd();
        public delegate void VideoReady();
        public delegate void VideoError(int errorCode, int errorCodeExtra);
        public delegate void VideoFirstFrameReady();
        public delegate void VideoResize();
    }
}
