using VRVIU.BitVRPlayer.BitVideo.Silver;
using System;
using System.Collections;
using UnityEngine;
using VRVIU.BitVRPlayer.BitData;
using VRVIU.BitVRPlayer.BitVRPlayer.BitData;
using System.Text;
using System.Runtime.InteropServices;
using Assets.VRVIUBitVR.Utils;
using System.Collections.Generic;

namespace VRVIU.BitVRPlayer.BitVideo
{
    public class BitVideoPlayer : MonoBehaviour
    {
        private BitPlayerTexture mediaPlayer;
        private SilverMediaPlayerCtrl mSilverPlayer = null;
        public VideoFormater mVideoFormater;
        public VideoCallback.VideoReady OnVideoReady;
        public VideoCallback.VideoEnd OnVideoEnd;
        public VideoCallback.VideoError OnVideoError;
        public VideoCallback.VideoResize OnVideoResize;
        public VideoCallback.VideoFirstFrameReady OnVideoFirstFrameReady;
        public GameObject[] mTargetMaterial = null;
        public Shader mShaderYUV;
        public Account mAccount;
        private string mUrl;
        private int mProjection; //0：未知；1：ERP；2：FISHEYE；3：TROPIZED；4：FLAT
        private int mStereoType;//0：未知；1：2D；2：3D左右；3：3D上下；4：3D右左；5：3D下上
        private int mHfov;//0：未知度数视角；180：180度视角；360：360度视角
        public int mVaid;
        private VideoFormat mFormat;
        private VideoFormat mVideoFormat;
        public object VLog { get; private set; }
        private List<Format> mResolution = null;
        private int mLoop;//1:loop play mode; 0: not loop
        public enum SILVER_ERROR
        {
            SILVER_SUCCESS = 0,
            SILVER_ERROR_GENERAL = -1,
            SILVER_NOT_IMPLEMENTED = -2,
            SILVER_OUT_OF_MEMORY = 1000,
            SILVER_ALREADY_EXIST = 1001,
            SILVER_NOT_EXIST = 1002,
            SILVER_NOT_READY = 1003,
            SILVER_ERROR_INVALID_PARAMETER,
            SILVER_INVALID_STATE,
            SILVER_PARSE_CONFIG_FAIL,
            SILVER_UNHANDLED_EXCEPTION
        }

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

        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverGetViuDescription(String pVr1Path, StringBuilder desc, int size);

        public void SetVid(string vid, int format, Account account)
        {
            mAccount = account;
            StartCoroutine(GetVideoInfo(vid, format, OnStreamInfoBack));
        }

        /**
         * url:指定的vr1或者mp4文件路径
         * 
         **/
        public void SetLocalVideoInfo(string url, int projection, int stereo, int hfov, Account account)
        {
            mAccount = account;
            if (string.IsNullOrEmpty(url))
            {
                Debug.Log("Url is empty!!");
                return;
            }

            if (url.Contains(".vr1"))
            {
                VideoInfo videoInfo = FileManager.getInstance().GetLocalFileVideoInfo(url);
                if (videoInfo != null) {
                    projection = videoInfo.projection;
                    stereo = videoInfo.stereo;
                    hfov = videoInfo.hfov;
                }
                mVaid = (int)DefineType.TYPE3;
                mUrl = videoInfo.url;
            }
            else
            {
                mVaid = (int)DefineType.TYPE1;
                mUrl = url;
            }
            SetUpPlayer(mVaid, projection, stereo, hfov, null);
        }

        /**
         * url:指定的vr1或者mp4文件路径
         * accuont:注册的账号
         **/
        public void SetLocalVideoInfo(string url,Account account)
        {
            mAccount = account;
            if (string.IsNullOrEmpty(url))
            {
                Debug.Log("Url is empty!!");
                return;
            }
            int projection = 0;
            int stereo = 0;
            int hfov = 0;
            if (url.Contains(".vr1"))
            {
                VideoInfo videoInfo = FileManager.getInstance().GetLocalFileVideoInfo(url);
                if (videoInfo != null)
                {
                    projection = videoInfo.projection;
                    stereo = videoInfo.stereo;
                    hfov = videoInfo.hfov;
                }
                mVaid = (int)DefineType.TYPE3;
                mUrl = videoInfo.url;
            }
            else
            {
                mVaid = (int)DefineType.TYPE1;
                mUrl = url;
            }
            SetUpPlayer(mVaid, projection, stereo, hfov, null);
        }

