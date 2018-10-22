using Assets.VRVIUBitVR.Scripts.Log;

namespace VRVIU.BitVRPlayer.BitVideo.Silver
{
    using UnityEngine;

    public class SetViewerFormat : MonoBehaviour
    {
        public static string m_formatType = "mono";
        public static string m_videoFormatType = "mono";

        private const string FORMAT_STEREO_STRING = "stereo";
        private const string FORMAT_TOP_BOTTOM_STRING = "top_bottom";
        private const string FORMAT_BOTTOM_TOP_STRING = "bottom_top";
        private const string FORMAT_LEFT_RIGHT_STRING = "left_right";
        private const string FORMAT_RIGHT_LEFT_STRING = "right_left";

        private const string LEFT_EYE_CAMERA_NAME = "LeftEye";
        private const string RIGHT_EYE_CAMERA_NAME = "RightEye";

        private const string LEFT_EYE_LAYER_NAME = "LeftEye";
        private const string RIGHT_EYE_LAYER_NAME = "RightEye";
        private const string UI_LAYER_NAME = "UI";
        private const string DEFAULT_LAYER_NAME = "Default";
        private const string IGONRE_RAYCAST_LAYER_NAME = "Ignore Raycast";
        private const string EVERYTHING_LAYER_NAME = "Everything";
        private const string TRANSPARENTFX_LAYER_NAME = "TransparentFX";


        private const string LEFT_EYE_IMAGE_SPHERE_NAME = "LeftSide/Holder";
        private const string RIGHT_EYE_IMAGE_SPHERE_NAME = "RightSide/Holder";

        private const float TOP_BOTTOM_TEXTURE_SCALE_X = 1.0f;
        private const float TOP_BOTTOM_TEXTURE_SCALE_Y = 0.5f;

        private const float LEFT_RIGHT_TEXTURE_SCALE_X = 0.5f;
        private const float LEFT_RIGHT_TEXTURE_SCALE_Y = 1.0f;

        private const float MONO_TEXTURE_SCALE = 1.0f;
        private const float TOMONO_TEXTURE_SCALE = 0.5f;

        private const float TOP_BOTTOM_OFFSET_X = 0.0f;
        private const float LEFT_RIGHT_OFFSET_Y = 0.0f;

        // reversed upper and lower since image is rendered inverted
        private const float UPPER_TEXTURE_OFFSET = 0.0f;
        private const float LOWER_TEXTURE_OFFSET = 0.5f;

        private const float LEFT_TEXTURE_OFFSET = 0.0f;
        private const float RIGHT_TEXTURE_OFFSET = 0.5f;

        private GameObject leftEyeCamera;
        private Camera leftEyeCameraComponent;
        private GameObject rightEyeCamera;
        private Camera rightEyeCameraComponent;

        private Renderer rightEyeImageRenderer;
        private Renderer leftEyeImageRenderer;

        private MeshFilter rightEyeImageFilter;
        private MeshFilter leftEyeImageFilter;
        public void OnEnable()
        {
            VLog.log(VLog.LEVEL_INFO,"SilverFormat OnEnable");
            VideoFormater formatter = FindObjectOfType< VideoFormater>();

            if (formatter)
            {
                leftEyeCamera = formatter.getLeftCameraObject();
                rightEyeCamera = formatter.getRightCameraObject();
            }
            else
            {
                // get the left eye camera object
                leftEyeCamera = GameObject.Find(LEFT_EYE_CAMERA_NAME);

                // get the right eye camera object
                rightEyeCamera = GameObject.Find(RIGHT_EYE_CAMERA_NAME);
            }
            // get left eye camera components
            leftEyeCameraComponent = leftEyeCamera.GetComponent<Camera>();
            // get right camera component
            rightEyeCameraComponent = rightEyeCamera.GetComponent<Camera>();
        }

