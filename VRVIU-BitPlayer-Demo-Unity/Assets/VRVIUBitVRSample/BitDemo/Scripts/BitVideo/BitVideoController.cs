//-----------------------------------------------------
//            VRVIU: VRVIU-BitPlayer-SDK-Unity
//            Author: kevin.zha@vrviu.com  
// Copyright © 2016-2018 VRVIU Technologies Limited. 
//-----------------------------------------------------

namespace Demo.Video
{
    using System.Collections;

    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.XR;

    using VRVIU.BitVRPlayer.BitVideo;
    using VRVIU.BitVRPlayer.BitData;
    using UnityEngine.SceneManagement;
    using Demo.Ui.Video;
    using System.Collections.Generic;
    using VRVIU.LightVR.UI;
    public enum PlayerSwitchDefinition
    {
        DEFINITION_NORMAL_2K = 1,
        DEFINITION_NORMAL_2P5K = 2,
        DEFINITION_NORMAL_3K = 3,
        DEFINITION_NORMAL_4K = 4,
        DEFINITION_FE_3K = 5,
        DEFINITION_FE_4K = 6,
        DEFINITION_FE_8K = 7,
        DEFINITION_FE_6K = 10,
        DEFINITION_AUTO = 999
    }
    public class BitVideoController : MonoBehaviour
    {
        #region Fields

        public Button mBackButton;

        public Text mNameText;

        public Text mBitrateText;

        public Text mPositionText;
          
        public Text mDurationText;

        public Button mPauseButton;

        public Button mPlayButton;

        public Button mRenderModeBtn;

        public GameObject mRenderModeOption;

        public GameObject mDefinitionBtn;

        public GameObject mDefinitionOption;

        public Button mMoreBtn;

        public GameObject m180VROption;

        public GameObject m360VROption;

        public GameObject mIMAXOption;

        public GameObject mLROption;

        public GameObject mTBOption;

        public GameObject mMonoOption; 

        public Button mHidePanelBtn;

        public GameObject mVideoCtrlPanel;

        public SeekBarAdjuster seekBarAdjuster;

        public BitVideoPlayer mPlayer;

        public VideoFormater videoFormater;

        public RawImage loadingImage;
        public RawImage loadingImageLeftBg;
        public RawImage loadingImageRigthBg;

        public GyroCtrl gyroCtrl;

        public VRCtrl vrCtrl;

        private VideoInfo mVideoInfo;

        private Account account;

        private Coroutine showOperationCoroutine;

        private bool isLoading = false;

        private bool isRenderModeShow = false;

        private bool isDefinitionShow = false;

        private bool m_bReadyToRender = false;

        private bool isLoadingShow = false;

        private bool isVideoCtrlPanelShow = false;

        private VideoPorjection m_projection = VideoPorjection.OPT_ERP; //0：未知；1：ERP；2：FISHEYE；3：TROPIZED；4：FLAT

        private VideoSteroType m_stero_type = VideoSteroType.OPT_MONO;//0：未知；1：2D；2：3D左右；3：3D上下；4：3D右左；5：3D下上

        private VideoHfov m_hfov = VideoHfov.DEGREE_360;//0：未知度数视角；180：180度视角；360：360度视角

        public GameObject m_imax_room;
        private int MAX_FORMATS_NUM = 8;

        public List<ClickedButton> m_formatsBtnList;

        public GameObject m_definitionBtns;// definition buttons

        public GameObject m_definitionText; // current select definition

        private bool m_bIsInitDefaultDefi = false;

        private int mCurrentDefiID = 0;

        private string[] colors = { "2D63FFFF", "FFFFFFFF", "000000FF", "C8C8C880", "090909F0" };
        private enum ColorEnum
        {
            blue = 0,
            white = 1,
            black = 2,
            gray = 3,
            gray2 = 4
        }
        private List<Format> m_formats;
        #endregion

        #region Methods

