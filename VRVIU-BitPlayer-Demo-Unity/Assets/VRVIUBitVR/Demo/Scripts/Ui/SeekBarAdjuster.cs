//-----------------------------------------------------
//            VRVIU: VRVIU-VR-PLAYER
//            Author: hogan.yin@vrviu.com  
// Copyright © 2016-2018 VRVIU Technologies Limited. 
//-----------------------------------------------------

namespace VRVIU.BitVrPlayer.Video
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    

    public class SeekBarAdjuster : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        #region Fields
        
        public BitVideoPlayer player = null;

        public Slider videoSlider;

        public bool isDragging = false;

        public bool isUpdate = true;

        public float lastOperationTime;

        #endregion

        #region Methods

        private void Start()
        {
            lastOperationTime = Time.time;
        }

        private void Update()
        {
            if (isDragging || !isUpdate)
            {
                return;
            }

			if (videoSlider != null && player != null)
            {
               videoSlider.value = player.GetSeekBarValue();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isUpdate = false;

            lastOperationTime = Time.time;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isUpdate = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isUpdate = true;
            player.SetSeekBarValue(videoSlider.value);
            
            lastOperationTime = Time.time;
        }

        public void OnDrag(PointerEventData eventData)
        {
            lastOperationTime = Time.time;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;

            isUpdate = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;

            isUpdate = true;
            player.SetSeekBarValue(videoSlider.value);
        }
    }

    #endregion
}