        private void SetUpPlayer(int vaid, int projection, int stereo, int hfov, string info)
        {
            if (mVideoFormater != null) {
                mVideoFormater.SetUp();
            }
            switch ((DefineType)vaid)
            {
                case DefineType.TYPE4:
                case DefineType.TYPE3:
                case DefineType.TYPE5:
                case DefineType.TYPE2:
                case DefineType.TYPE8:
                    mSilverPlayer = gameObject.AddComponent<SilverMediaPlayerCtrl>();
                    mSilverPlayer.mShaderYUV = mShaderYUV;
                    mSilverPlayer.m_ViewerFormatScript = gameObject.AddComponent<SetViewerFormat>();
                    mSilverPlayer.OnReady += OnReady;
                    mSilverPlayer.OnVideoFirstFrameReady += OnFirstFrameReady;
                    mSilverPlayer.OnEnd += OnEnd;
                    mSilverPlayer.OnVideoError += OnError;
                    mSilverPlayer.OnResize += OnResize;
                    mSilverPlayer.SetLoopPlay(mLoop);
                    if (!string.IsNullOrEmpty(mUrl))
                    {
                        mSilverPlayer.setUrl(mUrl);
                    }
                    else
                    {
                        mSilverPlayer.setVideoInfo(info);
                    }

                    mediaPlayer = null;
                    break;
                case DefineType.TYPE1:
                case DefineType.NULL:
                    mSilverPlayer = null;
                    mediaPlayer = gameObject.AddComponent<BitPlayerTexture>();
                    mediaPlayer.m_TargetMaterial = mTargetMaterial;
                    mediaPlayer.mShaderYUV = mShaderYUV;
                    mediaPlayer.init(mAccount);
                    mediaPlayer.setUrl(mUrl);
                    mediaPlayer.OnVideoFirstFrameReady += OnFirstFrameReady;
                    mediaPlayer.OnResize += OnResize;
                    mediaPlayer.OnVideoError += OnError;
                    mediaPlayer.OnEnd += OnEnd;
                    mediaPlayer.OnReady += OnReady;
                    break;
                default:
                    Debug.LogError("Unknown algorithm " + vaid);
                    mSilverPlayer = gameObject.AddComponent<SilverMediaPlayerCtrl>();
                    mSilverPlayer.mShaderYUV = mShaderYUV;
                    mSilverPlayer.m_ViewerFormatScript = gameObject.AddComponent<SetViewerFormat>();
                    //mSilverPlayer.m_TargetMaterial = mTargetMaterial;
                    mSilverPlayer.OnReady += OnReady;
                    mSilverPlayer.OnVideoFirstFrameReady += OnFirstFrameReady;
                    mSilverPlayer.OnEnd += OnEnd;
                    mSilverPlayer.OnVideoError += OnError;
                    mSilverPlayer.OnResize += OnResize;
                    mSilverPlayer.SetLoopPlay(mLoop);
                    if (!string.IsNullOrEmpty(mUrl))
                    {
                        mSilverPlayer.setUrl(mUrl);
                    }
                    else
                    {
                        mSilverPlayer.setVideoInfo(info);
                    }
                    mediaPlayer = null;
                    break;
            }
            SetRenderMode((VideoPorjection)projection, (VideoSteroType)stereo, (VideoHfov)hfov, (VideoPorjection)projection, (VideoSteroType)stereo, (VideoHfov)hfov);
        }