        private void Awake()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            XRSettings.enabled = true;
        }
        void Start()
        {
            RegisterEvents();
            mVideoInfo = BitLobby.videoData;
            account = BitLobby.account;
            if (string.IsNullOrEmpty(mVideoInfo.vid) && !string.IsNullOrEmpty(mVideoInfo.url))
            {
                mPlayer.SetLocalVideoInfo(mVideoInfo.url, mVideoInfo.projection, mVideoInfo.stereo, mVideoInfo.hfov, account);
            }
            else {
                mPlayer.SetVid(mVideoInfo.vid, mVideoInfo.format, account);
            }
            m_projection = (VideoPorjection)mVideoInfo.projection;
            m_stero_type = (VideoSteroType)mVideoInfo.stereo;
            m_hfov = (VideoHfov)mVideoInfo.hfov;
            mPlayer.SetLoopPlay(-1);
            //seekBarAdjuster.player = mPlayer;
            isLoading = true;
            mVideoCtrlPanel.SetActive(isVideoCtrlPanelShow);
            mHidePanelBtn.gameObject.SetActive(isVideoCtrlPanelShow);
            mMoreBtn.gameObject.SetActive(!isVideoCtrlPanelShow);
            mDefinitionBtn.SetActive(!isDefinitionShow);
            mDefinitionOption.SetActive(isDefinitionShow);
            ShowHideLoadingView(true);
        }

        private void Update()
        {
#if UNITY_IOS

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Back();
            }
            
#elif UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    Back();
                }
            }

