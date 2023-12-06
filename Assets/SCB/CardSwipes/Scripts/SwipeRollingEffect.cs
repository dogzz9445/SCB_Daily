using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCB.CardSwipes
{
    public class SwipeRollingEffect : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private Vector3 _initialPosition;

        public void OnDrag(PointerEventData eventData)
        {
            transform.localPosition = new Vector2(transform.localPosition.x+eventData.delta.x, transform.localPosition.y);
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            _initialPosition = transform.localPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.localPosition = _initialPosition;
        }
    }
}