using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCB.ScreenSwipes
{
    public class ScreenPlayer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private Vector3 _initialPosition;
        private List<IScreen> screens = new List<IScreen>();
        private Image cardSwipeImage;

        public void OnBeginDrag(PointerEventData eventData)
        {
            _initialPosition = transform.localPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.localPosition = new Vector2(transform.localPosition.x+eventData.delta.x, transform.localPosition.y);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var distanceMoved = Mathf.Abs(transform.localPosition.x - _initialPosition.x);
            if (distanceMoved < 0.4 * Screen.width)
            {
                transform.DOLocalMoveX(_initialPosition.x, 0.5f);
            }
            else
            {
                if (distanceMoved > 0)
                {
                    var direction = Mathf.Sign(transform.localPosition.x - _initialPosition.x);
                    var nextScreenIndex = screens.IndexOf(screens.First(s => s as MonoBehaviour == this)) + (int)direction;
                    if (nextScreenIndex >= 0 && nextScreenIndex < screens.Count)
                    {
                        var nextScreen = screens[nextScreenIndex] as MonoBehaviour;
                        nextScreen.transform.localPosition = new Vector3(Screen.width * -direction, 0, 0);
                        nextScreen.transform.DOLocalMoveX(0, 0.5f);
                        transform.DOLocalMoveX(Screen.width * direction, 0.5f).OnComplete(() =>
                        {
                            nextScreen.transform.localPosition = new Vector3(0, 0, 0);
                            transform.localPosition = new Vector3(Screen.width * -direction, 0, 0);
                            screens.ForEach(s => s.OnScreenExited());
                            nextScreen.GetComponent<IScreen>().OnScreenEntered();
                        });
                    }
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            screens = GetComponentsInChildren<IScreen>().ToList();
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(Screen.width * screens.Count, Screen.height);
            for (int i = 0; i < screens.Count; i++)
            {
                var screen = screens[i];
                var screenTransform = screen as MonoBehaviour;
                screenTransform.transform.localPosition = new Vector3(Screen.width * i, 0, 0);
            }
        }
    }
}
