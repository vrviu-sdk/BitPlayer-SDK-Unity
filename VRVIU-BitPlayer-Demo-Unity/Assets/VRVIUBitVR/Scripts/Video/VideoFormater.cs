/// <summary>
/// A video format controller, help switch mono or stereo format. notice that now only use left eye camera to do rendering under mono mode.
/// </summary>
namespace VRVIU.BitVrPlayer.Video
{
    using UnityEngine;
    using  VRVIU.BitVrPlayer.Silver;
    [DisallowMultipleComponent]
    public sealed class VideoFormater : MonoBehaviour
    {
        #region Constant variables
        private static readonly string LEFT_EYE_CAMERA_NAME = "LeftEye";

        private static readonly string RIGHT_EYE_CAMERA_NAME = "RightEye";

        private static readonly string LEFT_EYE_LAYER_NAME = "LeftEye";

        private static readonly string RIGHT_EYE_LAYER_NAME = "RightEye";
        #endregion

        #region Public variables
        /// <summary>
        /// Left eye handle.
        /// </summary>
        public GameObject leftSide = null;
        /// <summary>
        /// Right eye handle.
        /// </summary>
        public GameObject rightSide = null;

        public Mesh erp180Mesh = null;

        public Mesh erp360Mesh = null;

        public Mesh fisheyeMesh = null;
        #endregion

        #region Private variables
        private BitVideoPlayer player = null;

        private Mesh defaultMesh = null;

        private MeshFilter leftSideMeshFilter = null;

        private MeshFilter rightSideMeshFilter = null;

        private MeshRenderer leftSideMeshRenderer = null;

        private MeshRenderer rightSideMeshRenderer = null;

        /// <summary>
        /// Current video format.
        /// </summary>
        [SerializeField]
        [Tooltip("Default video playback format.")]
        private VideoFormat _format = VideoFormat.OPT_ERP_360;

        public VideoFormat format
        {
            get
            {
                return _format;
            }

            set
            {
                if (_format != value)
                {
                    _format = value;

                    Switch();
                }
               
            }
        }        

        /// <summary>
        /// Left eye camera component
        /// </summary>
        private Camera leftEyeCamera = null;

        /// <summary>
        /// Right eye camera component
        /// </summary>
        private Camera rightEyeCamera = null;

        private GameObject leftEyeCameraObject = null;
        private GameObject rightEyeCameraObject = null;
        #endregion
		 
        // Use this for initialization
        void Start()
        {
            if (leftSide != null)
            {
                leftSideMeshFilter = leftSide.GetComponent<MeshFilter>();
                leftSideMeshRenderer = leftSide.GetComponent<MeshRenderer>();
            }

            if (rightSide != null)
			{
				
				
                rightSideMeshFilter = rightSide.GetComponent<MeshFilter>();
                rightSideMeshRenderer = rightSide.GetComponent<MeshRenderer>();
            }

            BindPlayer();
            UnityEngine.XR.InputTracking.Recenter();

            // Get left eye camera component
            GameObject cameraObject = GameObject.Find(LEFT_EYE_CAMERA_NAME);

            if (cameraObject != null)
            {
                leftEyeCameraObject = cameraObject;
                leftEyeCamera = cameraObject.GetComponent<Camera>();
            }

            // Get right eye camera component
            cameraObject = GameObject.Find(RIGHT_EYE_CAMERA_NAME);

            if (cameraObject != null)
            {
                rightEyeCameraObject = cameraObject;
                rightEyeCamera = cameraObject.GetComponent<Camera>();
            }


            if (leftEyeCamera == null || rightEyeCamera == null)
            {
                Debug.LogWarning("Missing left or right eye camera in your scene, which will cause stereo unavailable.");
            }

            Switch();

        }

		public void init(){
			
		}

        private void OnDestroy()
        {
            UnbindPlayer();

            UnloadMesh();

            UnloadMaterial();
        }

        /// <summary>
        /// Switch to any supported video format.
        /// </summary>
        public void Switch(VideoFormat format)
        {
            if (this.format != format)
            {
                this.format = format;
            }
           
        }

