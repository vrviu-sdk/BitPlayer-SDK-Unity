//-----------------------------------------------------
//            VRVIU: BitPlayer-SDK-Unity
//            Author: kevin.zha@vrviu.com  
// Copyright © 2016-2018 VRVIU Technologies Limited. 
//-----------------------------------------------------

using UnityEngine;

namespace VRVIU.BitVrPlayer.Video
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
            Debug.Log("OnReady");
        }

        /// <summary>
        ///  Triggered when first frame feed out from video player.
        /// </summary>
        void OnFirstFrameReady()
        {
            Debug.Log("OnFirstFrameReady");
        }

        void OnEnd()
        {
            Debug.Log("OnEnd");
        }

        /// <summary>
        /// Triggered when video resized in video player.
        /// </summary>
        void OnResize()
        {
            Debug.Log("OnResize");
        }

        /// <summary>
        /// Triggered when some errors happened in video player.
        /// </summary>
        void OnError(int errorCode, int errorCodeExtra)
        {
            Debug.Log("OnError");
        }
    }
}