        private void OnStreamInfoBack(StreamInfoEnity streamInfo)
        {
            Format formatSelected = null;
            int vaid = 0;
            mResolution = streamInfo.formats;
            if (streamInfo.formats != null && streamInfo.formats.Count > 0)
            {
                foreach (Format tmp in streamInfo.formats)
                {
                    if (tmp.selected == 1)
                    {
                        formatSelected = tmp;
                    }
                }
            }
            if (formatSelected != null)
            {
                vaid = formatSelected.vaid;
            }

            if (vaid == (int)DefineType.TYPE1 || vaid == (int)DefineType.NULL)
            {

                int pro = 0;
                for (int i = 0; i < streamInfo.live_urls[0].views[0].urls.Count; i++)
                {
                    int tmp = streamInfo.live_urls[0].views[0].urls[i].priority;
                    if (tmp >= pro)
                    {
                        mUrl = streamInfo.live_urls[0].views[0].urls[i].url;
                        pro = tmp;
                    }
                }
            }
            SetUpPlayer(vaid, formatSelected.projection, formatSelected.stereo, formatSelected.hfov, streamInfo.total_content);
        }

        private IEnumerator GetVideoInfo(string vid, int format, Action<StreamInfoEnity> OnStreamInfoBack)
        {
            string url = NetAPIs.SERVER_GET_VIDEO_INFO + "&vid=" + vid + "&format=" + format;
            WWW web = new WWW(url);
            yield return web;

            if (web.error == null)
            {
                Debug.Log("VideoInfo url " + url);
                StreamInfoEnity streamInfo = JsonUtility.FromJson<StreamInfoEnity>(web.text);
                
                if (streamInfo.ret.code == 0)
                {
                    streamInfo.total_content = web.text;
                    OnStreamInfoBack(streamInfo);
                }
                else {
                    Debug.Log("VideoInfo error " + streamInfo.ret.msg);
                }
                
            }
            else
            {
                Debug.Log("VideoInfo error " + url);
            }
        }

        public List<Format> GetResolution() {
            return mResolution;
        }

