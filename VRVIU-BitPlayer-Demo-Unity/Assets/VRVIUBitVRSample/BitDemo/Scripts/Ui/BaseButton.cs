using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VRVIU.LightVR.InputManager;

namespace VRVIU.LightVR.UI
{
    public class BaseButton : Button
    {
        [SerializeField] public AbstractSelector m_selector;
        protected VRInteractiveItem m_InteractiveItem;     // The interactive item for where the user should click to load the video.
        protected bool m_GazeOver;                                          // Whether the user is looking at the VRInteractiveItem currently.

        [SerializeField] public AudioSource m_audioSource;                  // Sound effects when clicked
        [SerializeField] public AudioClip m_audioClip;                      // Which sound to play
        [SerializeField] public bool m_playSound = true;                    // Whether to play a sound or not

		[SerializeField] public bool m_gazeTimer = true;                    // Whether to turn on gaze timer or not

        private DateTime m_startupTime = DateTime.Now;                      // This time field is used for debugging.

        public bool Interactable
        {
            get { return this.interactable; }
            set
            {
                if (value == false)
                    m_GazeOver = false;

                if (m_InteractiveItem)
                    m_InteractiveItem.IsInteractable = value;
                
                this.interactable = value;
            }
        }

        // Default on Click Sound
        public void PlaySound()    
        {
            GameObject audioSourceObject = GameObject.FindGameObjectWithTag("SoundEffects"); 
            //Dialog.AssertObjectTag(audioSourceObject != null, "BaseButton.cs: SoundEffects.");

            if ((m_audioSource == null) && (null != audioSourceObject))        // Set audio player
                m_audioSource = audioSourceObject.GetComponent<AudioSource>();

            if (m_audioSource != null)
            {
                if (m_audioClip != null)
                {
                    m_audioSource.clip = m_audioClip;                            // Override with custom button sound..
                }
                else if (null != audioSourceObject)
                {
                   
                }

                if (m_audioSource != null)
                {
                    m_audioSource.Play();
                }
            }
        }

        protected virtual void OnClick()
        {
            //PlaySound();
        }

        protected virtual void UpdateDebugText()
        {
            Component[] textComponents;

            DateTime curTime = DateTime.Now;
            TimeSpan timeElapsed = curTime - m_startupTime;

            textComponents = this.GetComponentsInChildren<Text>();

            foreach (Text textComponent in textComponents)
            {
                if (textComponent.CompareTag("VR_Feedback"))
                    textComponent.text = string.Format("{0:D}:{1:D2}", timeElapsed.Minutes, timeElapsed.Seconds);
            }
        }

        protected override void OnEnable()
        {
            if (m_InteractiveItem != null)
            {
                m_InteractiveItem.OnOver += HandleOver;
                m_InteractiveItem.OnOut += HandleOut;
                m_InteractiveItem.OnClick += ActivateButton;
            }
            else {
                m_InteractiveItem = gameObject.GetComponent<VRInteractiveItem>();
                m_InteractiveItem.OnOver += HandleOver;
                m_InteractiveItem.OnOut += HandleOut;
                m_InteractiveItem.OnClick += ActivateButton;
            }
           
            if (m_selector == null)
            {
                m_selector =  GameObject.Find("Gaze_GUI").GetComponent("SelectionRadial") as AbstractSelector;
                //Dialog.Assert(m_selector != null, "BaseButton.OnEnable() could not find OVRCameraRig's SelectionRadial.", AssertLevel.Level_1);
            }

            // Invoke should NOT be called if the button is inactive (!interactable)
            this.onClick.AddListener(OnClick);

            if (m_selector != null)
                m_selector.OnSelectionComplete += HandleSelectionComplete;
        }

        protected override void OnDisable()
        {
            if (m_InteractiveItem != null)
            {
                m_InteractiveItem.OnOver -= HandleOver;
                m_InteractiveItem.OnOut -= HandleOut;
                m_InteractiveItem.OnClick -= ActivateButton;
            }

			// remove on click listener
			this.onClick.RemoveListener(OnClick);

            if (m_selector!=null)
                m_selector.OnSelectionComplete -= HandleSelectionComplete;
        }

        public void ClearVRButtonState()                        // This routing called when disabling Next/Prev Buttons.
        {
             if (m_GazeOver)
              {
                  // Activate vrButton's OnExit() in order to un-highlight the button
                  var eventPtr = new PointerEventData(EventSystem.current);
                  ExecuteEvents.Execute(this.gameObject, eventPtr, ExecuteEvents.pointerExitHandler);

                  m_GazeOver = false;
              }
        }

        protected virtual void HandleOver()
        {
            // Activate vrButton's OnEnter() in order to highlight the button
            var eventPtr = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(this.gameObject, eventPtr, ExecuteEvents.pointerEnterHandler);
			//check if user wants to use radial gaze timer
			if (m_gazeTimer)
            {
				// When the user looks at the rendering of the scene, show the radial.
				if (null != m_selector)
                {
					m_selector.Show();
				}
				m_GazeOver = true;
			}
            HandleOverKeyBtnsUIStatus();
        }

        public void HandleOverKeyBtnsUIStatus()
        {
            if (this.tag == "VRGazeInteractable")
            {
                //this.GetComponent<Image>().color = DataHandler.GetInstance().HexToColor("FFFFFFFF");
            }
        }

        public void HandleOutKeyBtnsUIStatus()
        {
            if (this.tag == "VRGazeInteractable")
            {
                //this.GetComponent<Image>().color = DataHandler.GetInstance().HexToColor("302F37FF");
            }
            
        }


        public void HandleOut()
        {
            // Activate vrButton's OnExit() in order to un-highlight the button
            var eventPtr = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(this.gameObject, eventPtr, ExecuteEvents.pointerExitHandler);

            // When the user looks away from the rendering of the scene, hide the radial.
            if (null != m_selector)
            {
                m_selector.Hide();
            }
            m_GazeOver = false;

            HandleOutKeyBtnsUIStatus();
        }

        protected virtual void HandleSelectionComplete()
        {
            // If the user is looking at the rendering of the scene when the radial's selection finishes, activate the button.
             if(m_GazeOver)
                ActivateButton();
        }

        protected virtual void ActivateButton()
        {
            this.onClick.Invoke();
        }
    }
}