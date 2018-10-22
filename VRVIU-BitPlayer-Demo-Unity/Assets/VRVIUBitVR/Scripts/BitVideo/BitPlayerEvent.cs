//-----------------------------------------------------
//            VRVIU: BitPlayer-SDK-Unity
//            Author: kevin.zha@vrviu.com  
// Copyright © 2016-2018 VRVIU Technologies Limited. 
//-----------------------------------------------------

using Assets.VRVIUBitVR.Scripts.Log;
using UnityEngine;

namespace VRVIU.BitVRPlayer.BitVideo
{
    /// <summary>
    /// This class as video player statemachine listener that interested events as follows:
    /// OnReady | OnVideoFristFrame | OnError | OnEnd | OnResize 
    /// </summary>
    public class BitPlayerEvent : MonoBehaviour
    {
        /// <summary>
        /// VideoPlayer instance.
        /// </summary>
        public BitPlayerTexture m_srcVideo;

        // Use this for initialization
        void Start()
        {
            m_srcVideo.OnReady += OnReady;
            m_srcVideo.OnVideoFirstFrameReady += OnFirstFrameReady;
            m_srcVideo.OnVideoError += OnError;
            m_srcVideo.OnEnd += OnEnd;
            m_srcVideo.OnResize += OnResize;
        }

        /// <summary>
        /// Triggered when video player state transition to ready.
        /// </summary>
        void OnReady()
        {
            VLog.log(VLog.LEVEL_INFO, "OnReady");
        }

        /// <summary>
        ///  Triggered when first frame feed out from video player.
        /// </summary>
        void OnFirstFrameReady()
        {
            VLog.log(VLog.LEVEL_INFO, "OnFirstFrameReady");
        }

        void OnEnd()
        {
            VLog.log(VLog.LEVEL_INFO, "OnEnd");
        }

        /// <summary>
        /// Triggered when video resized in video player.
        /// </summary>
        void OnResize()
        {
            VLog.log(VLog.LEVEL_INFO, "OnResize");
        }

        /// <summary>
        /// Triggered when some errors happened in video player.
        /// </summary>
        void OnError(int errorCode, int errorCodeExtra)
        {
            VLog.log(VLog.LEVEL_INFO, "OnError");
        }
    }
}