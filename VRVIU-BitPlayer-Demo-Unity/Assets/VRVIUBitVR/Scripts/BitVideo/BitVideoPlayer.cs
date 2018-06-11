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

			SwitchVideoFormat();

            switch ((Algorithm)data.algorithmType)
            {
                case Algorithm.ROI:
                case Algorithm.FE:
                case Algorithm.ROIMapping:
                case Algorithm.P4:
                    mSilverPlayer = gameObject.AddComponent<SilverMediaPlayerCtrl>();
                    mSilverPlayer.mShaderYUV = mShaderYUV;
                    mSilverPlayer.m_ViewerFormatScript = gameObject.AddComponent<SetViewerFormat>();
                    mSilverPlayer.m_TargetMaterial = mTargetMaterial;
                    mSilverPlayer.OnReady += OnReady;
                    mSilverPlayer.OnVideoFirstFrameReady += OnVideoFirstFrameReady;
                    mSilverPlayer.OnVideoFirstFrameReady += onFrameReady;
                    mSilverPlayer.setUrl(data.url);
                    mediaPlayer = null;
                    break;
                case Algorithm.NONE:
                case Algorithm.NULL:
                    
                    mSilverPlayer = null;
                    mediaPlayer = gameObject.AddComponent<BitPlayerTexture>();
                    mediaPlayer.m_TargetMaterial = mTargetMaterial;
                    mediaPlayer.mShaderYUV = mShaderYUV;
                    mediaPlayer.init(account);
                    mediaPlayer.setUrl(data.url);
                    mediaPlayer.OnVideoFirstFrameReady += OnVideoFirstFrameReady;
                    mediaPlayer.OnVideoFirstFrameReady += onFrameReady;

                    break;
                default:
                    Debug.LogError("Unknown algorithm " + data.algorithmType);
                    mSilverPlayer = null;
                    mediaPlayer.mShaderYUV = mShaderYUV;
                    mediaPlayer.setUrl(data.url);
                    mediaPlayer.OnVideoFirstFrameReady += OnVideoFirstFrameReady;
                    mediaPlayer.OnVideoFirstFrameReady += onFrameReady;
                    break;
            }
        }

        public void Play() {
            if (mSilverPlayer) {
                mSilverPlayer.Play();
            } else {
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

        public void Release(){
            if (mSilverPlayer)
            {
                mSilverPlayer.Stop();
                mSilverPlayer.UnLoad();
            }
            else
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
            else
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
            else
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
            else{
                value = mediaPlayer.GetSeekBarValue();
            }
            return value;
        }


        public void SetSeekBarValue(float fValue) {
            if (mSilverPlayer)
            {
                mSilverPlayer.SetSeekBarValue(fValue);
            }
            else
            {
                mediaPlayer.SetSeekBarValue(fValue);
            }
        }

        public void SeekTo(int iSeek) {
            if (mSilverPlayer)
            {
                mSilverPlayer.SeekTo(iSeek);
            }
            else
            {
                mediaPlayer.SeekTo(iSeek);
            }
        }

        public void SetVolume(float fVolume) {
            if (mSilverPlayer)
            {
                mSilverPlayer.SetVolume(fVolume);
            }
            else
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
            else
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
            MEDIAPLAYER_STATE currentState;
            if (mSilverPlayer)
            {
               currentState = mSilverPlayer.GetCurrentState();
            }
            else
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
            else
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
            else
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
                    case VideoFormat.Mono:
                        SetViewerFormat.m_formatType = "mono";
                        break;
                    case VideoFormat.TopBottom:
                        SetViewerFormat.m_formatType = "stereo_top_bottom";
                        break;
                    case VideoFormat.LeftRight:
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
            if (mData.meshType == (int)MeshType.Erp)
            {
                if (mData.format == (int)VideoFormat.Mono)
                {
                    if (mData.formatDegree == 180)
                    {
                        mVideoFormater.Switch(VideoFormat.OPT_ERP_180);
                    }
                    else
                    {
                        mVideoFormater.Switch(VideoFormat.OPT_ERP_360);
                    }

                }
                else if (mData.format == (int)VideoFormat.LeftRight)
                {
                    
                    if (mData.formatDegree == 180)
                    {
                        mVideoFormater.Switch(VideoFormat.OPT_ERP_180_LR);
                    }
                    else
                    {
                        mVideoFormater.Switch(VideoFormat.OPT_ERP_360_LR);
                    }

                }
                else if (mData.format == (int)VideoFormat.TopBottom)
                {
                    if (mData.formatDegree == 180)
                    {
                        mVideoFormater.Switch(VideoFormat.OPT_ERP_360_LR);
                    }
                    else
                    {
                        mVideoFormater.Switch(VideoFormat.OPT_ERP_360_TB);
                    }

                }
            }
            else if (mData.meshType == (int)MeshType.Fisheye)
            {
                if (mData.format == (int)VideoFormat.Mono)
                {
                    mVideoFormater.Switch(VideoFormat.OPT_FISHEYE);
                }
                else if (mData.format == (int)VideoFormat.LeftRight)
                {
                    mVideoFormater.Switch(VideoFormat.OPT_FISHEYE_LR);
                }
                else if (mData.format == (int)VideoFormat.TopBottom)
                {
                    mVideoFormater.Switch(VideoFormat.OPT_FISHEYE_TB);
                }
            } else if (mData.meshType == (int)MeshType.Trapezoid) {
                if (mData.format == (int)VideoFormat.Mono)
                {
                    mVideoFormater.Switch(VideoFormat.OPT_TROPIZED_MONO);
                }
                else if (mData.format == (int)VideoFormat.LeftRight)
                {
                    mVideoFormater.Switch(VideoFormat.OPT_TROPIZED_LR);
                }
                else if (mData.format == (int)VideoFormat.TopBottom)
                {
                    mVideoFormater.Switch(VideoFormat.OPT_TROPIZED_TB);
                }
            }
		
        }

        private void onFrameReady()
        {
            SwitchVideoFormat();
        }
    }
}