        public void SetUpViewerFormat()
        {
            int LEFT_EYE_LAYER_MASK = (1 << LayerMask.NameToLayer(LEFT_EYE_LAYER_NAME));
            int RIGHT_EYE_LAYER_MASK = (1 << LayerMask.NameToLayer(RIGHT_EYE_LAYER_NAME));
            int UI_LAYER_MASK = (1 << LayerMask.NameToLayer(UI_LAYER_NAME));
            int DEFAULT_LAYER_MASK = (1 << LayerMask.NameToLayer(DEFAULT_LAYER_NAME));
            int IGONRE_RAYCAST_LAYER_MASK = (1 << LayerMask.NameToLayer(IGONRE_RAYCAST_LAYER_NAME));
            int EVERY_THING_LAYER_MASK = (1 << LayerMask.NameToLayer(EVERYTHING_LAYER_NAME));
            int TRANSPRENTFX_LAYER_MASK = (1 << LayerMask.NameToLayer(TRANSPARENTFX_LAYER_NAME)); 

            // set up camera and image materials for specified playback format

            // monoscopic image
            if (!m_formatType.ToLower().Contains(FORMAT_STEREO_STRING))
            {
                // if mono image, then just use left camera and disable the right camera

                // disable right camera object
                if (rightEyeCamera != null)
                {
                    rightEyeCamera.SetActive(true);
                }

                // enable left eye camera object
                if (leftEyeCamera != null)
                {
                    leftEyeCamera.SetActive(true);
                }

                // set left camera component to display both eyes
                leftEyeCameraComponent.stereoTargetEye = StereoTargetEyeMask.Both;

                // only need to show left eye layer display (since we will display same image on both eyes)
                leftEyeCameraComponent.cullingMask = LEFT_EYE_LAYER_MASK+ UI_LAYER_MASK + DEFAULT_LAYER_MASK + 
                    IGONRE_RAYCAST_LAYER_MASK + EVERY_THING_LAYER_MASK + TRANSPRENTFX_LAYER_MASK;

                rightEyeCameraComponent.stereoTargetEye = StereoTargetEyeMask.Both;
                rightEyeCameraComponent.cullingMask = RIGHT_EYE_LAYER_MASK + UI_LAYER_MASK + DEFAULT_LAYER_MASK +
                    IGONRE_RAYCAST_LAYER_MASK + EVERY_THING_LAYER_MASK + TRANSPRENTFX_LAYER_MASK;

            }
            else
            {
                // stereoscopic image - use both left and right cameras

                // enable left camera object
                if (leftEyeCamera != null)
                {
                    // set left eye camera active
                    leftEyeCamera.SetActive(true);

                    // set left camera component to display to left eye only
                    leftEyeCameraComponent.stereoTargetEye = StereoTargetEyeMask.Left;
                    // set left camera to only show left eye layer display
                    leftEyeCameraComponent.cullingMask = LEFT_EYE_LAYER_MASK + UI_LAYER_MASK + DEFAULT_LAYER_MASK +
                    IGONRE_RAYCAST_LAYER_MASK + EVERY_THING_LAYER_MASK + TRANSPRENTFX_LAYER_MASK;
                    leftEyeCameraComponent.stereoSeparation = 0;
                }

                // enable right camera object
                if (rightEyeCamera != null)
                {
                    // set right eye camera active
                    rightEyeCamera.SetActive(true);

                    // set right camera component to display to right eye only
                    rightEyeCameraComponent.stereoTargetEye = StereoTargetEyeMask.Right;
                    // set right camera to only show right eye layer display
                    rightEyeCameraComponent.cullingMask = RIGHT_EYE_LAYER_MASK + UI_LAYER_MASK + DEFAULT_LAYER_MASK +
                    IGONRE_RAYCAST_LAYER_MASK + EVERY_THING_LAYER_MASK + TRANSPRENTFX_LAYER_MASK;
                    rightEyeCameraComponent.stereoSeparation = 0;
                }

            }

            // texture tiling and offset
            float textureScaleX, textureScaleY;
            float textureLeftEyeOffsetX, textureLeftEyeOffsetY;
            float textureRightEyeOffsetX, textureRightEyeOffsetY;
            VLog.log(VLog.LEVEL_DEBUG, "SetUpViewerFormat m_formatType" + m_formatType);
            VLog.log(VLog.LEVEL_DEBUG, "SetUpViewerFormat m_videoFormatType" + m_videoFormatType);

            // set display image material based on encoded image format
            if (m_formatType.ToLower().Contains(FORMAT_TOP_BOTTOM_STRING))
            {

                textureScaleX = TOP_BOTTOM_TEXTURE_SCALE_X;
                textureScaleY = TOP_BOTTOM_TEXTURE_SCALE_Y;
                textureLeftEyeOffsetX = TOP_BOTTOM_OFFSET_X;
                textureLeftEyeOffsetY = UPPER_TEXTURE_OFFSET;
                textureRightEyeOffsetX = TOP_BOTTOM_OFFSET_X;
                textureRightEyeOffsetY = LOWER_TEXTURE_OFFSET;

            }
            else if (m_formatType.ToLower().Contains(FORMAT_BOTTOM_TOP_STRING))
            {

                textureScaleX = TOP_BOTTOM_TEXTURE_SCALE_X;
                textureScaleY = TOP_BOTTOM_TEXTURE_SCALE_Y;
                textureLeftEyeOffsetX = TOP_BOTTOM_OFFSET_X;
                textureLeftEyeOffsetY = LOWER_TEXTURE_OFFSET;
                textureRightEyeOffsetX = TOP_BOTTOM_OFFSET_X;
                textureRightEyeOffsetY = UPPER_TEXTURE_OFFSET;

            }
            else if (m_formatType.ToLower().Contains(FORMAT_LEFT_RIGHT_STRING))
            {

                textureScaleX = LEFT_RIGHT_TEXTURE_SCALE_X;
                textureScaleY = LEFT_RIGHT_TEXTURE_SCALE_Y;
                textureLeftEyeOffsetX = LEFT_TEXTURE_OFFSET;
                textureLeftEyeOffsetY = LEFT_RIGHT_OFFSET_Y;
                textureRightEyeOffsetX = RIGHT_TEXTURE_OFFSET;
                textureRightEyeOffsetY = LEFT_RIGHT_OFFSET_Y;

            }
            else if (m_formatType.ToLower().Contains(FORMAT_RIGHT_LEFT_STRING))
            {

                textureScaleX = LEFT_RIGHT_TEXTURE_SCALE_X;
                textureScaleY = LEFT_RIGHT_TEXTURE_SCALE_Y;
                textureLeftEyeOffsetX = RIGHT_TEXTURE_OFFSET;
                textureLeftEyeOffsetY = LEFT_RIGHT_OFFSET_Y;
                textureRightEyeOffsetX = LEFT_TEXTURE_OFFSET;
                textureRightEyeOffsetY = LEFT_RIGHT_OFFSET_Y;

            }
            else
            {
                VLog.log(VLog.LEVEL_DEBUG, "SetUpViewerFormat m_videoFormatType" + m_videoFormatType);

                textureScaleX = MONO_TEXTURE_SCALE;
                textureScaleY = MONO_TEXTURE_SCALE;
                textureLeftEyeOffsetX = 0.0f;
                textureLeftEyeOffsetY = 0.0f;
                textureRightEyeOffsetX = 0.0f;
                textureRightEyeOffsetY = 0.0f;

                if(m_videoFormatType.ToLower().Contains(FORMAT_LEFT_RIGHT_STRING)
                    || m_videoFormatType.ToLower().Contains(FORMAT_RIGHT_LEFT_STRING))
                {
                    textureScaleX = TOMONO_TEXTURE_SCALE;
                    VLog.log(VLog.LEVEL_DEBUG, "SetUpViewerFormat textureScaleX = TOMONO_TEXTURE_SCALE" + TOMONO_TEXTURE_SCALE);
                }

                if (m_videoFormatType.ToLower().Contains(FORMAT_BOTTOM_TOP_STRING)
                    || m_videoFormatType.ToLower().Contains(FORMAT_TOP_BOTTOM_STRING))
                {
                    textureScaleY = TOMONO_TEXTURE_SCALE;
                    VLog.log(VLog.LEVEL_DEBUG, "SetUpViewerFormat textureScaleY = TOMONO_TEXTURE_SCALE" + TOMONO_TEXTURE_SCALE);
                }

            }

            // get left eye image sphere
            GameObject leftEyeImageSphere = GameObject.Find(LEFT_EYE_IMAGE_SPHERE_NAME);
            // get left eye mesh renderer
            leftEyeImageRenderer = leftEyeImageSphere.GetComponent<MeshRenderer>();
            leftEyeImageFilter = leftEyeImageSphere.GetComponent<MeshFilter>();
            // load default material for left eye
            Material leftMat = (Material)Instantiate(leftEyeImageRenderer.material);
            // duplicate material for right eye
            Material rightMat = (Material)Instantiate(leftMat);

            // set up tiling and offset for left material
            leftMat.mainTextureScale = new Vector2(textureScaleX, textureScaleY);
            leftMat.mainTextureOffset = new Vector2(textureLeftEyeOffsetX, textureLeftEyeOffsetY);

            // set up tiling and offset for right material
            rightMat.mainTextureScale = new Vector2(textureScaleX, textureScaleY);
            rightMat.mainTextureOffset = new Vector2(textureRightEyeOffsetX, textureRightEyeOffsetY);

            // set material for left eye image rendering
            leftEyeImageRenderer.material = leftMat;

            // get right eye image sphere
            GameObject rightEyeImageSphere = GameObject.Find(RIGHT_EYE_IMAGE_SPHERE_NAME);
            // get right eye mesh renderer
            rightEyeImageRenderer = rightEyeImageSphere.GetComponent<MeshRenderer>();
            rightEyeImageFilter = rightEyeImageSphere.GetComponent<MeshFilter>();
            // set material for right eye image rendering
            rightEyeImageRenderer.material = rightMat;
        }

