using VRVIU.BitVRPlayer.BitVideo.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRVIU.BitVRPlayer.BitData;
using VRVIU.BitVRPlayer.BitVRPlayer.BitData;

namespace VRVIU.BitVRPlayer.BitVideo
{
    public class BitVideoPlayer : MonoBehaviour
    {
        private BitPlayerTexture mediaPlayer;
        private SilverMediaPlayerCtrl mSilverPlayer = null;
        public VideoFormater mVideoFormater;
        private VideoData mData;
        public VideoCallback.VideoReady OnReady;
        public VideoCallback.VideoEnd OnEnd;
        public VideoCallback.VideoError OnVideoError;
        public VideoCallback.VideoResize OnResize;
        public VideoCallback.VideoFirstFrameReady OnVideoFirstFrameReady;
        public GameObject[] mTargetMaterial = null;
        public Shader mShaderYUV;
        public Account mAccount;
        private void Awake()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
        }
        void Start()
        {
			//mVideoFormater = new VideoFormater();
        }

        public void SetupPlayer(VideoData data, Account account)
        {
            mData = data;
            
            switch ((Algorithm)data.algorithmType)
            {
                case Algorithm.ROI:
                case Algorithm.FE:
                case Algorithm.ROIMapping:
                case Algorithm.P4:
                case Algorithm.MVM:
                    mSilverPlayer = gameObject.AddComponent<SilverMediaPlayerCtrl>();
                    mSilverPlayer.mShaderYUV = mShaderYUV;
                    mSilverPlayer.m_ViewerFormatScript = gameObject.AddComponent<SetViewerFormat>();
                    mSilverPlayer.m_TargetMaterial = mTargetMaterial;
                    mSilverPlayer.OnReady += OnReady;
                    mSilverPlayer.OnVideoFirstFrameReady += OnVideoFirstFrameReady;
                    mSilverPlayer.OnVideoFirstFrameReady += onFrameReady;
                    if (!string.IsNullOrEmpty(data.url))
                    {
                        mSilverPlayer.setUrl(data.url);
                    }
                    else
                    {
                        mSilverPlayer.setVideoInfo(data.videoInfo);
                    }
                    if (data.format != VideoFormat.OPT_UNKONW)
                    {
                        SwitchVideoFormat();
                    }
                    
                    mediaPlayer = null;
                    break;
                case Algorithm.ERP:
                case Algorithm.NULL:
                    mSilverPlayer = null;
                    mediaPlayer = gameObject.AddComponent<BitPlayerTexture>();
                    mediaPlayer.m_TargetMaterial = mTargetMaterial;
                    mediaPlayer.mShaderYUV = mShaderYUV;
                    mediaPlayer.init(account);
                    mediaPlayer.setUrl(data.url);
                    if(mVideoFormater != null && data != null && !string.IsNullOrEmpty(data.meshUrl))
                    {
                        mVideoFormater.SetMeshUrl(data.meshUrl);
                    }
                    mediaPlayer.OnVideoFirstFrameReady += OnVideoFirstFrameReady;
                    mediaPlayer.OnVideoFirstFrameReady += onFrameReady;
                    if (data.format != VideoFormat.OPT_UNKONW)
                    {
                        mVideoFormater.Switch(data.format);
                    }
                    break;
                default:
                    Debug.LogError("Unknown algorithm " + data.algorithmType);
                    mSilverPlayer = gameObject.AddComponent<SilverMediaPlayerCtrl>();
                    mSilverPlayer.mShaderYUV = mShaderYUV;
                    mSilverPlayer.m_ViewerFormatScript = gameObject.AddComponent<SetViewerFormat>();
                    mSilverPlayer.m_TargetMaterial = mTargetMaterial;
                    mSilverPlayer.OnReady += OnReady;
                    mSilverPlayer.OnVideoFirstFrameReady += OnVideoFirstFrameReady;
                    mSilverPlayer.OnVideoFirstFrameReady += onFrameReady;
                    if (!string.IsNullOrEmpty(data.url))
                    {
                        mSilverPlayer.setUrl(data.url);
                    }
                    else
                    {
                        mSilverPlayer.setVideoInfo(data.videoInfo);
                    }
                    if (data.format != VideoFormat.OPT_UNKONW)
                    {
                        SwitchVideoFormat();
                    }
                    mediaPlayer = null;
                    break;
            }
        }

