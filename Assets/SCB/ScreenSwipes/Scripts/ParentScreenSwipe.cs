using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SCB.ScreenSwipes
{
    public class ScreenSwipe : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private IDragHandler _dragHandler;
        public IDragHandler DragHandler 
        { 
            get 
            {
                _dragHandler ??= GetComponentInParent<ScreenPlayer>(); 
                _dragHandler ??= GetComponentInParent<ScreenSwipe>();
                return _dragHandler;
            }
        }

        private IBeginDragHandler _beginDragHandler;
        public IBeginDragHandler BeginDragHandler 
        { 
            get 
            {
                _beginDragHandler ??= GetComponentInParent<ScreenPlayer>(); 
                _beginDragHandler ??= GetComponentInParent<ScreenSwipe>();
                return _beginDragHandler;
            }
        }

        private IEndDragHandler _endDragHandler;
        public IEndDragHandler EndDragHandler 
        { 
            get 
            {
                _endDragHandler ??= GetComponentInParent<ScreenPlayer>(); 
                _endDragHandler ??= GetComponentInParent<ScreenSwipe>();
                return _endDragHandler;
            }
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            BeginDragHandler.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            DragHandler.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EndDragHandler.OnEndDrag(eventData);
        }
    }
}
