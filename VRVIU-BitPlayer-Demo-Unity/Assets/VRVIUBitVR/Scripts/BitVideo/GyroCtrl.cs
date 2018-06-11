//-----------------------------------------------------
//            VRVIU: VRVIU-VR-PLAYER
//            Author: hogan.yin@vrviu.com  
// Copyright © 2016-2018 VRVIU Technologies Limited. 
//-----------------------------------------------------

namespace VRVIU.BitVRPlayer.BitVideo
{
    using UnityEngine;
    using UnityEngine.XR;

    public class GyroCtrl : MonoBehaviour
    {
        public bool enableDrag = false;

        private bool gyroEnabled = false;

        private const float DRAG_RATE = .05f;

        private float dragYawDegrees;

        private const float lowPassFilterFactor = 1.0f;

        private readonly Quaternion baseIdentity = Quaternion.Euler(90, 0, 0);

        private readonly Quaternion landscapeRight = Quaternion.Euler(0, 0, 90);

        private readonly Quaternion landscapeLeft = Quaternion.Euler(0, 0, -90);

        private readonly Quaternion upsideDown = Quaternion.Euler(0, 0, 180);

        private Quaternion cameraBase = Quaternion.identity;

        private Quaternion calibration = Quaternion.identity;

        private Quaternion baseOrientation = Quaternion.Euler(90, 0, 0);

        private Quaternion baseOrientationRotationFix = Quaternion.identity;

        private Quaternion referanceRotation = Quaternion.identity;

        void Start()
        {
            AttachGyro();
        }

        private void OnDestroy()
        {
            DetachGyro();
        }

        void Update()
        {
            if (XRSettings.enabled)
            {
                // Unity takes care of updating camera transform in VR.
                return;
            }

            if (!gyroEnabled)
            {
                return;
            }

            // android-developers.blogspot.com/2010/09/one-screen-turn-deserves-another.html
            // developer.android.com/guide/topics/sensors/sensors_overview.html#sensors-coords
            //
            //     y                                       x
            //     |  Gyro upright phone                   |  Gyro landscape left phone
            //     |                                       |
            //     |______ x                      y  ______|
            //     /                                       \
            //    /                                         \
            //   z                                           z
            //
            //
            //     y
            //     |  z   Unity
            //     | /
            //     |/_____ x
            //

            // Update `dragYawDegrees` based on user touch.
            if (enableDrag)
            {
                CheckDrag();
            }

            //transform.localRotation =

            //  // Allow user to drag left/right to adjust direction they're facing.
            //  Quaternion.Euler(0f, -dragYawDegrees, 0f) *

            //  // Neutral position is phone held upright, not flat on a table.
            //  Quaternion.Euler(90f, 0f, 0f) *

            //  // Sensor reading, assuming default `Input.compensateSensors == true`.
            //  Input.gyro.attitude *

            //  // So image is not upside down.
            //  Quaternion.Euler(0f, 0f, 180f);

            transform.localRotation = Quaternion.Euler(0f, -dragYawDegrees, 0f) * Quaternion.Slerp(transform.rotation, cameraBase * (ConvertRotation(referanceRotation * Input.gyro.attitude) * GetRotFix()), lowPassFilterFactor);
        }

        void CheckDrag()
        {
            if (Input.touchCount != 1)
            {
                return;
            }

            Touch touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Moved)
            {
                return;
            }

            dragYawDegrees += touch.deltaPosition.x * DRAG_RATE;
        }

        void UpdateCalibration(bool onlyHorizontal)
        {
            if (onlyHorizontal)
            {
                var fw = (Input.gyro.attitude) * (-Vector3.forward);
                fw.z = 0;

                if (fw == Vector3.zero)
                {
                    calibration = Quaternion.identity;
                }
                else
                {
                    calibration = (Quaternion.FromToRotation(baseOrientationRotationFix * Vector3.up, fw));
                }
            }
            else
            {
                calibration = Input.gyro.attitude;
            }
        }

        void UpdateCameraBaseRotation(bool onlyHorizontal)
        {
            if (onlyHorizontal)
            {
                var fw = transform.forward;
                fw.y = 0;
                if (fw == Vector3.zero)
                {
                    cameraBase = Quaternion.identity;
                }
                else
                {
                    cameraBase = Quaternion.FromToRotation(Vector3.forward, fw);
                }
            }
            else
            {
                cameraBase = transform.rotation;
            }
        }

        private static Quaternion ConvertRotation(Quaternion q)
        {
            return new Quaternion(q.x, q.y, -q.z, -q.w);
        }

        private Quaternion GetRotFix()
        {
#if UNITY_3_5
        if (Screen.orientation == ScreenOrientation.Portrait)
            return Quaternion.identity;
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.Landscape)
            return landscapeLeft;     
        if (Screen.orientation == ScreenOrientation.LandscapeRight)
            return landscapeRight;
        if (Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            return upsideDown;
        return Quaternion.identity;
#else
            return Quaternion.identity;
#endif
        }

        private void ResetBaseOrientation()
        {
            baseOrientationRotationFix = GetRotFix();
            baseOrientation = baseOrientationRotationFix * baseIdentity;
        }

        private void RecalculateReferenceRotation()
        {
            referanceRotation = Quaternion.Inverse(baseOrientation) * Quaternion.Inverse(calibration);
        }

        public void AttachGyro(bool recentering = true)
        {
            if (SystemInfo.supportsGyroscope)
            {
                Input.gyro.enabled = true;
                gyroEnabled = true;

                if (recentering)
                {
                    Recenter();
                }
            }
        }

        public void Recenter()
        {
            if (gyroEnabled)
            {
                transform.rotation = Quaternion.identity;
                ResetBaseOrientation();
                UpdateCalibration(true);
                UpdateCameraBaseRotation(true);
                RecalculateReferenceRotation();
                dragYawDegrees = 0;
            }
        }

        public void DetachGyro()
        {
            if (SystemInfo.supportsGyroscope)
            {
                Input.gyro.enabled = false;
                gyroEnabled = false;
            }
        }

    }
}