        private VideoFormat ConvertFormat(VideoPorjection projection, VideoSteroType stereoType, VideoHfov hfov)
        {
            VideoFormat videoFormat = VideoFormat.OPT_ERP_360_MONO;
            if (projection ==  VideoPorjection.OPT_ERP)
            {
                switch (stereoType)
                {
                    case VideoSteroType.OPT_MONO:
                        if (hfov == VideoHfov.DEGREE_180)
                        {
                            videoFormat = VideoFormat.OPT_ERP_180_MONO;
                        }
                        else if (hfov == VideoHfov.DEGREE_360)
                        {
                            videoFormat = VideoFormat.OPT_ERP_360_MONO;
                        }
                        else
                        {
                            videoFormat = VideoFormat.OPT_ERP_360_MONO;
                        }
                        break;
                    case VideoSteroType.OPT_STERO_LR:
                        if (hfov == VideoHfov.DEGREE_180)
                        {
                            videoFormat = VideoFormat.OPT_ERP_180_LR;
                        }
                        else if (hfov ==  VideoHfov.DEGREE_360)
                        {
                            videoFormat = VideoFormat.OPT_ERP_360_LR;
                        }
                        else
                        {
                            videoFormat = VideoFormat.OPT_ERP_360_LR;
                        }
                        break;
                    case VideoSteroType.OPT_STERO_TB:
                        if (hfov ==  VideoHfov.DEGREE_180)
                        {
                            videoFormat = VideoFormat.OPT_ERP_180_TB;
                        }
                        else if (hfov == VideoHfov.DEGREE_360)
                        {
                            videoFormat = VideoFormat.OPT_ERP_360_TB;
                        }
                        else
                        {
                            videoFormat = VideoFormat.OPT_ERP_360_TB;
                        }
                        break;
                    case VideoSteroType.OPT_STERO_RL:
                        break;
                    case VideoSteroType.OPT_STERO_BT:
                        break;
                    default:
                        break;
                }
            }
            else if (projection == VideoPorjection.OPT_FISHEYE)
            {
                switch (stereoType)
                {
                    case  VideoSteroType.OPT_MONO:
                        videoFormat = VideoFormat.OPT_FISHEYE_MONO;
                        break;
                    case VideoSteroType.OPT_STERO_LR:
                        videoFormat = VideoFormat.OPT_FISHEYE_LR;
                        break;
                    case VideoSteroType.OPT_STERO_TB:
                        videoFormat = VideoFormat.OPT_FISHEYE_TB;
                        break;
                    case VideoSteroType.OPT_STERO_RL:
                        break;
                    case VideoSteroType.OPT_STERO_BT:
                        break;
                    default:
                        videoFormat = VideoFormat.OPT_FISHEYE_MONO;
                        break;
                }
            }
            else if (projection == VideoPorjection.OPT_TROPIZOED)
            {
                switch (stereoType)
                {
                    case VideoSteroType.OPT_MONO:
                        videoFormat = VideoFormat.OPT_TROPIZED_MONO;
                        break;
                    case VideoSteroType.OPT_STERO_LR:
                        videoFormat = VideoFormat.OPT_TROPIZED_LR;
                        break;
                    case VideoSteroType.OPT_STERO_TB:
                        videoFormat = VideoFormat.OPT_TROPIZED_TB;
                        break;
                    case VideoSteroType.OPT_STERO_RL:
                        break;
                    case VideoSteroType.OPT_STERO_BT:
                        break;
                    default:
                        videoFormat = VideoFormat.OPT_TROPIZED_MONO;
                        break;
                }
            }
            else if (projection == VideoPorjection.OPT_FLAT)
            {
                switch (stereoType)
                {
                    case VideoSteroType.OPT_MONO:
                        videoFormat = VideoFormat.OPT_FLAT_MONO;
                        break;
                    case VideoSteroType.OPT_STERO_LR:
                        videoFormat = VideoFormat.OPT_FLAT_LR;
                        break;
                    case VideoSteroType.OPT_STERO_TB:
                        videoFormat = VideoFormat.OPT_FLAT_TB;
                        break;
                    case VideoSteroType.OPT_STERO_RL:
                        break;
                    case VideoSteroType.OPT_STERO_BT:
                        break;
                    default:
                        videoFormat = VideoFormat.OPT_FLAT_MONO;
                        break;
                }
            }
            return videoFormat;
        }

