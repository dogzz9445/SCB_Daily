using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SCB.ScreenSwipes
{
    public class ScreenSwipe : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public void OnBeginDrag(PointerEventData eventData)
        {
            GetComponentInParent<ScreenPlayer>()?.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            GetComponentInParent<ScreenPlayer>()?.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            GetComponentInParent<ScreenPlayer>()?.OnEndDrag(eventData);
        }
    }
}
