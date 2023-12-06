using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace SCB.CardSwipes
{
    public class SwipeEffect : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private Vector3 _initialPosition;
        private float _distanceMoved;
        private bool _isSwipeLeft;

        public void OnDrag(PointerEventData eventData)
        {
            transform.localPosition = new Vector2(transform.localPosition.x+eventData.delta.x, transform.localPosition.y);

            if (transform.localPosition.x - _initialPosition.x > 0)
            {
                transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0, -30, (_initialPosition.x + transform.localPosition.x)/(Screen.width/2)));
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0, 30, (_initialPosition.x - transform.localPosition.x)/(Screen.width/2)));
            }
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
                transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
            }
            else
            {
                if (transform.localPosition.x > _initialPosition.x)
                {
                    _isSwipeLeft = false;
                }
                else
                {
                    _isSwipeLeft = true;
                }
                StartCoroutine(MovedCard());
            }
            // DOTween.To(() => transform.localPosition, x => transform.localPosition = x, _initialPosition, 0.5f);
        }

        private IEnumerator MovedCard()
        {
            float time = 0;
            while (GetComponent<Image>().color != new Color(1, 1, 1, 0))
            {
                time += Time.deltaTime;
                if (_isSwipeLeft)
                {
                    transform.localPosition = new Vector3(Mathf.SmoothStep(transform.localPosition.x, transform.localPosition.x-Screen.width, 4*time), transform.localPosition.y, 0);
                }
                else
                {
                    transform.localPosition = new Vector3(Mathf.SmoothStep(transform.localPosition.x, transform.localPosition.x+Screen.width, 4*time), transform.localPosition.y, 0);
                }
                GetComponent<Image>().color = new Color(1, 1, 1, Mathf.SmoothStep(1, 0, 4*time));
                // GetComponent<Image>().color = Color.Lerp(GetComponent<Image>().color, new Color(1, 1, 1, 0), 4*time);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}