        public void SetVideoFormat(VideoPorjection projection, VideoSteroType steroType, VideoHfov hfov) {
            VideoFormat format = ConvertFormat(projection, steroType,hfov);
            Debug.Log("set video format "+format +" projection "+ projection +" steroType "+steroType + " hfov "+hfov);
            if (mediaPlayer) {
                mVideoFormater.Switch(format);
            }
        }

        public void AutoVideoFormat() {
            if (mediaPlayer)
            {
                //mediaPlayer.SetVideoFormat(format);
            }
        }

        public void Play() {
            if (mSilverPlayer) {
                mSilverPlayer.Play();
            }
            else if (mediaPlayer)
            {
                mediaPlayer.Play();
            }
        }

        public void Pause() {
            if (mSilverPlayer)
            {
                mSilverPlayer.Pause();
            }
            else
            {
                mediaPlayer.Pause();
            }
        }

        public void SetReplay(int count) {
            if (mSilverPlayer)
            {
                mSilverPlayer.SetRePlay(count);
            }
            else if (mediaPlayer)
            {
                //mediaPlayer.SetRePlay(count);
            }
        }

        public void Release(){
            if (mSilverPlayer)
            {
                mSilverPlayer.Stop();
                mSilverPlayer.UnLoad();
            }
            else if (mediaPlayer)
            {
                mediaPlayer.Stop();
                mediaPlayer.UnLoad();
            }
        }

        public int GetVideoWidth() {
            int value = 0;
            if (mSilverPlayer)
            {
                //value = mSilverPlayer.GetVideoWidth();
            }
            else if (mediaPlayer)
            {
                value = mediaPlayer.GetVideoWidth();
            }
            return value;
        }

        public int GetVideoHeight() {
            int value = 0;
            if (mSilverPlayer)
            {
                //value = mSilverPlayer.GetVideoHeight();
            }
            else if (mediaPlayer)
            {
                value = mediaPlayer.GetVideoHeight();
            }
            return value;
        }

        public float GetSeekBarValue(){
            float value = 0;
            if (mSilverPlayer){
                value = mSilverPlayer.GetSeekBarValue();
            }
            else if (mediaPlayer){
                value = mediaPlayer.GetSeekBarValue();
            }
            return value;
        }


        public void SetSeekBarValue(float fValue) {
            if (mSilverPlayer)
            {
                mSilverPlayer.SetSeekBarValue(fValue);
            }
            else if (mediaPlayer)
            {
                mediaPlayer.SetSeekBarValue(fValue);
            }
        }

        public void SeekTo(int iSeek) {
            if (mSilverPlayer)
            {
                mSilverPlayer.SeekTo(iSeek);
            }
            else if (mediaPlayer)
            {
                mediaPlayer.SeekTo(iSeek);
            }
        }

        public void SetVolume(float fVolume) {
            if (mSilverPlayer)
            {
                mSilverPlayer.SetVolume(fVolume);
            }
            else if (mediaPlayer)
            {
                mediaPlayer.SetVolume(fVolume);
            }
        }

        public void SetSpeed(float fSpeed)
        {
            if (mSilverPlayer)
            {
                mSilverPlayer.SetSpeed(fSpeed);
            }
            else if (mediaPlayer)
            {
                mediaPlayer.SetSpeed(fSpeed);
            }
        }