        public static  VideoFormat ConvertFormat(VideoPorjection projection, VideoSteroType stereoType, VideoHfov hfov)
        {
            VideoFormat videoFormat = VideoFormat.OPT_ERP_360_MONO;
            if (projection == VideoPorjection.OPT_ERP)
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
                        else if (hfov == VideoHfov.DEGREE_360)
                        {
                            videoFormat = VideoFormat.OPT_ERP_360_LR;
                        }
                        else
                        {
                            videoFormat = VideoFormat.OPT_ERP_360_LR;
                        }
                        break;
                    case VideoSteroType.OPT_STERO_TB:
                        if (hfov == VideoHfov.DEGREE_180)
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
                    case VideoSteroType.OPT_MONO:
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

        public void SetRenderMode(VideoPorjection projection, VideoSteroType steroType, VideoHfov hfov, VideoPorjection videoProjection, VideoSteroType videoSteroType, VideoHfov videoHfov)
        {
            mFormat = ConvertFormat(projection, steroType, hfov);

            Debug.Log("SetRenderMode videoProjection " + videoProjection + " videoSteroType " + videoSteroType + " videoHfov " + videoHfov);
            mVideoFormat = ConvertFormat(videoProjection, videoSteroType, videoHfov);
            if (mediaPlayer)
            {
                Debug.Log("media player set video forma projection " + projection + " steroType " + steroType + " hfov " + hfov);
                mVideoFormater.Switch(mFormat, mVideoFormat);
            }

            if (mSilverPlayer)
            {
                Debug.Log("silver set video format projection " + projection + " steroType " + steroType + " hfov " + hfov);
                SwitchVideoFormat();
            }
        }

        public void AutoVideoFormat()
        {
            if (mediaPlayer)
            {
                //mediaPlayer.SetVideoFormat(format);
            }
        }

        public void Play()
        {
            if (mSilverPlayer)
            {
                mSilverPlayer.Play();
            }
            else if (mediaPlayer)
            {
                mediaPlayer.Play();
            }
        }

        public void Pause()
        {
            if (mSilverPlayer)
            {
                mSilverPlayer.Pause();
            }
            else
            {
                mediaPlayer.Pause();
            }
        }

        public bool IsUpdateFrame()
        {
            bool bUpdate = false;
            if (mSilverPlayer)
            {
                
                if(mSilverPlayer.GetCurrentState() == MEDIAPLAYER_STATE.PLAYING && mSilverPlayer.IsReadyToRender())
                {
                    bUpdate = true;
                }
                
            }
            else
            {
                if(mediaPlayer)
                {
                    bUpdate = mediaPlayer.IsUpdateFrame();
                }
                else
                {
                    Debug.Log("mediaPlayer is null!");
                }
            }
            return bUpdate;
        }

        public void SetLoopPlay(int loop)
        {
            mLoop = loop;
        }

        public void Resume()
        {
            if (mSilverPlayer)
            {
                mSilverPlayer.Resume();
            }
            else if (mediaPlayer)
            {
                //mediaPlayer.Resume();
            }
        }

        public bool IsReadyToRender()
        {
            if (mSilverPlayer)
            {
                return mSilverPlayer.IsReadyToRender();
            }
            else if (mediaPlayer)
            {
                //mediaPlayer.SetLoopPlay(loop);
                return true;
            }
            return true;
        }

        public void RePlay()
        {
            if (mSilverPlayer)
            {
                mSilverPlayer.RePlay();
            }
            else if(mediaPlayer)
            {
                mediaPlayer.Play();
            }
        }

        public void Release()
        {
            mUrl = null;
            if (mSilverPlayer)
            {
                mSilverPlayer.OnReady -= OnReady;
                mSilverPlayer.OnVideoFirstFrameReady -= OnFirstFrameReady;
                mSilverPlayer.OnEnd -= OnEnd;
                mSilverPlayer.OnVideoError -= OnError;
                mSilverPlayer.OnResize -= OnResize;
                Destroy(gameObject.GetComponent<SilverMediaPlayerCtrl>());
                mSilverPlayer = null;
            }
            else if (mediaPlayer)
            {
                if (mVideoFormater != null)
                {
                    mVideoFormater.Release();
                }
                mediaPlayer.OnReady -= OnReady;
                mediaPlayer.OnVideoFirstFrameReady -= OnFirstFrameReady;
                mediaPlayer.OnEnd -= OnEnd;
                mediaPlayer.OnVideoError -= OnError;
                mediaPlayer.OnResize -= OnResize;
                Destroy(gameObject.GetComponent<BitPlayerTexture>());
                mediaPlayer = null;
            }
        }

        public int GetVideoWidth()
        {
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

        public int GetVideoHeight()
        {
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

        public float GetSeekBarValue()
        {
            float value = 0;
            if (mSilverPlayer)
            {
                value = mSilverPlayer.GetSeekBarValue();
            }
            else if (mediaPlayer)
            {
                value = mediaPlayer.GetSeekBarValue();
            }
            return value;
        }


        public void SetSeekBarValue(float fValue)
        {
            if (mSilverPlayer)
            {
                mSilverPlayer.SetSeekBarValue(fValue);
            }
            else if (mediaPlayer)
            {
                mediaPlayer.SetSeekBarValue(fValue);
            }
        }

        public void SeekTo(int iSeek)
        {
            if (mSilverPlayer)
            {
                int duration = (int)(mSilverPlayer.GetDuration() / 1000);
                if (duration - iSeek <= 3000)
                {
                    iSeek = duration - 3000;
                }
                mSilverPlayer.SeekTo(iSeek * 1000);
            }
            else if (mediaPlayer)
            {
                mediaPlayer.SeekTo(iSeek);
            }
        }

        public void SetVolume(float fVolume)
        {
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
            else if (mediaPlayer)
            {
                currentState = mediaPlayer.GetCurrentState();
            }
            return currentState;
        }

        public  int  GetErrorCode()
        {
            if (mSilverPlayer)
            {
                return (int)mSilverPlayer.GetError();
            }
            else if(mediaPlayer){
                return (int)mediaPlayer.GetError();
            }
            return -110000;
        }

        private int  GetErrorExtra(int error)
        {
            if (mSilverPlayer)
            {
                return  mSilverPlayer.GetErrorExtra();
            }
            else if (mediaPlayer)
            {
                return  mediaPlayer.GetErrorExtra();
            }
            return -11000;
        }

        public void Reload()
        {
            switch ((DefineType)mVaid)
            {
                case DefineType.TYPE4:
                case DefineType.TYPE3:
                case DefineType.TYPE5:
                    mSilverPlayer = new SilverMediaPlayerCtrl();
                    break;
                default:
                    mSilverPlayer = null;
                    break;
            }

            if (mSilverPlayer)
            {
                mSilverPlayer.SetLoopPlay(mLoop);
                mSilverPlayer.setUrl(mUrl);
            }
            else if (mediaPlayer)
            {
                mediaPlayer.setUrl(mUrl);
            }
        }

        public void Stop()
        {
            if (mSilverPlayer)
            {
                mSilverPlayer.Stop();
            }
            else if (mediaPlayer)
            {
                mediaPlayer.Stop();
            }
        }
       
        public int GetDuration()
        {
            int duration = 0;
            if (mSilverPlayer)
            {
                duration = (int)(mSilverPlayer.GetDuration() / 1000);
            }
            else if (mediaPlayer)
            {
                duration = mediaPlayer.GetDuration();
            }
            return duration;
        }

        public long GetNetWorkSpeed()
        {
            if (mSilverPlayer)
            {
                return (long)mSilverPlayer.GetNetWorkSpeed()/8;
            }
            else
            {
                return mediaPlayer.GetNetWorkSpeed();
            }
        }

        public int GetPlayPosition()
        {
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
                switch ((VideoFormat)mFormat)
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
                        Debug.LogError("Unknown format " + mFormat);
                        SetViewerFormat.m_formatType = "mono";
                        break;
                }
                switch ((VideoFormat)mVideoFormat)
                {
                    case VideoFormat.OPT_ERP_180_MONO:
                    case VideoFormat.OPT_ERP_360_MONO:
                    case VideoFormat.OPT_FISHEYE_MONO:
                    case VideoFormat.OPT_FLAT_MONO:
                        SetViewerFormat.m_videoFormatType = "mono";
                        break;
                    case VideoFormat.OPT_ERP_180_TB:
                    case VideoFormat.OPT_ERP_360_TB:
                    case VideoFormat.OPT_FISHEYE_TB:
                    case VideoFormat.OPT_FLAT_TB:
                        SetViewerFormat.m_videoFormatType = "stereo_top_bottom";
                        break;
                    case VideoFormat.OPT_ERP_180_LR:
                    case VideoFormat.OPT_ERP_360_LR:
                    case VideoFormat.OPT_FISHEYE_LR:
                    case VideoFormat.OPT_FLAT_LR:
                        SetViewerFormat.m_videoFormatType = "stereo_left_right";
                        break;
                    default:
                        Debug.LogError("Unknown format " + mFormat);
                        SetViewerFormat.m_videoFormatType = "mono";
                        break;
                }

                mSilverPlayer.m_ViewerFormatScript.SetUpViewerFormat();

                return;
            }
        }

        void OnEnd()
        {
            if (OnVideoEnd != null) {
                OnVideoEnd();
            }
        }

        void OnError(int errorCode, int errorCodeExtra) {
            if (OnVideoError != null) {
                OnVideoError(errorCode, errorCodeExtra);
            }
        }

        void OnFirstFrameReady() {
            if (OnVideoFirstFrameReady != null) {
                OnVideoFirstFrameReady();
            }
        }

        void OnResize() {
            if (OnVideoResize != null) {
                OnVideoResize();
            }
        }

        void OnReady() {
            if (OnVideoReady != null) {
                OnVideoReady();
            }
        }
    }
}
