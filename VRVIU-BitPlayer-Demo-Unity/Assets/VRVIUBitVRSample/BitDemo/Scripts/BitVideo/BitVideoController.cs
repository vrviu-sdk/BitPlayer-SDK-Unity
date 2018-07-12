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
    using UnityEngine.VR;
    using UnityEngine.SceneManagement;
    using Demo.Ui.Video;

    public class BitVideoController : MonoBehaviour
    {
        #region Fields

        public Button backButton;

        public Text nameText;

        public Text bitrateText;

        public Text durationText;

        public Text timeTipText;

        public Text totalDurationText;

        public Button eventButton;

        public Button pauseButton;

        public Button playButton;

        public Button vrButton;

        public Button exitVRButton;

        // public RawImage loadingImage;

        public SeekBarAdjuster seekBarAdjuster;

        public BitVideoPlayer player;

        public CanvasGroup fsOperationCanvas;

        public Canvas vrOperationCanvas;

        public VideoFormater videoFormater;

        public GyroCtrl gyroCtrl;

        public VRCtrl vrCtrl;

        private VideoData data;

        private Account account;
        private Coroutine showOperationCoroutine;

        private bool isLoading = false;

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
            data = BitLobby.videoData;
            account = BitLobby.account;

            player.SetupPlayer(data, account);
            player.SetReplay(-1);
            seekBarAdjuster.player = player;
            isLoading = true;

            vrOperationCanvas.gameObject.SetActive(false);

        }

        private void RegisterEvents()
        {
            backButton.onClick.AddListener(delegate ()
            {
                this.OnClick(backButton.gameObject);
            });

            eventButton.onClick.AddListener(delegate ()
            {
                this.OnClick(eventButton.gameObject);
            });

            pauseButton.onClick.AddListener(delegate ()
            {
                this.OnClick(pauseButton.gameObject);
            });

            playButton.onClick.AddListener(delegate ()
            {
                this.OnClick(playButton.gameObject);
            });

            vrButton.onClick.AddListener(delegate ()
            {
                this.OnClick(vrButton.gameObject);
            });

            exitVRButton.onClick.AddListener(delegate ()
            {
                this.OnClick(exitVRButton.gameObject);
            });

            player.OnReady += OnReady;
            player.OnVideoFirstFrameReady += OnVideoFirstFrameReady;
        }

        private bool CheckOnlineStatus()
        {
            return true;
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
                    //if (XRSettings.enabled)
                    //{
                    //  gyroCtrl.AttachGyro(true);
                    //  StartCoroutine(vrCtrl.SwitchTo2D());
                    //  SetupReadyVideoUIStatus();
                    // SetupPlayVideoUIStatus();
                    //}
                    //else
                    //{
                    Back();
                    //}
                }
            }
#endif

            int duration = 0;
            
            duration = player.GetDuration();
            durationText.text = FormatTime(player.GetPlayPosition());
           

            if (seekBarAdjuster.isDragging)
            {
                int currentDragDuration = (int)(seekBarAdjuster.videoSlider.value * duration);
                timeTipText.text = FormatTime(currentDragDuration);
                timeTipText.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                timeTipText.transform.parent.gameObject.SetActive(false);
            }

            gyroCtrl.enableDrag = !seekBarAdjuster.isDragging;
        }

        private void OnDestroy()
        {
            player.OnReady -= OnReady;
            player.OnVideoFirstFrameReady -= OnVideoFirstFrameReady;
            player.Release();
        }

        private void OnClick(GameObject go)
        {
            if (go == backButton.gameObject)
            {
                Back();
            }
            else if (go == exitVRButton.gameObject)
            {
                gyroCtrl.AttachGyro(true);

                HideVRModeUIStatus();
            }
            else if (go == eventButton.gameObject)
            {
                if (eventButton.GetComponent<EventButtonCtrl>().isDragging)
                {
                    return;
                }
                if (showOperationCoroutine != null)
                {
                    StopCoroutine(showOperationCoroutine);
                    showOperationCoroutine = null;
                    HideFSOperationPanel();
                }
                else
                {
                    showOperationCoroutine = StartCoroutine(ShowOperationPanel());
                }
            }
            else if (go == pauseButton.gameObject)
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
                player.Pause();
                SetupPauseVideoUIStatus();
            }
            else if (go == playButton.gameObject)
            {
                player.Play();

                SetupPlayVideoUIStatus();
            }
            else if (go == vrButton.gameObject)
            {
                gyroCtrl.DetachGyro();
                HideFSOperationPanel();
                SetupVRModeUIStatus();
                if (!XRSettings.enabled)
                {
                    StartCoroutine(vrCtrl.SwitchToVR());
                    showOperationCoroutine = null;
                }
            }
        }

        private void Reload(VideoData videoData)
        {
            player.Release();

            isLoading = true;
            SetupLoadingVideoUIStatus();

            player.Reload();
        }


        private void HideFSOperationPanel()
        {
            fsOperationCanvas.alpha = 0;
            fsOperationCanvas.interactable = false;
            fsOperationCanvas.blocksRaycasts = false;
        }

        private IEnumerator ShowOperationPanel()
        {
            fsOperationCanvas.alpha = 1;

            seekBarAdjuster.isUpdate = true;
            fsOperationCanvas.interactable = true;
            fsOperationCanvas.blocksRaycasts = true;

            seekBarAdjuster.lastOperationTime = Time.time;
            while (Time.time - seekBarAdjuster.lastOperationTime < 3.0 || seekBarAdjuster.isDragging || isLoading)
            {
                yield return null;
            }

            showOperationCoroutine = null;
            HideFSOperationPanel();
        }

        private void Back()
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            SceneManager.LoadScene("BitLobby");
          //  gyroCtrl.DetachGyro();
        }

        private void SetupPauseVideoUIStatus()
        {
            pauseButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
        }

        private void SetupPlayVideoUIStatus()
        {
            pauseButton.gameObject.SetActive(true);
            playButton.gameObject.SetActive(false);
        }

        private void SetupLoadingVideoUIStatus()
        {
            eventButton.interactable = false;
           // loadingImage.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(false);
            vrButton.gameObject.SetActive(false);
            nameText.gameObject.SetActive(false);
            bitrateText.gameObject.SetActive(false);
            durationText.gameObject.SetActive(false);
            totalDurationText.gameObject.SetActive(false);
            seekBarAdjuster.videoSlider.gameObject.SetActive(false);

        }

        private void SetupReadyVideoUIStatus()
        {
            
            eventButton.interactable = true;
            nameText.text = data.name;
            bitrateText.text = data.bitrate;
            //loadingImage.gameObject.SetActive(false);
            vrButton.gameObject.SetActive(true);

            nameText.gameObject.SetActive(true);

            bitrateText.gameObject.SetActive(true);

            durationText.gameObject.SetActive(true);

            totalDurationText.gameObject.SetActive(true);

            seekBarAdjuster.videoSlider.gameObject.SetActive(true);

        }

        private void SetupVRModeUIStatus()
        {
            vrOperationCanvas.gameObject.SetActive(true);
        }

        private void HideVRModeUIStatus()
        {
            vrOperationCanvas.gameObject.SetActive(false);
        }
        

        private void OnReady()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            totalDurationText.text = FormatTime((int)(player.GetDuration()));
            durationText.text = FormatTime((int)(player.GetPlayPosition()/1000));
            player.Play();

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            isLoading = false;
        }

        private void OnVideoFirstFrameReady()
        {
            
            gyroCtrl.Recenter();

            eventButton.interactable = true;

            SetupReadyVideoUIStatus();

            SetupPlayVideoUIStatus();

            HideFSOperationPanel();


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