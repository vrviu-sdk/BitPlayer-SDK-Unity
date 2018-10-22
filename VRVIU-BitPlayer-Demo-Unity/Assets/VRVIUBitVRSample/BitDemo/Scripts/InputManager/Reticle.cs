using UnityEngine;
using UnityEngine.UI;

namespace VRVIU.LightVR.InputManager
{
    // The reticle is a small point at the centre of the screen.
    // It is used as a visual aid for aiming. The position of the
    // reticle is either at a default position in space or on the
    // surface of a VRInteractiveItem as determined by the VREyeRaycaster.
    public class Reticle : MonoBehaviour
    {
        private const string RETICLE_OBJECT_NAME = "Gaze_Canvas";
        private const string RETICLE_SCRIPT_NAME = "Reticle";
        private const string RETICLE_RAYCASTER_SCRIPT_NAME = "VREyeRaycaster";

        [SerializeField] private float m_DefaultDistance = 1f;      // The default distance away from the camera the reticle is placed.
        [SerializeField] private bool m_UseNormal;                  // Whether the reticle should be placed parallel to a surface.
        [SerializeField] private Image m_Image;                     // Reference to the image component that represents the reticle.
        [SerializeField] private Transform m_ReticleTransform;      // We need to affect the reticle's transform.
        [SerializeField] private Transform m_Camera;                // The reticle is always placed relative to the camera.


        private Vector3 m_OriginalScale;                            // Since the scale of the reticle changes, the original scale needs to be stored.
        private Quaternion m_OriginalRotation;                      // Used to store the original rotation of the reticle.


        public bool UseNormal
        {
            get { return m_UseNormal; }
            set { m_UseNormal = value; }
        }


        public Transform ReticleTransform { get { return m_ReticleTransform; } }


        private void Awake()
        {
            // Store the original scale and rotation.
            m_OriginalScale = m_ReticleTransform.localScale;
            m_OriginalRotation = m_ReticleTransform.localRotation;
        }


        public void Hide()
        {
            m_Image.enabled = false;
        }


        public void Show()
        {
            m_Image.enabled = true;
        }


        // This overload of SetPosition is used when the the VREyeRaycaster hasn't hit anything.
        public void SetPosition ()
        {
            // Set the position of the reticle to the default distance in front of the camera.
            //m_ReticleTransform.position = m_Camera.position + m_Camera.forward * m_DefaultDistance;

            // Set the scale based on the original and the distance from the camera.
            m_ReticleTransform.localScale = Vector3.Scale(new Vector3(1.5f, 1.5f, 1.5f), m_OriginalScale); ;
           
            // The rotation should just be the default.
            //m_ReticleTransform.localRotation = m_OriginalRotation;
        }


        // This overload of SetPosition is used when the VREyeRaycaster has hit something.
        public void SetPosition (RaycastHit hit)
        {
           // m_ReticleTransform.position = hit.point;
            m_ReticleTransform.localScale = Vector3.Scale(new Vector3(2.5f, 2.5f, 2.5f), m_OriginalScale);
            
            // If the reticle should use the normal of what has been hit...
            if (m_UseNormal)
                // ... set it's rotation based on it's forward vector facing along the normal.
                m_ReticleTransform.rotation = Quaternion.FromToRotation (Vector3.forward, hit.normal);
            else
                // However if it isn't using the normal then it's local rotation should be as it was originally.
                m_ReticleTransform.localRotation = m_OriginalRotation;
        }

        // Searches for reticle object in scene and if found calls script to hide reticle
        public static void HideReticle()
        {
            // hide reticle
            Reticle reticle = GameObject.Find(RETICLE_OBJECT_NAME).GetComponent(RETICLE_SCRIPT_NAME) as Reticle;
            if (null != reticle)
            {
                reticle.Hide();
            }

            // disable reticle raycaster
            VREyeRaycaster reticleSelection = GameObject.Find(RETICLE_OBJECT_NAME).GetComponent(RETICLE_RAYCASTER_SCRIPT_NAME) as VREyeRaycaster;
            if (null != reticleSelection)
            {
                reticleSelection.enabled = false;
            }
        }

        // Searches for reticle object in scene and if found calls script to show reticle
        public static void ShowReticle()
        {
            // enable reticle
            Reticle reticle = GameObject.Find(RETICLE_OBJECT_NAME).GetComponent(RETICLE_SCRIPT_NAME) as Reticle;
            if (null != reticle)
            {
                reticle.Show();
            }

            // enable reticle raycaster
            VREyeRaycaster reticleSelection = GameObject.Find(RETICLE_OBJECT_NAME).GetComponent(RETICLE_RAYCASTER_SCRIPT_NAME) as VREyeRaycaster;
            if (null != reticleSelection)
            {
                reticleSelection.enabled = true;
            }

        }

        public void RemoveReticleFromScene()
        {
            HideReticle();
        }

        public void AddReticleToScene()
        {
            ShowReticle();
        }
    }
}