        public MeshFilter[] GetTargetMaterial (){
            MeshFilter[] material = new MeshFilter[2];
            material[0] = leftEyeImageFilter;
            material[1] = rightEyeImageFilter;
            return material;
        }

        public void Release() {
            UnloadMesh();
            UnloadMaterial();
        }
        private void UnloadMesh()
        {
            if (leftEyeImageFilter != null && leftEyeImageFilter.mesh != null)
            {
                GameObject.Destroy(leftEyeImageFilter.mesh);
                leftEyeImageFilter.mesh = null;
            }

            if (rightEyeImageFilter != null && rightEyeImageFilter.mesh != null)
            {
                GameObject.Destroy(rightEyeImageFilter.mesh);
                rightEyeImageFilter.mesh = null;
            }
        }

        private void UnloadMaterial()
        {
            if (leftEyeImageRenderer != null && leftEyeImageRenderer.material != null)
            {
                GameObject.Destroy(leftEyeImageRenderer.material);
            }

            if (rightEyeImageRenderer != null && rightEyeImageRenderer.material != null)
            {
                GameObject.Destroy(rightEyeImageRenderer.material);
            }
        }

        // Use this for initialization
        void Start()
        {
            VLog.log(VLog.LEVEL_INFO, "SilverFormat Start");
            SetUpViewerFormat();
        }

        public GameObject getLeftEyeCarema() {
            return leftEyeCamera;
        }
    }
}