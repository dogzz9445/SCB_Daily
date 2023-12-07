using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace SCB.CardSwipes
{
    public class SwipeRollingEffect : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private Vector3 _initialPosition;
        private float _distanceMoved;

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
            _distanceMoved = Mathf.Abs(transform.localPosition.x - _initialPosition.x);
            if (_distanceMoved < 0.4*Screen.width)
            {
                transform.DOLocalMoveX(_initialPosition.x, 0.5f);
            }
            // DOTween.To(() => transform.localPosition, x => transform.localPosition = x, _initialPosition, 0.5f);
        }
    }
}