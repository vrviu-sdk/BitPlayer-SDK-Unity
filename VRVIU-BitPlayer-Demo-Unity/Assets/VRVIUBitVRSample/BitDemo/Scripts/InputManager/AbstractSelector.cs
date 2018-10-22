using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace VRVIU.LightVR.InputManager
{
    public abstract class AbstractSelector : MonoBehaviour
    {
        public event Action OnSelectionComplete;
        public abstract void Show();
        public abstract void Hide();
        protected void NotifySelectionComplete()
        {
            // If there is anything subscribed to OnSelectionComplete call it.
            if (this.OnSelectionComplete != null)
                OnSelectionComplete();
        }
    }
}