        /// <summary>
        /// Switch video format according to current viedoFormat.
        /// </summary>
        private void Switch()
        {
            #region Ajust camera settings.
            int LEFT_EYE_LAYER_MASK  = (1 << LayerMask.NameToLayer(LEFT_EYE_LAYER_NAME))  | (1 << 0);
            int RIGHT_EYE_LAYER_MASK = (1 << LayerMask.NameToLayer(RIGHT_EYE_LAYER_NAME)) | (1 << 0);

            // Monoscopic
            if (format == VideoFormat.OPT_FLAT || format == VideoFormat.OPT_ERP_180 || format == VideoFormat.OPT_ERP_360 || format == VideoFormat.OPT_FISHEYE)
            {
                if (leftEyeCamera != null)
                {
                    // Enable left eye camera component
                    leftEyeCamera.gameObject.SetActive(true);

                    // Set left camera component to display both eyes
                    leftEyeCamera.stereoTargetEye = StereoTargetEyeMask.Both;

                    // Only need to show left eye layer display content (since we will display same image on both eyes)
                    leftEyeCamera.cullingMask = LEFT_EYE_LAYER_MASK;

                    leftSide.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", Vector2.one);
                    leftSide.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", Vector2.zero);
                }

                if (rightEyeCamera != null)
                {
                    // Disable right eye camera component
                    rightEyeCamera.gameObject.SetActive(false);
                }

            }
            // Stereoscopic
            else
            {
                if (leftEyeCamera != null)
                {
                    // Enable left camera component
                    leftEyeCamera.gameObject.SetActive(true);
                    // Set left camera component to display to left eye only
                    leftEyeCamera.stereoTargetEye = StereoTargetEyeMask.Left;
                    // Set left camera to only show left eye layer display
                    leftEyeCamera.cullingMask = RIGHT_EYE_LAYER_MASK;

                    if (format == VideoFormat.OPT_ERP_180_LR || format == VideoFormat.OPT_ERP_360_LR || format == VideoFormat.OPT_FLAT_LR || format == VideoFormat.OPT_FISHEYE_LR)
                    {
                        leftSide.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(0.5f, 1.0f));
                        leftSide.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(0.5f, 0.0f));
                    }
                    else
                    {
                        leftSide.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(0.5f, 1.0f));
                        leftSide.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(0.0f, 0.5f));
                    }
                }

                if (rightEyeCamera != null)
                {
                    // Enable right camera component
                    rightEyeCamera.gameObject.SetActive(true);
                    // Set right camera component to display to right eye only
                    rightEyeCamera.stereoTargetEye = StereoTargetEyeMask.Right;
                    // Set right camera to only show right eye layer display
                    rightEyeCamera.cullingMask = LEFT_EYE_LAYER_MASK;

                    if (format == VideoFormat.OPT_ERP_180_TB || format == VideoFormat.OPT_ERP_360_TB || format == VideoFormat.OPT_FLAT_TB || format == VideoFormat.OPT_FISHEYE_TB)
                    {
                        rightSide.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(0.5f, 1.0f));
                        rightSide.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", Vector2.zero);
                    }
                    else
                    {
                        rightSide.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(0.5f, 1.0f));
                        rightSide.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", Vector2.zero);
                    }
                }
            }
            #endregion

            #region Adjust mesh component for rendering.
			UnloadMesh();
           

            SilverPlayer.MeshType meshType = SilverPlayer.MeshType.eMeshTypeErp360;
            SilverPlayer.MeshQuality meshQuality = SilverPlayer.MeshQuality.eQ1;
            float density = 0.5f;

            if (format == VideoFormat.OPT_FISHEYE_TB || format == VideoFormat.OPT_FISHEYE_LR)
            {
                meshType = SilverPlayer.MeshType.eMeshTypeFisheye180;
            }
            else if (format == VideoFormat.OPT_ERP_180 || format == VideoFormat.OPT_ERP_180_LR || format == VideoFormat.OPT_ERP_180_TB)
            {
                meshType = SilverPlayer.MeshType.eMeshTypeErp180;
            }
            else if (format == VideoFormat.OPT_ERP_360 || format == VideoFormat.OPT_ERP_360_LR || format == VideoFormat.OPT_ERP_360_TB)
            {
                meshType = SilverPlayer.MeshType.eMeshTypeErp360;
            }
            else
            {
                Debug.LogError("ERROR: unknown video format input.");
            }
            defaultMesh = SilverPlayer.GenerateMesh(meshType, meshQuality, density);
            if (leftSideMeshFilter != null)
            {
                leftSideMeshFilter.mesh = defaultMesh;
            }

            if (rightSideMeshFilter != null)
            {
                rightSideMeshFilter.mesh = defaultMesh;
            }
            #endregion
        }

        /// <summary>
        /// Bind player instance firstly, otherwise some events will be missing.
        /// </summary>
        private void BindPlayer()
        {
            player = gameObject.GetComponent<BitVideoPlayer>();

            player.OnResize += OnResize;
        }

        private void UnbindPlayer()
        {
            if (player != null)
            {
                player.OnResize -= OnResize;

                player = null;
            }
        }

        private void UnloadMesh()
		{
            if (leftSideMeshFilter != null && leftSideMeshFilter.mesh != null)
            {
                GameObject.Destroy(leftSideMeshFilter.mesh);
                leftSideMeshFilter.mesh = null;
            }

            if (rightSideMeshFilter != null && rightSideMeshFilter.mesh != null)
            {
                GameObject.Destroy(rightSideMeshFilter.mesh);
                rightSideMeshFilter.mesh = null;
            }
        }

        private void UnloadMaterial()
        {
            if (leftSideMeshRenderer != null && leftSideMeshRenderer.material != null)
            {
                GameObject.Destroy(leftSideMeshRenderer.material);
            }

            if (rightSideMeshRenderer != null && rightSideMeshRenderer.material != null)
            {
                GameObject.Destroy(rightSideMeshRenderer.material);
            }
        }

        // Adjust the mesh file when video resolution changed.
        void OnResize()
        {
            int videoWidth = player.GetVideoWidth();
            int videoHeight = player.GetVideoHeight();
        }
        public GameObject getLeftCameraObject()
        {
            return leftEyeCameraObject;
        }
        public GameObject getRightCameraObject()
        {
            return rightEyeCameraObject;
        }
    }
}