#endif
            int duration = 0;
            duration = mPlayer.GetDuration();
            mPositionText.text = FormatTime(mPlayer.GetPlayPosition());
            mDurationText.text = FormatTime(duration);

            if (!m_bReadyToRender && mPlayer != null)
            {
                m_bReadyToRender = mPlayer.IsUpdateFrame();
            }

            if (m_bReadyToRender&& isLoadingShow) {
                isLoadingShow = false;
                ShowHideLoadingView(isLoadingShow);
            }
            if (!m_bIsInitDefaultDefi)
            {
                if (mPlayer != null && mPlayer.GetCurrentState() == MEDIAPLAYER_STATE.PLAYING && mPlayer.GetResolution() != null)
                {
                    m_bIsInitDefaultDefi = true;
                    m_formats = mPlayer.GetResolution();
                    foreach (Format fmt in m_formats)
                    {
                        if (fmt.selected == 1)
                        {
                            mVideoInfo.vaid = fmt.vaid;
                            m_stero_type = (VideoSteroType)fmt.stereo;
                            m_projection = (VideoPorjection)fmt.projection;
                            m_hfov = (VideoHfov)fmt.hfov;
                            break;
                        }
                    }

                    m_formats.Sort((left, right) =>
                    {
                        if (left.id > right.id)
                            return 1;
                        else if (left.id == right.id)
                            return 0;
                        else
                            return -1;
                    });
                    SetFormatsBtnInfo();
                }
            }
            
        }

        private void RegisterEvents()
        {
            mBackButton.onClick.AddListener(delegate ()
            {
                this.OnClick(mBackButton.gameObject);
            });

          
            mPauseButton.onClick.AddListener(delegate ()
            {
                this.OnClick(mPauseButton.gameObject);
            });

            mPlayButton.onClick.AddListener(delegate ()
            {
                this.OnClick(mPlayButton.gameObject);
            });

            mDefinitionBtn.GetComponent<ClickedButton>().onClick.AddListener(delegate() {
                if (m_formats != null && m_formats.Count > 0)
                {
                    isDefinitionShow = !isDefinitionShow;
                    mDefinitionOption.SetActive(isDefinitionShow);
                }
            });

            mMoreBtn.onClick.AddListener(delegate() {
                isVideoCtrlPanelShow = !isVideoCtrlPanelShow;
                mVideoCtrlPanel.SetActive(isVideoCtrlPanelShow);
                mMoreBtn.gameObject.SetActive(!isVideoCtrlPanelShow);
                mHidePanelBtn.gameObject.SetActive(isVideoCtrlPanelShow);
                mRenderModeOption.SetActive(false);
            });

            mHidePanelBtn.onClick.AddListener(delegate() {
                isVideoCtrlPanelShow = !isVideoCtrlPanelShow;
                mVideoCtrlPanel.SetActive(isVideoCtrlPanelShow);
                mMoreBtn.gameObject.SetActive(!isVideoCtrlPanelShow);
                mHidePanelBtn.gameObject.SetActive(isVideoCtrlPanelShow);
            });

            mRenderModeBtn.onClick.AddListener(delegate() {
                isRenderModeShow = !isRenderModeShow;
                mRenderModeOption.SetActive(isRenderModeShow);
                SetFormatOptionsStatus();
            });

            mIMAXOption.GetComponent<ClickedButton>().onClick.AddListener(delegate() {
                OnSetupIMaxBtnClicked();
            });

            m180VROption.GetComponent<ClickedButton>().onClick.AddListener(delegate () {
                OnSetup180VRBtnClicked();
            });

            m360VROption.GetComponent<ClickedButton>().onClick.AddListener(delegate () {
                OnSetup360VRBtnClicked();
            });

            mLROption.GetComponent<ClickedButton>().onClick.AddListener(delegate () {
                OnSetupLF3DBtnClicked();
            });

            mTBOption.GetComponent<ClickedButton>().onClick.AddListener(delegate () {
                OnSetupTB3DBtnClicked();
            });

            mMonoOption.GetComponent<ClickedButton>().onClick.AddListener(delegate () {
                OnSetupMonoBtnClicked();
            });
        }
          
        
        private void SetFormatsBtnInfo()
        {
            for (int i = 0; i < MAX_FORMATS_NUM; i++)
            {
                m_formatsBtnList[i].gameObject.SetActive(false);
            }

            if (m_formats != null && m_formats.Count > 0 && m_formats.Count <= MAX_FORMATS_NUM)
            {
                for (int i = 0; i < m_formats.Count; i++)
                {
                    m_formatsBtnList[i].gameObject.SetActive(true);
                    m_formatsBtnList[i].GetComponentInChildren<Text>().text = m_formats[i].display_name;
                    m_formatsBtnList[i].GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.gray2]);
                    if (m_formats[i].selected == 1)
                    {
                        mCurrentDefiID = m_formats[i].id;
                        m_formatsBtnList[i].GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.blue]);
                        m_definitionText.GetComponent<Text>().text = m_formats[i].display_name;
                    }

                    m_formatsBtnList[i].onClick.RemoveAllListeners();

                    switch (m_formats[i].id)
                    {
                        case (int)PlayerSwitchDefinition.DEFINITION_FE_8K:
                            m_formatsBtnList[i].onClick.AddListener(delegate ()
                            {
                                OnFE8KBtnClicked();
                            });
                            break;
                        case (int)PlayerSwitchDefinition.DEFINITION_FE_6K:
                            m_formatsBtnList[i].onClick.AddListener(delegate ()
                            {
                                OnFE6KBtnClicked();
                            });
                            break;
                        case (int)PlayerSwitchDefinition.DEFINITION_FE_4K:
                            m_formatsBtnList[i].onClick.AddListener(delegate ()
                            {
                                OnFE4KBtnClicked();
                            });
                            break;
                        case (int)PlayerSwitchDefinition.DEFINITION_FE_3K:
                            m_formatsBtnList[i].onClick.AddListener(delegate ()
                            {
                                OnEF3KBtnClicked();

                            });
                            break;
                        case (int)PlayerSwitchDefinition.DEFINITION_NORMAL_4K:
                            m_formatsBtnList[i].onClick.AddListener(delegate ()
                            {
                                On4KBtnClicked();
                            });
                            break;
                        case (int)PlayerSwitchDefinition.DEFINITION_NORMAL_3K:
                            m_formatsBtnList[i].onClick.AddListener(delegate ()
                            {
                                On3KBtnClicked();
                            });
                            break;
                        case (int)PlayerSwitchDefinition.DEFINITION_NORMAL_2P5K:
                            m_formatsBtnList[i].onClick.AddListener(delegate ()
                            {
                                On2_5KBtnClicked();
                            });
                            break;
                        case (int)PlayerSwitchDefinition.DEFINITION_NORMAL_2K:
                            m_formatsBtnList[i].onClick.AddListener(delegate ()
                            {
                                On2KBtnClicked();

                            });
                            break;
                        default:
                            break;
                    }
                }
            }


        }
         
        private string GetDefinitionName(int dfID)
        {
            string name = "";

            foreach (Format tmp in m_formats)
            {
                if (tmp.id == dfID)
                {
                    name = tmp.display_name;
                }
            }
            return name;
        }

        private void SwitchDefinition(int definition_format) {
            m_bReadyToRender = false;
            ShowHideLoadingView(true);
            mVideoInfo.format = definition_format;
            if (mPlayer == null)
            {
                return;
            }
            m_bIsInitDefaultDefi = false;
            // player.Stop();
            mPlayer.Release();
            PlayByVid(mVideoInfo);
        }

        private void PlayByVid(VideoInfo vi)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                m_projection = (VideoPorjection)vi.projection;
                m_stero_type = (VideoSteroType)vi.stereo;
                m_hfov = (VideoHfov)vi.hfov;
                mPlayer.SetVid(vi.vid, vi.format, account); 
                mPlayer.SetLoopPlay(1);
                //player.OnVideoEnd += PlayVideoEnd;
                mPlayer.Play();
            }
            else
            {
            }
        }

        private void ShowHideLoadingView(bool bShow)
        {
            isLoadingShow = bShow;
            loadingImage.gameObject.SetActive(bShow);
            loadingImageLeftBg.gameObject.SetActive(bShow);
            loadingImageRigthBg.gameObject.SetActive(bShow);
        }
        /// <summary>
        /// switch to FE8K
        /// </summary>
        private void OnFE8KBtnClicked()
        {
            if (mCurrentDefiID == (int)PlayerSwitchDefinition.DEFINITION_FE_8K)
            {
                return;
            } 
            m_definitionText.GetComponent<Text>().text = GetDefinitionName((int)PlayerSwitchDefinition.DEFINITION_FE_8K);
            SwitchDefinition((int)PlayerSwitchDefinition.DEFINITION_FE_8K);
        }

        /// <summary>
        /// switch to FE6K
        /// </summary>
        private void OnFE6KBtnClicked()
        {
            if (mCurrentDefiID == (int)PlayerSwitchDefinition.DEFINITION_FE_6K)
            {
                return;
            } 
            m_definitionText.GetComponent<Text>().text = GetDefinitionName((int)PlayerSwitchDefinition.DEFINITION_FE_6K);
            SwitchDefinition((int)PlayerSwitchDefinition.DEFINITION_FE_6K);
        }


        /// <summary>
        /// switch to FE4K
        /// </summary>
        private void OnFE4KBtnClicked()
        {
            if (mCurrentDefiID == (int)PlayerSwitchDefinition.DEFINITION_FE_4K)
            {
                return;
            } 
            m_definitionText.GetComponent<Text>().text = GetDefinitionName((int)PlayerSwitchDefinition.DEFINITION_FE_4K);
            SwitchDefinition((int)PlayerSwitchDefinition.DEFINITION_FE_4K);
        }

        /// <summary>
        /// switch to FE3K
        /// </summary>
        private void OnEF3KBtnClicked()
        {
            if (mCurrentDefiID == (int)PlayerSwitchDefinition.DEFINITION_FE_3K)
            {
                return;
            } 
            m_definitionText.GetComponent<Text>().text = GetDefinitionName((int)PlayerSwitchDefinition.DEFINITION_FE_3K);
            SwitchDefinition((int)PlayerSwitchDefinition.DEFINITION_FE_3K);
        }

        /// <summary>
        /// switch to 4K
        /// </summary>
        private void On4KBtnClicked()
        {
            if (mCurrentDefiID == (int)PlayerSwitchDefinition.DEFINITION_NORMAL_4K)
            {
                return;
            } 
            m_definitionText.GetComponent<Text>().text = GetDefinitionName((int)PlayerSwitchDefinition.DEFINITION_NORMAL_4K);
            SwitchDefinition((int)PlayerSwitchDefinition.DEFINITION_NORMAL_4K);
        }


        /// <summary>
        /// switch to 3K
        /// </summary>
        private void On3KBtnClicked()
        {
            if (mCurrentDefiID == (int)PlayerSwitchDefinition.DEFINITION_NORMAL_3K)
            {
                return;
            } 
            m_definitionText.GetComponent<Text>().text = GetDefinitionName((int)PlayerSwitchDefinition.DEFINITION_NORMAL_3K);
            SwitchDefinition((int)PlayerSwitchDefinition.DEFINITION_NORMAL_3K);
        }

        /// <summary>
        /// switch to 2.5k
        /// </summary>
        private void On2_5KBtnClicked()
        {
            if (mCurrentDefiID == (int)PlayerSwitchDefinition.DEFINITION_NORMAL_2P5K)
            {
                return;
            } 
            m_definitionText.GetComponent<Text>().text = GetDefinitionName((int)PlayerSwitchDefinition.DEFINITION_NORMAL_2P5K);
            SwitchDefinition((int)PlayerSwitchDefinition.DEFINITION_NORMAL_2P5K);
        }
        /// <summary>
        /// switch to 2k
        /// </summary>
        private void On2KBtnClicked()
        {
            if (mCurrentDefiID == (int)PlayerSwitchDefinition.DEFINITION_NORMAL_2K)
            {
                return;
            } 
            m_definitionText.GetComponent<Text>().text = GetDefinitionName((int)PlayerSwitchDefinition.DEFINITION_NORMAL_2K);
            SwitchDefinition((int)PlayerSwitchDefinition.DEFINITION_NORMAL_2K);
        }

        private Color HexToColor(string hex)
        {
            byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            float r = br / 255f;
            float g = bg / 255f;
            float b = bb / 255f;
            float a = cc / 255f;
            return new Color(r, g, b, a);
        }

        private string GetTextColor(string text, string color)
        {
            return string.Format("<color=#{0}>{1}</color>", color, text);
        }

        private void SetFormatOptionsStatus()
        {
            //针对vrviu转码的视频，已经知道格式，可以设置为指定格式
            if (mVideoInfo == null)
            {
                return;
            }
            //Debug.Log("PlayCtrView mCurrentVideoInfo projection " + mCurrentVideoInfo.projection +m_projection+
            //    " hfov " + mCurrentVideoInfo.hfov +m_hfov+ " stero " + mCurrentVideoInfo.stereo+m_stero_type);

            if (m_projection == VideoPorjection.OPT_FLAT)
            {
                mIMAXOption.GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.blue]);
            }
            else
            {
                mIMAXOption.GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.white]);
                
            }

            if (mVideoInfo.projection != (int)VideoPorjection.OPT_FLAT)
            {
                mIMAXOption.GetComponent<ClickedButton>().interactable = false;
                mIMAXOption.GetComponent<ClickedButton>().enabled = false;
                mIMAXOption.GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.gray]);
                string strText = mIMAXOption.GetComponentInChildren<Text>().text;
                mIMAXOption.GetComponentInChildren<Text>().text = GetTextColor(strText, colors[(int)ColorEnum.gray]);
            }


            if ( m_hfov == VideoHfov.DEGREE_360)
            {
                m360VROption.GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.blue]);
            }
            else
            {
                m360VROption.GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.white]);
               
            }

            if (m_hfov == VideoHfov.DEGREE_180)
            {
                m180VROption.GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.blue]);
            }
            else
            {
                m180VROption.GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.white]);
               
            }
             

            if (m_stero_type == VideoSteroType.OPT_STERO_LR)
            {
                mLROption.GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.blue]);
            }
            else
            {
                mLROption.GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.white]);
            }
             
            if (m_stero_type == VideoSteroType.OPT_STERO_TB)
            {
                mTBOption.GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.blue]);
            }
            else
            {
                mTBOption.GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.white]);
            }

            
            if (m_stero_type == VideoSteroType.OPT_MONO)
            {
                mMonoOption.GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.blue]);
            }
            else
            {
                mMonoOption.GetComponent<Image>().color = HexToColor(colors[(int)ColorEnum.white]);
            } 
        }

        private void SetVideoFormat()
        {
            mPlayer.SetRenderMode(m_projection, m_stero_type, m_hfov, (VideoPorjection)mVideoInfo.projection, (VideoSteroType)mVideoInfo.stereo, (VideoHfov)mVideoInfo.hfov);
            if (m_projection == VideoPorjection.OPT_FLAT)
            {
                videoFormater.leftSide.transform.localScale = new Vector3(1920, 960, 25);
                videoFormater.rightSide.transform.localScale = new Vector3(1920, 960, 25);
                videoFormater.rightSide.transform.position = new Vector3(0, 220, 1500);
                videoFormater.leftSide.transform.position = new Vector3(0, 220, 1500);
            }
            else
            {
                videoFormater.leftSide.transform.localScale = new Vector3(3000, 3000, 3000);
                videoFormater.rightSide.transform.localScale = new Vector3(3000, 3000, 3000);
                videoFormater.leftSide.transform.position = new Vector3(0, 0, 0);
                videoFormater.rightSide.transform.position = new Vector3(0, 0, 0);

            }

        }

        /// <summary>
        /// Setup 2D mode
        /// </summary>
        private void OnSetupIMaxBtnClicked()
        {
            m_imax_room.SetActive(true);
            m_projection = VideoPorjection.OPT_FLAT;
            SetFormatOptionsStatus();
            SetVideoFormat();
        }

        /// <summary>
        /// Setup 180VR mode
        /// </summary>
        private void OnSetup180VRBtnClicked()
        {
            m_imax_room.SetActive(false);
            m_projection = VideoPorjection.OPT_ERP;
            m_hfov = VideoHfov.DEGREE_180;
            SetFormatOptionsStatus();
            SetVideoFormat();
        }


        /// <summary>
        /// Setup 360VR mode
        /// </summary>
        private void OnSetup360VRBtnClicked()
        {
            m_imax_room.SetActive(false);
            m_projection = VideoPorjection.OPT_ERP;
            m_hfov = VideoHfov.DEGREE_360;
            SetFormatOptionsStatus();
            SetVideoFormat();
        }

        /// <summary>
        /// Setup Normal mode
        /// </summary>
        private void OnSetupMonoBtnClicked()
        {
            m_stero_type = VideoSteroType.OPT_MONO;
            SetFormatOptionsStatus();
            SetVideoFormat();
        }


        /// <summary>
        /// Setup LF3D mode
        /// </summary>
        private void OnSetupLF3DBtnClicked()
        {
            m_stero_type = VideoSteroType.OPT_STERO_LR;
            SetFormatOptionsStatus();
            SetVideoFormat();
        }


        /// <summary>
        /// Setup TB3D mode
        /// </summary>
        private void OnSetupTB3DBtnClicked()
        {
            m_stero_type = VideoSteroType.OPT_STERO_TB;
            SetFormatOptionsStatus();
            SetVideoFormat();
        }

        private bool CheckOnlineStatus()
        {
            return true;
        }

        

        private void OnDestroy()
        {
            mPlayer.OnVideoReady -= OnReady;
            mPlayer.Release();
        }

        private void OnClick(GameObject go)
        {
            if (go == mBackButton.gameObject)
            {
                Back();
            }
           
            else if (go == mPauseButton.gameObject)
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
                mPlayer.Pause();
                SetupPauseVideoUIStatus();
            }
            else if (go == mPlayButton.gameObject)
            {
                mPlayer.Play();

                SetupPlayVideoUIStatus();
            }
           
        }

        private void Reload(VideoInfo videoData)
        {
            mPlayer.Release(); 
            isLoading = true;
            SetupLoadingVideoUIStatus(); 
            mPlayer.Reload();
        }
        

        private void Back()
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            SceneManager.LoadScene("BitLobby"); 
        }

        private void SetupPauseVideoUIStatus()
        {
            mPauseButton.gameObject.SetActive(false);
            mPlayButton.gameObject.SetActive(true);
        }

        private void SetupPlayVideoUIStatus()
        {
            mPauseButton.gameObject.SetActive(true);
            mPlayButton.gameObject.SetActive(false);
        }

        private void SetupLoadingVideoUIStatus()
        {
           
        }

        private void SetupReadyVideoUIStatus()
        {
          

        }
        
        private void OnReady()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            mDurationText.text = FormatTime((int)(mPlayer.GetDuration()));
            mPositionText.text = FormatTime((int)(mPlayer.GetPlayPosition()/1000));
            mPlayer.Play();

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            isLoading = false;
        }

        private void OnVideoFirstFrameReady()
        {
            gyroCtrl.Recenter();
            SetupReadyVideoUIStatus();
            SetupPlayVideoUIStatus();
        }

        private string FormatTime(int time)
        {
            int duration = time / 1000;
            int minutes = Mathf.FloorToInt(duration / 60F);
            int seconds = Mathf.FloorToInt(duration - minutes * 60);

            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }

#endregion
    }
}