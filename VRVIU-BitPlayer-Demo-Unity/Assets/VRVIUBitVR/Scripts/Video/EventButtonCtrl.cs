//-----------------------------------------------------
//            VRVIU: VRVIU-VR-PLAYER
//            Author: hogan.yin@vrviu.com  
// Copyright © 2016-2018 VRVIU Technologies Limited. 
//-----------------------------------------------------

namespace VRVIU.BitVrPlayer.Video
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class EventButtonCtrl : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler{

        public bool isDragging;

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnPointerUp(PointerEventData eventData)
        {

        }
    }
}