        public float GetSpeed()
        {
            float speed = 1.0f;
            if (mSilverPlayer)
            {
               //speed =  mSilverPlayer.GetSpeed();
            }
            else
            {
                //speed = mediaPlayer.GetSpeed();
            }
            return speed;
        }

        public MEDIAPLAYER_STATE GetCurrentState()
        {
            MEDIAPLAYER_STATE currentState = MEDIAPLAYER_STATE.NOT_READY;
            if (mSilverPlayer)
            {
               currentState = mSilverPlayer.GetCurrentState();
            }
            else if(mediaPlayer)
            {
                currentState = mediaPlayer.GetCurrentState();
            }
            return currentState;
        }

        public void Reload() {
            switch ((Algorithm)mData.algorithmType)
            {
                case Algorithm.ROI:
                case Algorithm.FE:
                case Algorithm.ROIMapping:
                    mSilverPlayer = new SilverMediaPlayerCtrl();
                    break;
                default:
                    mSilverPlayer = null;
                    break;
            }

            if (mSilverPlayer)
            {
                mSilverPlayer.setUrl(mData.url);
            }
            else if (mediaPlayer)
            {
                mediaPlayer.setUrl(mData.url);
            }
        }

      /*  public void Load() {
            if (mSilverPlayer)
            {
                mSilverPlayer.setUrl(mData.url);
            }
            else
            {
                mediaPlayer.setUrl(mData.url);
            }
        }
        */
        public void Stop() {
            if (mSilverPlayer)
            {
                mSilverPlayer.Stop();
            }
            else if (mediaPlayer)
            {
                mediaPlayer.Stop();
            }
        }

        public int GetDuration() {
            int duration = 0;
            if (mSilverPlayer){
               duration = (int)(mSilverPlayer.GetDuration() / 1000);
            }
			else if(mediaPlayer){
               duration = mediaPlayer.GetDuration(); 
            }
            return duration;
        }

        public int GetPlayPosition() {
            int playPosition = 0;
            if (mSilverPlayer)
            {
               playPosition = (int)(mSilverPlayer.GetSeekPosition() / 1000);
                
            }
            else
            {
                if (mediaPlayer != null && mediaPlayer.GetSeekPosition() > 0)
                {
                    playPosition = mediaPlayer.GetSeekPosition();
                }
            }
            return playPosition;
        }

        private void SwitchVideoFormat()
        {
            // Silver-player will set up it's own mesh, so no need to specify mesh type.
            // only need to know if it's mono/stereo.
            if (mSilverPlayer)
            {
                mSilverPlayer.m_ViewerFormatScript = gameObject.AddComponent<SetViewerFormat>();
                switch ((VideoFormat)mData.format)
                {
                    case VideoFormat.OPT_ERP_180_MONO:
                    case VideoFormat.OPT_ERP_360_MONO:
                    case VideoFormat.OPT_FISHEYE_MONO:
                    case VideoFormat.OPT_FLAT_MONO:
                        SetViewerFormat.m_formatType = "mono";
                        break;
                    case VideoFormat.OPT_ERP_180_TB:
                    case VideoFormat.OPT_ERP_360_TB:
                    case VideoFormat.OPT_FISHEYE_TB:
                    case VideoFormat.OPT_FLAT_TB:
                        SetViewerFormat.m_formatType = "stereo_top_bottom";
                        break;
                    case VideoFormat.OPT_ERP_180_LR:
                    case VideoFormat.OPT_ERP_360_LR:
                    case VideoFormat.OPT_FISHEYE_LR:
                    case VideoFormat.OPT_FLAT_LR:
                        SetViewerFormat.m_formatType = "stereo_left_right";
                        break;
                    default:
                        Debug.LogError("Unknown format " + mData.format);
                        SetViewerFormat.m_formatType = "mono";
                        break;
                }

                mSilverPlayer.m_ViewerFormatScript.SetUpViewerFormat();

                return;
            }    
        }

        private void onFrameReady()
        {
            //SwitchVideoFormat();
        }
    }
}
