namespace VRVIU.BitVRPlayer.BitVideo.Silver
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Assertions;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using VRVIU.BitVRPlayer.BitVideo;
    using System.Text;

    public class SilverMediaPlayerCtrl : MonoBehaviour
    {
        Boolean VIDEO_PLAYBACK_ENABLED = true;
        //
        // Define order of array, for language barrier.
        // Order must be synchronized with Orientation.java in MAL library
        //
        private const int ORIENTATION_YAW = 0;
        private const int ORIENTATION_PITCH = 1;
        private const int ORIENTATION_ROLL = 2;

        struct PLAYER_STATE
        {
            public MEDIAPLAYER_STATE eState;
            public int iWidth;
            public int iHeight;
            public float fFrameRate;
            public long lTimestamp;
            public bool bFrameReady;
            public string format;
            public SilverPlayer.SilverPixelFormat pixelFormat;
        };

        public bool m_bEnableAutoRotation = false;
        public float m_AutoRotationSpeed = 1.0f;
        public bool m_bDisplayRotation = false;
        private string m_strFileName;
        private string m_videoInfo;
        private string m_meshUrl;
        private string m_filterVersion;
        public SetViewerFormat m_ViewerFormatScript = null;
        //public GameObject[] m_TargetMaterial = null;

        private Texture2D[] m_VideoTexture = null;
        private PLAYER_STATE m_CurrentState = new PLAYER_STATE();
        private PLAYER_STATE m_PreviousState = new PLAYER_STATE();
        private long m_lTextureTimestamp = 0;
        private long m_iCurrentSeekPosition;
        private float m_fVolume = 1.0f;
        private float m_fSpeed = 1.0f;
        private float m_FrameRate = 1.0f;
        private Mesh[] m_defaultMesh = null;
        private Vector3 m_headOrientation = new Vector3(0, 0, 0);
        private Vector3 m_viewOrientation = new Vector3(0, 0, 0);

        private bool m_bDisableOrientationSetting = false;
        private Material m_yuvMaterial;
        private Material m_nv12Material;

        public bool m_bFullScreen = false;
        //Please use only in FullScreen prefab.

        public VideoCallback.VideoResize OnResize;
        public VideoCallback.VideoReady OnReady;
        public VideoCallback.VideoEnd OnEnd;
        public VideoCallback.VideoError OnVideoError;
        public VideoCallback.VideoFirstFrameReady OnVideoFirstFrameReady;
        float m_debugRotation = -180.0f;

        private bool m_bIsFirstFrameReady;

        private int m_replayCount = 0;
        public Shader mShaderYUV;
        public enum MEDIAPLAYER_ERROR
        {
            MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK = 200,
            MEDIA_ERROR_IO = -1004,
            MEDIA_ERROR_MALFORMED = -1007,
            MEDIA_ERROR_TIMED_OUT = -110,
            MEDIA_ERROR_UNSUPPORTED = -1010,
            MEDIA_ERROR_SERVER_DIED = 100,
            MEDIA_ERROR_UNKNOWN = 1
        }

        public enum MEDIA_SCALE
        {
            SCALE_X_TO_Y = 0,
            SCALE_X_TO_Z = 1,
            SCALE_Y_TO_X = 2,
            SCALE_Y_TO_Z = 3,
            SCALE_Z_TO_X = 4,
            SCALE_Z_TO_Y = 5,
            SCALE_X_TO_Y_2 = 6,
        }


        HashSet<MEDIAPLAYER_STATE> mediaStateIsReadyOrActive = new HashSet<MEDIAPLAYER_STATE>()
    { MEDIAPLAYER_STATE.PLAYING, MEDIAPLAYER_STATE.END, MEDIAPLAYER_STATE.READY, MEDIAPLAYER_STATE.STOPPED };


        bool m_bInitializedMoviePlayback = false;

        public MEDIA_SCALE m_ScaleValue;
        public GameObject[] m_objResize = null;
        public bool m_bLoop = true;
        public bool m_bAutoPlay = true;
        public bool m_bInit = false;
        private bool m_bStop = false;
        private bool m_bCheckFBO = false;
        private GameObject m_DebugObject = null;
        private Text m_DebugTextObject = null;
        private SilverPlayer m_Player = new SilverPlayer();

        //-------------------------------------------------------------------------
        void Awake()
        {
            Debug.Log("SilverMediaPlayerCtrl  Awake");
            m_bInit = true;
            m_bInitializedMoviePlayback = false;
        }

        //-------------------------------------------------------------------------
        // Use this for initialization
        void Start()
        {
            Debug.Log("SilverMediaPlayerCtrl Start");
            m_bInitializedMoviePlayback = false;

            // Enable gyro input, so that if VR is not enabled, we can use tilt
            if (UnityEngine.XR.XRDevice.isPresent == false)
            {
                Input.gyro.enabled = true;
            }

            saveDefaultMesh();
            if (m_bDisplayRotation)
            {
                // Create debug text object and add it to scene
                // Create a debug text object template.
                // Has a canvas, locked to screen space
                // 
                Camera camera = Camera.main;
                if (camera == null)
                {
                    Debug.Log("Missing camera");
                    return;
                }
                // create a canvas and a text element to display the logs
                m_DebugObject = new GameObject("DebugCanvas");
                m_DebugObject.layer = LayerMask.NameToLayer("LeftEye")| LayerMask.NameToLayer("RightEye");
                m_DebugObject.transform.parent = camera.gameObject.transform;
                m_DebugObject.transform.position = Vector3.zero;

                Canvas canvas = m_DebugObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;

                CanvasScaler canvasScaler = m_DebugObject.AddComponent<CanvasScaler>();
                canvasScaler.dynamicPixelsPerUnit = 15;
                canvasScaler.referencePixelsPerUnit = 1;

                GameObject goText = new GameObject("DebugText");
                goText.transform.parent = m_DebugObject.transform;
                goText.layer = m_DebugObject.layer;
                m_DebugTextObject = goText.AddComponent<Text>();
                m_DebugTextObject.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                m_DebugTextObject.fontSize = 17;
                m_DebugTextObject.color = Color.white;
                m_DebugTextObject.text = "";
                RectTransform tr = goText.GetComponent<RectTransform>();
                tr.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                tr.sizeDelta = new Vector2(384, 128);
                tr.position = new Vector3(0, -100, 300);
            }
            Assert.AreNotEqual(mShaderYUV, null);

            m_yuvMaterial = new Material(mShaderYUV);
            Shader nv12_shader = Shader.Find("Unlit/Unlit_NV12");
            Debug.Log("Using shader Unlit_NV12");
            Debug.Assert(nv12_shader != null, "Make sure you have the Unlit/Unlit_NV12 shader in the project");
            m_nv12Material = new Material(nv12_shader);

        }

        //-------------------------------------------------------------------------
        void OnApplicationQuit()
        {
            Debug.Log("SilverMediaPlayerCtrl OnApplicationQuit");
            if (System.IO.Directory.Exists(Application.persistentDataPath + "/Data") == true)
                System.IO.Directory.Delete(Application.persistentDataPath + "/Data", true);
        }


        //-------------------------------------------------------------------------
        void OnDisable()
        {
            if (m_CurrentState.eState == MEDIAPLAYER_STATE.PLAYING)
            {
                Pause();
                m_bPause = true;
            }
        }

        //-------------------------------------------------------------------------
        void OnEnable()
        {
            if (m_bPause)
            {
                Play();
            }
        }

        //-------------------------------------------------------------------------
        private static Quaternion ConvertRotation(Quaternion q)
        {
            return new Quaternion(q.x, q.y, -q.z, -q.w);
        }

        //-------------------------------------------------------------------------
        // Used at startup, save the default mesh's somewhere safe, so when we switch between sequences without re-initializing, we can
        // switch back to the other mesh quickly.
        private void saveDefaultMesh()
        {
            m_defaultMesh = new Mesh[GetTargetMaterial().Length];

            for (int i = 0; i < GetTargetMaterial().Length; i++)
            {
                m_defaultMesh[i] = null;
                if (GetTargetMaterial()[i] != null)
                {
                    MeshFilter meshFilter = GetTargetMaterial()[i].GetComponent<MeshFilter>();
                    if (meshFilter)
                    {
                        m_defaultMesh[i] = meshFilter.mesh;
                    }
                }
            }
        }


        private MeshFilter[] GetTargetMaterial() {
            if (m_ViewerFormatScript == null) {
                return null;
            }
            return m_ViewerFormatScript.GetTargetMaterial() ;
        }
        //-------------------------------------------------------------------------
        // Restore default mesh, if no mesh file was specified.
        private void setDefaultMesh()
        {
            for (int i = 0; i < GetTargetMaterial().Length; i++)
            {
                if (GetTargetMaterial()[i] != null)
                {
                    MeshFilter meshFilter = GetTargetMaterial()[i].GetComponent<MeshFilter>();
                    if (meshFilter)
                    {
                        meshFilter.mesh = m_defaultMesh[i];
                    }
                }
            }
        }

        //-------------------------------------------------------------------------
        private void setMesh(Mesh mesh)
        {
            for (int i = 0; i < GetTargetMaterial().Length; i++)
            {
                if (GetTargetMaterial()[i] != null)
                {
                    MeshFilter meshFilter = GetTargetMaterial()[i].GetComponent<MeshFilter>();
                    if (meshFilter)
                    {
                        meshFilter.mesh = mesh;
                    }
                }
            }
        }

        //-------------------------------------------------------------------------
        // Load new mesh and bind to target materials
        private bool loadMesh(string meshURL)
        {
            // Try to load the mesh as a resource, we will have a copy in the Resource/Meshes folder.
            // To do this, we only need the 'filename' component of the mesh name.
            Mesh newMesh = null;

            string[] parts = meshURL.Split('/');
            if (parts.Length > 0)
            {
                string meshName;
                string[] meshFullName = parts[parts.Length - 1].Split('.');
                meshName = "Meshes/" + meshFullName[0];
                newMesh = Resources.Load(meshName, typeof(Mesh)) as Mesh;
                if (newMesh != null)
                {
                    Debug.Log("Loaded mesh from internal resource " + meshName);
                }
            }

            // If we can't load a built-in resource, then we load the mesh URL from the web.
            if (newMesh == null)
            {
                WWW meshFile = new WWW(meshURL);
                while (!meshFile.isDone && (meshFile.error == null)) ;
                if (string.IsNullOrEmpty(meshFile.error))
                {
                    // Load mesh here
                    newMesh = ObjImporter.ImportMesh(meshFile.text);
                    Debug.Log("New mesh imported from " + meshURL);
                    if (newMesh == null)
                    {
                        Debug.LogError("Unable to parse mesh at URL " + meshURL);
                        return false;
                    }
                }
                else
                {
                    Debug.LogError("Unable to retrieve mesh URL " + meshURL);
                    return false;
                }
            }
            for (int i = 0; i < GetTargetMaterial().Length; i++)
            {
                if (GetTargetMaterial()[i] != null)
                {
                    MeshFilter meshFilter = GetTargetMaterial()[i].GetComponent<MeshFilter>();
                    if (meshFilter == null)
                    {
                        throw new System.Exception("Object did not contain meshFilter");
                    }
                    meshFilter.mesh = newMesh;
                }
            }
            return true;
        }

        //-------------------------------------------------------------------------
        private bool isFrameReady()
        {
            return m_CurrentState.bFrameReady;
        }

        //-------------------------------------------------------------------------
        private void setMaterialOrientation(Vector3 currentOrientation)
        {
            for (int i = 0; i < GetTargetMaterial().Length; i++)
            {
                if (GetTargetMaterial()[i])
                {
                    GetTargetMaterial()[i].transform.localEulerAngles = currentOrientation;
                }
            }
        }

        //-------------------------------------------------------------------------
        private void UpdateState()
        {
            if (m_CurrentState.eState != m_PreviousState.eState)
            {
                m_PreviousState.eState = m_CurrentState.eState;

                switch (m_CurrentState.eState)
                {
                    //-----------------------------
                    case MEDIAPLAYER_STATE.READY:
                        if (m_bPause)
                        {
                            return;
                        }
                        if (OnReady != null)
                        {
                            OnReady();
                        }

                        //
                        //*BW* Move mesh setup to just before starting movie; this makes sure we never see it with the wrong mesh
                        // 

                        Mesh mesh = Call_GetMesh();

                        bool bLoaded = (mesh != null);
                        m_bDisableOrientationSetting = true;
                        if (bLoaded == false)
                        {
                            m_bDisableOrientationSetting = false;
                            setDefaultMesh();
                        }
                        else
                        {
                            setMesh(mesh);
                        }
                        //
                        // Set up stereo/mono views after meshes have been loaded/reset
                        //
                        if (null != m_ViewerFormatScript)
                        {
                            m_ViewerFormatScript.SetUpViewerFormat();
                        }


                        if (m_bAutoPlay)
                        {
                            Call_Play();
                        }

                        SetVolume(m_fVolume);
                        break;
                    //-----------------------------
                    case MEDIAPLAYER_STATE.END:
                        if (OnEnd != null)
                        {
                            OnEnd();
                        }
                        break;
                    //-----------------------------
                    case MEDIAPLAYER_STATE.ERROR:
                        OnError((MEDIAPLAYER_ERROR)Call_GetError(), (MEDIAPLAYER_ERROR)Call_GetErrorExtra());
                        break;
                }
            }
        }


        private void Call_SetUrl(string url)
        {
            m_VideoTexture = null;
            SilverPlayer.SILVER_ERROR Result = m_Player.Initialize();
            m_Player.SetReplay(m_replayCount);
            Result = m_Player.SetVideoUrl(m_strFileName.Trim());
            Call_Prepare();
        }

        private void Call_SetVideoInfo(string videoInfo, int size)
        {
            m_VideoTexture = null;
            SilverPlayer.SILVER_ERROR Result = m_Player.Initialize();
            m_Player.SetReplay(m_replayCount);
            Result = m_Player.SetVideoInfo(videoInfo, size);
            Call_Prepare();
        }
        private void Call_Prepare()
        {

            SilverPlayer.SILVER_ERROR Result = m_Player.Prepare();
        }

        //-------------------------------------------------------------------------
        // This code needs to be in update in order for the display to be updated correctly based on the orientation of the device
        // If placed in PreRender then the orientation based on the gyro is incorrect
        IntPtr m_lastTexId = IntPtr.Zero;
        void Update()
        {
            if (string.IsNullOrEmpty(m_strFileName) && string.IsNullOrEmpty(m_videoInfo))
            {
                return;
            }

            // Any attached exoplayer playback instance takes over, here.
            if (!m_bInitializedMoviePlayback)
            {
                if (!string.IsNullOrEmpty(m_strFileName))
                {
                    String extension = Path.GetExtension(m_strFileName).ToLower();
                    // Any attached exoplayer playback instance takes over, here.
                    Call_SetUrl(m_strFileName);
                    m_bInitializedMoviePlayback = true;
                }
                else if (!string.IsNullOrEmpty(m_videoInfo))
                {
                    int len = Encoding.UTF8.GetByteCount(m_videoInfo);
                    Call_SetVideoInfo(m_videoInfo, len);
                    m_bInitializedMoviePlayback = true;
                }
                UnityEngine.XR.InputTracking.Recenter();
            }
            Call_Update();
            if (VIDEO_PLAYBACK_ENABLED)
            {
                m_CurrentState = Call_GetState();
                if (m_CurrentState.eState == MEDIAPLAYER_STATE.PLAYING || m_bPause)
                {
                    if (m_bCheckFBO == false)
                    {
                        if (m_CurrentState.iWidth <= 0 || m_CurrentState.iHeight <= 0)
                        {
                            return;
                        }

                        Resize();

                        //
                        // Set up stereo/mono views after meshes have been loaded/reset
                        //
                        SetViewerFormat.m_formatType = m_CurrentState.format;
                        m_PreviousState.format = m_CurrentState.format;
                        if (null != m_ViewerFormatScript)
                        {
                            m_ViewerFormatScript.SetUpViewerFormat();
                        }

                        m_FrameRate = m_CurrentState.fFrameRate;

                        m_bCheckFBO = true;
                        if (OnResize != null)
                            OnResize();
                    }
                    else
                    {
                        bool bSizeChanged = m_CurrentState.iWidth != m_PreviousState.iWidth || m_CurrentState.iHeight != m_PreviousState.iHeight;
                        bool bFormatChanged = m_CurrentState.pixelFormat != m_PreviousState.pixelFormat;
                        if (bSizeChanged || bFormatChanged)
                        {
                            m_PreviousState.iWidth = m_CurrentState.iWidth;
                            m_PreviousState.iHeight = m_CurrentState.iHeight;
                            m_PreviousState.pixelFormat = m_CurrentState.pixelFormat;

                            ResizeTexture();
                        }
                        if (m_CurrentState.format != m_PreviousState.format)
                        {
                            SetViewerFormat.m_formatType = m_CurrentState.format;
                            m_PreviousState.format = m_CurrentState.format;
                            if (null != m_ViewerFormatScript)
                            {
                                m_ViewerFormatScript.SetUpViewerFormat();
                            }
                        }
                        IntPtr texId;
                        SilverPlayer.SILVER_ERROR result = m_Player.GetExternalTexture(out texId);
                        Debug.Assert(result == SilverPlayer.SILVER_ERROR.SILVER_SUCCESS);
                        if (texId != m_lastTexId)
                        {
                            if (texId != IntPtr.Zero)
                            {
                                Debug.Log("NV12 setup, external texture id " + texId);
                                m_VideoTexture[0] = Texture2D.CreateExternalTexture(m_CurrentState.iWidth, m_CurrentState.iHeight, TextureFormat.RGBA32, false, false, texId);
                            }
                            else
                            {
                                m_VideoTexture[0] = Texture2D.blackTexture;
                            }
                            m_lastTexId = texId;
                        }
                    }

                    int iFrameTime = 1;
                    if (m_FrameRate > 0.0)
                    {
                        iFrameTime = (int)(1e6 / m_FrameRate);
                    }

                    if (m_bEnableAutoRotation)
                    {
                        if (UnityEngine.XR.XRDevice.isPresent == false)
                        {
                            Input.gyro.enabled = false;
                        }
                        m_ViewerFormatScript.getLeftEyeCarema().transform.rotation = new Quaternion(0.0f, m_debugRotation, 0.0f, 0.0f);
                        // Rotate all the way around. For 40 second rotation, roughly 5 seconds per view switch
                        m_debugRotation += Time.deltaTime * 360.0f / m_AutoRotationSpeed;
                        if (m_debugRotation >= 180.0f)
                        {
                            m_debugRotation -= 360.0f;
                        }
                        m_headOrientation = new Vector3(0.0f, m_debugRotation, 0.0f);
                    }
                    else
                    {
                        if (UnityEngine.XR.XRDevice.isPresent == false)
                        {
#if UNITY_EDITOR || UNITY_STANDALONE
                            m_headOrientation = m_ViewerFormatScript.getLeftEyeCarema().transform.rotation.eulerAngles;
                            m_headOrientation.x = -m_headOrientation.x;             // Invert pitch *BW* not sure why
#else
                        // get orientation from device's gyro input
                       // m_ViewerFormatScript.getLeftEyeCarema().transform.rotation = ConvertRotation(Input.gyro.attitude);
#endif
                        }
                        
                        // Pull head orientation from the camera object
                        m_headOrientation = m_ViewerFormatScript.getLeftEyeCarema().transform.rotation.eulerAngles;
                        if (m_headOrientation.y >= 180)
                        {
                            m_headOrientation.y -= 360;
                        }
                        if (m_headOrientation.x >= 180)
                        {
                            m_headOrientation.x -= 360;
                        }
                        m_headOrientation.x = -m_headOrientation.x; // Invert pitch *BW* not sure why

                        if (m_headOrientation.z >= 180)
                        {
                            m_headOrientation.z -= 360;
                        }
                        m_headOrientation.z = -m_headOrientation.z;
                    }
                    
                    Call_SetOrientation(m_headOrientation);
                    bool rotateMesh = false;
                    if (isFrameReady())
                    {
                       
                        // UpdateVideoTexture returns new timestamp decoded, so update the state structure accordingly
                        m_lTextureTimestamp = Call_UpdateVideoTexture(out m_viewOrientation, out rotateMesh);
                        m_viewOrientation.x = -m_viewOrientation.x;

                        if (rotateMesh)
                        {
                            if (m_viewOrientation.x == -90)
                            {
                                m_viewOrientation.x = -m_viewOrientation.x;
                            }
                            else if(m_viewOrientation.x == 90) {
                                m_viewOrientation.x = -m_viewOrientation.x;
                            }
                            setMaterialOrientation(m_viewOrientation);
                        }
                        else
                        {
                            // If we're using a view adaptive movie, that doesn't have a mesh, then we want to make sure 
                            // it's mesh stays on forward orientation.
                            //*NOTE* The default mesh does not orient at 0,0,0 for forward direction being center of the image.
                            // So we have to force it to -90 degrees. THIS IS UNEXPECTED.
                            Vector3 zeroOrientation = new Vector3(0, 0, 0);
                            setMaterialOrientation(zeroOrientation);
                        }
                       
                        m_iCurrentSeekPosition = Call_GetSeekPosition();
                    }
                    if (m_DebugTextObject != null)
                    {
                        m_DebugObject.transform.rotation = Camera.main.transform.rotation;
                        m_DebugTextObject.text = "Head orientation:" + m_headOrientation + "\n" +
                                                 "View orientation:" + m_viewOrientation + "\n" +
                                                 "Mesh orientation:" + GetTargetMaterial()[0].transform.localEulerAngles + "\n" +
                                                 "Mesh rotate     :"+ rotateMesh+
                                                 string.Format("State Timestamp: {0,9:##0.000000}\n", (double)m_CurrentState.lTimestamp / 1e6) +
                                                 string.Format("Tex   Timestamp: {0,9:##0.000000}\n", (double)m_lTextureTimestamp / 1e6);
                    }
                }
                UpdateState();
            }
        }

        //-------------------------------------------------------------------------
        private void ResizeTexture()
        {
            if (m_VideoTexture != null)
            {
                foreach (Texture texture in m_VideoTexture)
                {
                    if (texture != Texture2D.blackTexture)
                    {
                        Destroy(texture);
                    }
                }
            }

            switch (m_CurrentState.pixelFormat)
            {
                case SilverPlayer.SilverPixelFormat.PIX_FMT_YUV420P:
                    {
                        m_VideoTexture = new Texture2D[3];
                        m_VideoTexture[0] = new Texture2D(m_CurrentState.iWidth, m_CurrentState.iHeight, TextureFormat.R8, false);
                        m_VideoTexture[1] = new Texture2D(m_CurrentState.iWidth / 2, m_CurrentState.iHeight / 2, TextureFormat.R8, false);
                        m_VideoTexture[2] = new Texture2D(m_CurrentState.iWidth / 2, m_CurrentState.iHeight / 2, TextureFormat.R8, false);
                        m_VideoTexture[0].Apply();
                        m_VideoTexture[1].Apply();
                        m_VideoTexture[2].Apply();
                        break;
                    }
                case SilverPlayer.SilverPixelFormat.PIX_FMT_NV12:
                    {
#if UNITY_ANDROID && !UNITY_EDITOR
                    m_VideoTexture = new Texture2D[1];
                    IntPtr texId;
                    m_Player.GetExternalTexture( out texId );
                    Debug.Log("NV12 setup, external texture id " + texId);
                    m_VideoTexture[0] = Texture2D.CreateExternalTexture(m_CurrentState.iWidth, m_CurrentState.iHeight, TextureFormat.RGBA32, false, false, texId );
#else
                        m_VideoTexture = new Texture2D[2];
                        m_VideoTexture[0] = new Texture2D(m_CurrentState.iWidth, m_CurrentState.iHeight, TextureFormat.R8, false);
                        m_VideoTexture[1] = new Texture2D(m_CurrentState.iWidth / 2, m_CurrentState.iHeight / 2, TextureFormat.RG16, false);
                        m_VideoTexture[0].Apply();
                        m_VideoTexture[1].Apply();
                        Debug.Log("NV12 textures, Y:" + m_VideoTexture[0].GetNativeTexturePtr() + ", UV:" + m_VideoTexture[1].GetNativeTexturePtr());
#endif
                        break;
                    }
                case SilverPlayer.SilverPixelFormat.PIX_FMT_RGBA:
                    {
                        m_VideoTexture = new Texture2D[1];
                        m_VideoTexture[0] = new Texture2D(m_CurrentState.iWidth, m_CurrentState.iHeight, TextureFormat.RGBA32, false);
                        m_VideoTexture[0].Apply();
                        break;
                    }

                // For an unknown format, display at least *something*
                default:
                    m_VideoTexture = new Texture2D[1];
                    m_VideoTexture[0] = Texture2D.blackTexture;
                    break;
            }
            foreach (Texture2D texture in m_VideoTexture)
            {
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Bilinear;
            }
        }

        //-------------------------------------------------------------------------
        private void Resize()
        {
            if (m_CurrentState.eState != MEDIAPLAYER_STATE.PLAYING)
                return;

            if (m_CurrentState.iWidth <= 0 || m_CurrentState.iHeight <= 0)
            {
                return;
            }

            if (m_objResize != null)
            {
                int iScreenWidth = Screen.width;
                int iScreenHeight = Screen.height;

                float fRatioScreen = (float)iScreenHeight / (float)iScreenWidth;
                int iWidth = m_CurrentState.iWidth;
                int iHeight = m_CurrentState.iHeight;

                float fRatio = (float)iHeight / (float)iWidth;
                float fRatioResult = fRatioScreen / fRatio;

                for (int i = 0; i < m_objResize.Length; i++)
                {
                    if (m_objResize[i] == null)
                        continue;

                    if (m_bFullScreen)
                    {
                        m_objResize[i].transform.localScale = new Vector3(20.0f / fRatioScreen, 20.0f / fRatioScreen, 1.0f);
                        if (fRatio < 1.0f)
                        {
                            if (fRatioScreen < 1.0f)
                            {
                                if (fRatio > fRatioScreen)
                                {
                                    m_objResize[i].transform.localScale *= fRatioResult;
                                }
                            }

                            m_ScaleValue = MEDIA_SCALE.SCALE_X_TO_Y;
                        }
                        else
                        {
                            if (fRatioScreen > 1.0f)
                            {
                                if (fRatio >= fRatioScreen)
                                {
                                    m_objResize[i].transform.localScale *= fRatioResult;
                                }
                            }

                            m_ScaleValue = MEDIA_SCALE.SCALE_X_TO_Y;
                        }
                    }


                    if (m_ScaleValue == MEDIA_SCALE.SCALE_X_TO_Y)
                    {
                        m_objResize[i].transform.localScale
                            = new Vector3(m_objResize[i].transform.localScale.x
                                          , m_objResize[i].transform.localScale.x * fRatio
                                          , m_objResize[i].transform.localScale.z);
                    }
                    else if (m_ScaleValue == MEDIA_SCALE.SCALE_X_TO_Y_2)
                    {
                        m_objResize[i].transform.localScale
                            = new Vector3(m_objResize[i].transform.localScale.x
                                          , m_objResize[i].transform.localScale.x * fRatio / 2.0f
                                          , m_objResize[i].transform.localScale.z);
                    }
                    else if (m_ScaleValue == MEDIA_SCALE.SCALE_X_TO_Z)
                    {
                        m_objResize[i].transform.localScale
                            = new Vector3(m_objResize[i].transform.localScale.x
                                          , m_objResize[i].transform.localScale.y
                                          , m_objResize[i].transform.localScale.x * fRatio);
                    }
                    else if (m_ScaleValue == MEDIA_SCALE.SCALE_Y_TO_X)
                    {
                        m_objResize[i].transform.localScale
                            = new Vector3(m_objResize[i].transform.localScale.y / fRatio
                                          , m_objResize[i].transform.localScale.y
                                          , m_objResize[i].transform.localScale.z);
                    }
                    else if (m_ScaleValue == MEDIA_SCALE.SCALE_Y_TO_Z)
                    {
                        m_objResize[i].transform.localScale
                            = new Vector3(m_objResize[i].transform.localScale.x
                                          , m_objResize[i].transform.localScale.y
                                          , m_objResize[i].transform.localScale.y / fRatio);
                    }
                    else if (m_ScaleValue == MEDIA_SCALE.SCALE_Z_TO_X)
                    {
                        m_objResize[i].transform.localScale
                            = new Vector3(m_objResize[i].transform.localScale.z * fRatio
                                          , m_objResize[i].transform.localScale.y
                                          , m_objResize[i].transform.localScale.z);
                    }
                    else if (m_ScaleValue == MEDIA_SCALE.SCALE_Z_TO_Y)
                    {
                        m_objResize[i].transform.localScale
                            = new Vector3(m_objResize[i].transform.localScale.x
                                          , m_objResize[i].transform.localScale.z * fRatio
                                          , m_objResize[i].transform.localScale.z);
                    }
                    else
                    {
                        m_objResize[i].transform.localScale
                            = new Vector3(m_objResize[i].transform.localScale.x, m_objResize[i].transform.localScale.y, m_objResize[i].transform.localScale.z);
                    }
                }

            }
        }


        //-------------------------------------------------------------------------
        //The error code is the following sites related documents.
        //http://developer.android.com/reference/android/media/MediaPlayer.OnErrorListener.html
        void OnError(MEDIAPLAYER_ERROR iCode, MEDIAPLAYER_ERROR iCodeExtra)
        {
            //Debug.Assert(false, "SilverMediaPlayerCtrl: error " + iCode+"("+Call_GetErrorString((int)iCode)+")", AssertLevel.Level_2);   // Display VR Dialog on headset.

            if (OnVideoError != null)
            {
                OnVideoError((int)iCode, (int)iCodeExtra);
            }
        }


        //-------------------------------------------------------------------------
        void OnDestroy()
        {
            Debug.Log("SilverMediaPlayerCtrl OnDestroy");
            if (m_ViewerFormatScript != null) {
                m_ViewerFormatScript.Release();
            }
            Call_UnLoad();
        }

        bool m_bPause = false;

        void OnApplicationPause(bool bPause)
        {
            // if movie playback not initialized then don't attempt to pause media
            if (!m_bInitializedMoviePlayback)
                return;

            if (bPause)
            {
                m_bPause = true;
                Call_Pause();
            }
            else
            {
                Call_Play();
                m_bPause = false;
            }
        }

        //-------------------------------------------------------------------------
        public MEDIAPLAYER_STATE GetCurrentState()
        {
            m_CurrentState = Call_GetState();
            return m_CurrentState.eState;
        }
        //-------------------------------------------------------------------------
        public bool isPaused()
        {
            return m_bPause;
        }
        //-------------------------------------------------------------------------
        public Texture2D GetVideoTexture()
        {
            return m_VideoTexture[0];
        }

        //-------------------------------------------------------------------------
        public void Play()
        {
            Call_Play();
        }

        //-------------------------------------------------------------------------
        public void Stop()
        {
            if (m_CurrentState.eState == MEDIAPLAYER_STATE.PLAYING)
            {
                Call_Pause();
                m_bStop = true;
                m_CurrentState.eState = MEDIAPLAYER_STATE.STOPPED;
                m_iCurrentSeekPosition = 0;
            }
        }

        //-------------------------------------------------------------------------
        public void Pause()
        {
            if (m_CurrentState.eState == MEDIAPLAYER_STATE.PLAYING)
            {
                Call_Pause();
                m_bPause = true;
            }
        }

        //-------------------------------------------------------------------------
        // The meshUrl should normally be null; so we don't normally load it. This
        // is a debug override of the default mesh
        public void setUrl(string strFileName, string meshUrl, string filterVersion)
        {
            m_meshUrl = meshUrl;
            m_filterVersion = filterVersion;
            setUrl(strFileName);
        }

        //-------------------------------------------------------------------------
        public void setUrl(string strFileName)
        {
            m_strFileName = strFileName;

            String extension = Path.GetExtension(m_strFileName).ToLower();
            if (extension == ".flv" || extension == ".mp4")
            {
                //m_ExoPlayer = gameObject.AddComponent<VRVIU.Silver180.MediaPlayerCtrl>();
                //m_ExoPlayer.m_TargetMaterial = m_TargetMaterial;
                //m_ExoPlayer.Load(m_strFileName, m_meshUrl, m_filterVersion);
                return;
            }

            if (m_CurrentState.eState != MEDIAPLAYER_STATE.NOT_READY)
                UnLoad();

            m_bIsFirstFrameReady = false;

            m_bInitializedMoviePlayback = false;
            m_bCheckFBO = false;

            if (m_bInit == false)
                return;

            m_CurrentState.eState = MEDIAPLAYER_STATE.NOT_READY;
        }

        //-------------------------------------------------------------------------
        public void setVideoInfo(string videoInfo)
        {
            m_videoInfo = videoInfo;

            if (m_CurrentState.eState != MEDIAPLAYER_STATE.NOT_READY)
                UnLoad();

            m_bIsFirstFrameReady = false;

            m_bInitializedMoviePlayback = false;
            m_bCheckFBO = false;

            if (m_bInit == false)
                return;

            m_CurrentState.eState = MEDIAPLAYER_STATE.NOT_READY;
        }

        //-------------------------------------------------------------------------
        public void SetVolume(float fVolume)
        {
            if (mediaStateIsReadyOrActive.Contains(m_CurrentState.eState))
            {
                m_fVolume = fVolume;
                Call_SetVolume(fVolume);
            }
        }

        //-------------------------------------------------------------------------
        // return milliseconds
        public long GetSeekPosition()
        {
            switch (m_CurrentState.eState)
            {
                //-----------------------------
                case MEDIAPLAYER_STATE.PLAYING:
                case MEDIAPLAYER_STATE.END:
                    return m_iCurrentSeekPosition;
            }
            return 0;
        }

        //-------------------------------------------------------------------------
        // seek to position
        public void SeekTo(long lSeek)
        {
            if (mediaStateIsReadyOrActive.Contains(m_CurrentState.eState))
            {
                Call_SetSeekPositionUs(lSeek);
            }
        }

        //-------------------------------------------------------------------------
        /// <summary>
        /// Sets the speed.
        /// Experimental API( PC&iOS support, support from Android version 6.0 or later)
        /// </summary>
        /// <param name="fSpeed">video playback speed.</param>
        public void SetSpeed(float fSpeed)
        {
            if (mediaStateIsReadyOrActive.Contains(m_CurrentState.eState))
            {
                m_fSpeed = fSpeed;
                Call_SetSpeed(m_fSpeed);
            }
        }

        public void SetRePlay(int count) {
            Call_SetReplay(count);
        }

        //-------------------------------------------------------------------------
        //Gets the duration of the file.
        //Returns
        //the duration in MICROSECONDS, if no duration is available (for example, if streaming live content), -1 is returned.
        public long GetDuration()
        {
            if (mediaStateIsReadyOrActive.Contains(m_CurrentState.eState))
            {
                return Call_GetDuration();
            }
            else
            {
                return 0;
            }
        }

        //-------------------------------------------------------------------------
        public float GetSeekBarValue()
        {
            if (mediaStateIsReadyOrActive.Contains(m_CurrentState.eState))
            {
                if (GetDuration() == 0)
                {
                    return 0;
                }

                return (float)GetSeekPosition() / (float)GetDuration();// duration is 0 ?
            }
            else
            {
                return 0;
            }
        }

        //-------------------------------------------------------------------------
        public void SetSeekBarValue(float fValue)
        {
            if (mediaStateIsReadyOrActive.Contains(m_CurrentState.eState))
            {
                if (GetDuration() == 0)
                {
                    return;
                }

                SeekTo((int)((float)GetDuration() * fValue));
            }
            else
            {
                return;
            }
        }


        //-------------------------------------------------------------------------
        //Only Android support.
        //Get update status in buffering a media stream received through progressive HTTP download.
        //The received buffering percentage indicates how much of the content has been buffered or played.
        //For example a buffering update of 80 percent when half the content has already been played indicates that the next 30 percent of the content to play has been buffered.
        //the percentage (0-100) of the content that has been buffered or played thus far
        public int GetCurrentSeekPercent()
        {
            switch (m_CurrentState.eState)
            {
                //-----------------------------
                case MEDIAPLAYER_STATE.PLAYING:
                case MEDIAPLAYER_STATE.END:
                case MEDIAPLAYER_STATE.READY:
                    return Call_GetCurrentSeekPercent();
            }
            return 0;
        }

        //-------------------------------------------------------------------------
        public void UnLoad()
        {
            Debug.Log("SilverMediaPlayerCtrl UnLoad");
            m_bCheckFBO = false;
            Call_UnLoad();
            m_CurrentState.eState = MEDIAPLAYER_STATE.NOT_READY;
        }

        private void Call_UnLoad()
        {
            m_Player.Terminate();
        }

        private bool Call_Load(string strFileName, int iSeek)
        {
            return true;
        }

        private long Call_UpdateVideoTexture(out Vector3 orientation, out bool rotateMesh)
        {
            long lTimestamp = 0;
            GL.InvalidateState();

            rotateMesh = false;
            IntPtr[] textureData = null;
            if (VIDEO_PLAYBACK_ENABLED)
            {
                textureData = Call_GetVideoTexture(out lTimestamp, out orientation, out rotateMesh);
            }
            else
            {
                orientation = Vector3.zero;
            }

            if (textureData != null && m_VideoTexture != null)
            {
                switch (m_CurrentState.pixelFormat)
                {
                    case SilverPlayer.SilverPixelFormat.PIX_FMT_YUV420P:
                        {
                            int szY = m_CurrentState.iWidth * m_CurrentState.iHeight;
                            int szU = szY / 4;
                            int szV = szU;

                            m_VideoTexture[0].LoadRawTextureData(textureData[0], szY);
                            m_VideoTexture[1].LoadRawTextureData(textureData[1], szU);
                            m_VideoTexture[2].LoadRawTextureData(textureData[2], szV);
                            m_VideoTexture[0].Apply();
                            m_VideoTexture[1].Apply();
                            m_VideoTexture[2].Apply();
                            break;
                        }
                    case SilverPlayer.SilverPixelFormat.PIX_FMT_NV12:
                        {
#if !UNITY_ANDROID || UNITY_EDITOR
                            int szY = m_CurrentState.iWidth * m_CurrentState.iHeight;
                            int szUV = szY / 2;

                            m_VideoTexture[0].LoadRawTextureData(textureData[0], szY);
                            m_VideoTexture[1].LoadRawTextureData(textureData[1], szUV);
                            m_VideoTexture[0].Apply();
                            m_VideoTexture[1].Apply();
#endif
                            break;
                        }
                    case SilverPlayer.SilverPixelFormat.PIX_FMT_RGBA:
                        {
                            int szTexture = m_CurrentState.iWidth * m_CurrentState.iHeight * 4;
                            m_VideoTexture[0].LoadRawTextureData(textureData[0], szTexture);
                            m_VideoTexture[0].Apply();
                            break;
                        }

                    default:
                        break;
                }

                bool bMaterialChanged = false;
                for (int i = 0; i < GetTargetMaterial().Length; i++)
                {
                    if (GetTargetMaterial()[i])
                    {
                        MeshRenderer Renderer = GetTargetMaterial()[i].GetComponent<MeshRenderer>();
                        if (Renderer != null)
                        {
                            if (Renderer.material.mainTexture != m_VideoTexture[0])
                            {
                                Renderer.material.mainTexture = m_VideoTexture[0];
                            }
                            switch (m_CurrentState.pixelFormat)
                            {
                                case SilverPlayer.SilverPixelFormat.PIX_FMT_YUV420P:
                                    {
                                        if (Renderer.material.shader.name != m_yuvMaterial.shader.name)
                                        {
                                            Renderer.material = m_yuvMaterial;
                                            Debug.Log("Setup material YUV");
                                            bMaterialChanged = true;
                                        }
                                        if (Renderer.material.GetTexture("_Y") != m_VideoTexture[0]) Renderer.material.SetTexture("_Y", m_VideoTexture[0]);
                                        if (Renderer.material.GetTexture("_U") != m_VideoTexture[1]) Renderer.material.SetTexture("_U", m_VideoTexture[1]);
                                        if (Renderer.material.GetTexture("_V") != m_VideoTexture[2]) Renderer.material.SetTexture("_V", m_VideoTexture[2]);
                                    }
                                    break;
                                case SilverPlayer.SilverPixelFormat.PIX_FMT_NV12:
                                    {
                                        if (Renderer.material.shader.name != m_nv12Material.shader.name)
                                        {
                                            Renderer.material = m_nv12Material;
                                            Debug.Log("Setup material NV12");
                                            bMaterialChanged = true;
                                        }
#if !UNITY_ANDROID || UNITY_EDITOR
                                        if (Renderer.material.GetTexture("_Y") != m_VideoTexture[0]) Renderer.material.SetTexture("_Y", m_VideoTexture[0]);
                                        if (Renderer.material.GetTexture("_UV") != m_VideoTexture[1]) Renderer.material.SetTexture("_UV", m_VideoTexture[1]);
#endif
                                    }
                                    break;
                                default:
                                    Debug.LogError("Error: Unhandled pixelFormat " + m_CurrentState.pixelFormat);
                                    break;
                            }
                        }

                        RawImage Image = GetTargetMaterial()[i].GetComponent<RawImage>();
                        if (Image != null)
                        {
                            if (Image.texture != m_VideoTexture[0])
                            {
                                Image.texture = m_VideoTexture[0];
                            }
                        }
                    }
                }

                //
                // Set up stereo/mono views if materials were changed
                //
                if (bMaterialChanged && m_ViewerFormatScript)
                {
                    m_ViewerFormatScript.SetUpViewerFormat();
                }
            }

            if (!m_bIsFirstFrameReady)
            {
                m_bIsFirstFrameReady = true;
                if (OnVideoFirstFrameReady != null)
                {
                    OnVideoFirstFrameReady();
                    OnVideoFirstFrameReady = null;
                }
            }
            return lTimestamp;
        }

        private void Call_SetVolume(float fVolume)
        {
            m_Player.SetVolume(fVolume);
        }

        private void Call_SetSeekPositionUs(long lSeek)
        {
            m_Player.SeekTo(lSeek);
        }

        private void Call_SetReplay(int count) {
            m_Player.SetReplay(count);
        }
        private long Call_GetSeekPosition()
        {
            SilverPlayer.SILVER_ERROR Result;
            long pos;
            Result = m_Player.GetCurrentSeekPosition(out pos);
            return pos;
        }

        private void Call_Play()
        {
            m_Player.Play();
        }

        private IntPtr[] Call_GetVideoTexture(out Int64 lTimestamp, out Vector3 orientation, out bool rotateMesh )
        {
            IntPtr[] texture;
            m_Player.GetTexture(out texture, out lTimestamp, out orientation, out rotateMesh);
            return texture;
        }

        private void Call_Stop()
        {
            m_Player.Stop();
        }

        private void Call_RePlay()
        {
            m_Player.Play();
        }

        private void Call_Pause()
        {
            m_Player.Pause();
        }

        private Mesh Call_GetMesh()
        {
            SilverPlayer.SILVER_ERROR Result;
            Mesh mesh;
            Result = m_Player.GetMesh(out mesh);
            if (Result != SilverPlayer.SILVER_ERROR.SILVER_SUCCESS)
            {
                return null;
            }
            return mesh;
        }


        private int Call_GetError()
        {
            return (int)m_Player.GetError();
        }

        private string Call_GetErrorString(int error)
        {
            return m_Player.GetErrorString((SilverPlayer.SILVER_ERROR)error);
        }

        private int Call_GetErrorExtra()
        {
            return 0;
        }

        private long Call_GetDuration()
        {
            long duration;
            m_Player.GetDuration(out duration);
            return duration;
        }

        private int Call_GetCurrentSeekPercent()
        {
            return -1;
        }

        private void Call_SetSpeed(float fSpeed)
        {
            m_Player.SetSpeed(fSpeed);
        }

        private int Call_Update()
        {
            SilverPlayer.SILVER_ERROR Result;
            double startTime = Time.realtimeSinceStartup;
            Result = m_Player.Update();
            if (Result != SilverPlayer.SILVER_ERROR.SILVER_SUCCESS)
            {
                Debug.LogError("Update returned error " + Result);
            }
            double elapsedTime = (Time.realtimeSinceStartup - startTime) * 1e3;
            if (elapsedTime > 10)
            {
                Debug.LogWarning("Update took a long time " + elapsedTime + "mS");
            }
            return (int)Result;
        }

        private PLAYER_STATE Call_GetState()
        {
            SilverPlayer.SILVER_ERROR Result;
            SilverPlayer.SilverVideoInfo VideoInfo;
            double startTime = Time.realtimeSinceStartup;

            PLAYER_STATE ResultingState = new PLAYER_STATE();
            ResultingState.eState = MEDIAPLAYER_STATE.NOT_READY;
            //
            // Get Silver status
            //
            Result = m_Player.GetVideoInfo(out VideoInfo);
            if (Result != SilverPlayer.SILVER_ERROR.SILVER_SUCCESS)
            {
                // Any error, return not ready
                return ResultingState;
            }

            // Convert state
            switch (VideoInfo.state)
            {
                case SilverPlayer.SILVER_STATE.SILVER_STATE_UNKNOWN: ResultingState.eState = MEDIAPLAYER_STATE.NOT_READY; break;
                case SilverPlayer.SILVER_STATE.SILVER_STATE_IDLE: ResultingState.eState = MEDIAPLAYER_STATE.INITIALIZED; break;
                case SilverPlayer.SILVER_STATE.SILVER_STATE_PREPARING: ResultingState.eState = MEDIAPLAYER_STATE.PREPARING; break;
                case SilverPlayer.SILVER_STATE.SILVER_STATE_READY: ResultingState.eState = MEDIAPLAYER_STATE.READY; break;
                case SilverPlayer.SILVER_STATE.SILVER_STATE_BUFFERING: ResultingState.eState = MEDIAPLAYER_STATE.BUFFERING; break;
                case SilverPlayer.SILVER_STATE.SILVER_STATE_PLAYING: ResultingState.eState = MEDIAPLAYER_STATE.PLAYING; break;
                case SilverPlayer.SILVER_STATE.SILVER_STATE_PAUSE:  ResultingState.eState = MEDIAPLAYER_STATE.PAUSED;break;
                case SilverPlayer.SILVER_STATE.SILVER_STATE_ERROR: ResultingState.eState = MEDIAPLAYER_STATE.ERROR; break;
                case SilverPlayer.SILVER_STATE.SILVER_STATE_END: ResultingState.eState = MEDIAPLAYER_STATE.END; break;
                default: ResultingState.eState = MEDIAPLAYER_STATE.NOT_READY; break;
            }
            switch (VideoInfo.sourceFormat)
            {
                case SilverPlayer.SilverSourceFormat.eSilverSource_Mono: ResultingState.format = "mono"; break;
                case SilverPlayer.SilverSourceFormat.eSilverSource_Stereo_L_R: ResultingState.format = "stereo_left_right"; break;
                case SilverPlayer.SilverSourceFormat.eSilverSource_Stereo_T_B: ResultingState.format = "stereo_top_bottom"; break;
                case SilverPlayer.SilverSourceFormat.eSilverSource_Stereo_R_L: ResultingState.format = "stereo_right_left"; break;
                case SilverPlayer.SilverSourceFormat.eSilverSource_Stereo_B_T: ResultingState.format = "stereo_bottom_top"; break;
                default: ResultingState.format = "mono"; break;
            }
            if (ResultingState.eState < MEDIAPLAYER_STATE.READY)
            {
                return ResultingState;
            }
            // Convert width & height
            ResultingState.iWidth = (int)VideoInfo.iWidth;
            ResultingState.iHeight = (int)VideoInfo.iHeight;
            ResultingState.pixelFormat = VideoInfo.pixelFormat;

            // Get timebase numerator and denominator; need to do a check here, since it's possible no numerator denominator was set
            long numerator = 1001;
            long denominator = 30000;
            if (VideoInfo.iTimebaseNumerator > 0)
            {
                numerator = VideoInfo.iTimebaseNumerator;
            }
            else
            {
                //Debug.LogError("silverGetVideoInfo() returned zero numerator, defaulting to 1001 (29.97fps).");
            }
            if (VideoInfo.iTimebaseDenominator > 0)
            {
                denominator = VideoInfo.iTimebaseDenominator;
            }
            else
            {
                //Debug.LogError("silverGetVideoInfo() returned zero denominator, defaulting to 30,000 (29.97fps).");
            }

            ResultingState.fFrameRate = (float)denominator / (float)numerator;
            ResultingState.lTimestamp = ((long)VideoInfo.lPlayPositionTicks * 1000000) / denominator;
            ResultingState.bFrameReady = VideoInfo.bFrameReady;
            double elapsedTime = (Time.realtimeSinceStartup - startTime) * 1e3;
            if (elapsedTime > 5)
            {
                Debug.LogWarning("GetState took a long time " + elapsedTime + "mS");
            }
            return ResultingState;
        }

        private void Call_SetOrientation(Vector3 Orientation)
        {
            m_Player.SetHeadOrientation(Orientation);
        }
        private Vector3 Call_GetOrientation(long lTimestamp)
        {
            SilverPlayer.SILVER_ERROR Result;

            Vector3 orientation = Vector3.zero;

            Result = m_Player.GetOrientation(out orientation, lTimestamp);
            return orientation;
        }
    }

}