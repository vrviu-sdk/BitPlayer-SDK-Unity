namespace VRVIU.BitVrPlayer.Silver
{
    using UnityEngine;
    using VRVIU.BitVrPlayer.Video;

    public class SetViewerFormat : MonoBehaviour
    {
        public static string m_formatType = "mono";

        private const string FORMAT_STEREO_STRING = "stereo";
        private const string FORMAT_TOP_BOTTOM_STRING = "top_bottom";
        private const string FORMAT_BOTTOM_TOP_STRING = "bottom_top";
        private const string FORMAT_LEFT_RIGHT_STRING = "left_right";
        private const string FORMAT_RIGHT_LEFT_STRING = "right_left";

        private const string LEFT_EYE_CAMERA_NAME = "LeftEye";
        private const string RIGHT_EYE_CAMERA_NAME = "RightEye";

        private const string LEFT_EYE_LAYER_NAME = "LeftEye";
        private const string RIGHT_EYE_LAYER_NAME = "RightEye";

        private const string LEFT_EYE_IMAGE_SPHERE_NAME = "LeftSide/Holder";
        private const string RIGHT_EYE_IMAGE_SPHERE_NAME = "RightSide/Holder";

        private const float TOP_BOTTOM_TEXTURE_SCALE_X = 1.0f;
        private const float TOP_BOTTOM_TEXTURE_SCALE_Y = 0.5f;

        private const float LEFT_RIGHT_TEXTURE_SCALE_X = 0.5f;
        private const float LEFT_RIGHT_TEXTURE_SCALE_Y = 1.0f;

        private const float MONO_TEXTURE_SCALE = 1.0f;

        private const float TOP_BOTTOM_OFFSET_X = 0.0f;
        private const float LEFT_RIGHT_OFFSET_Y = 0.0f;

        // reversed upper and lower since image is rendered inverted
        private const float UPPER_TEXTURE_OFFSET = 0.5f;
        private const float LOWER_TEXTURE_OFFSET = 0.0f;

        private const float LEFT_TEXTURE_OFFSET = 0.0f;
        private const float RIGHT_TEXTURE_OFFSET = 0.5f;

        private GameObject leftEyeCamera;
        private Camera leftEyeCameraComponent;
        private GameObject rightEyeCamera;
        private Camera rightEyeCameraComponent;

        public void OnEnable()
        {
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

            // set up camera and image materials for specified playback format

            // monoscopic image
            if (!m_formatType.ToLower().Contains(FORMAT_STEREO_STRING))
            {
                // if mono image, then just use left camera and disable the right camera

                // disable right camera object
                if (rightEyeCamera != null)
                {
                    rightEyeCamera.SetActive(false);
                }

                // enable left eye camera object
                if (leftEyeCamera != null)
                {
                    leftEyeCamera.SetActive(true);
                }

                // set left camera component to display both eyes
                leftEyeCameraComponent.stereoTargetEye = StereoTargetEyeMask.Both;

                // only need to show left eye layer display (since we will display same image on both eyes)
                leftEyeCameraComponent.cullingMask = LEFT_EYE_LAYER_MASK;

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
                    leftEyeCameraComponent.cullingMask = LEFT_EYE_LAYER_MASK;
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
                    rightEyeCameraComponent.cullingMask = RIGHT_EYE_LAYER_MASK;
                    rightEyeCameraComponent.stereoSeparation = 0;
                }

            }

            // texture tiling and offset
            float textureScaleX, textureScaleY;
            float textureLeftEyeOffsetX, textureLeftEyeOffsetY;
            float textureRightEyeOffsetX, textureRightEyeOffsetY;

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

                textureScaleX = MONO_TEXTURE_SCALE;
                textureScaleY = MONO_TEXTURE_SCALE;
                textureLeftEyeOffsetX = 0.0f;
                textureLeftEyeOffsetY = 0.0f;
                textureRightEyeOffsetX = 0.0f;
                textureRightEyeOffsetY = 0.0f;

            }

            // get left eye image sphere
            GameObject leftEyeImageSphere = GameObject.Find(LEFT_EYE_IMAGE_SPHERE_NAME);
            // get left eye mesh renderer
            Renderer leftEyeImageRenderer = leftEyeImageSphere.GetComponent<MeshRenderer>();

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
            Renderer rightEyeImageRenderer = rightEyeImageSphere.GetComponent<MeshRenderer>();
            // set material for right eye image rendering
            rightEyeImageRenderer.material = rightMat;
        }

        // Use this for initialization
        void Start()
        {
            SetUpViewerFormat();
        }

    }
}