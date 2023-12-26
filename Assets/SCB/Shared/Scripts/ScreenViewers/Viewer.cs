using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SCB.Cores.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SCB.Shared.Viewers
{
    public enum ScrollType
    {
        Step,
        Linear,
    }

    public enum ScrollDirection
    {
        Left,
        Right,
    }

    public enum ScrollOrientation
    {
        Horizontal,
        Vertical,
    }

    public class Viewer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [Header("Viewer Settings")]
        private Vector3 _initialPosition;
        private View _initialScreen;
        public ScrollType ScrollType;
        public List<View> views = new List<View>();

        public void OnBeginDrag(PointerEventData eventData)
        {
            switch (ScrollType)
            {
                case ScrollType.Step:
                    OnBeginDragStep(eventData);
                    break;
                case ScrollType.Linear:
                    OnBeginDragLinear(eventData);
                    break;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            switch (ScrollType)
            {
                case ScrollType.Step:
                    OnDragStep(eventData);
                    break;
                case ScrollType.Linear:
                    OnDragLinear(eventData);
                    break;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            switch (ScrollType)
            {
                case ScrollType.Step:
                    OnEndDragStep(eventData);
                    break;
                case ScrollType.Linear:
                    OnEndDragLinear(eventData);
                    break;
            }
        }

#region Step Drag
        public void OnBeginDragStep(PointerEventData eventData)
        {
            _initialPosition = transform.localPosition;
            _initialScreen = views.FirstOrDefault(s => s.transform.position.x > 0 && s.transform.position.x < Screen.width);
        }

        public void OnDragStep(PointerEventData eventData)
        {
            transform.localPosition = new Vector2(transform.localPosition.x + eventData.delta.x, transform.localPosition.y);
        }

        public void OnEndDragStep(PointerEventData eventData)
        {
            var distanceMoved = Mathf.Abs(transform.localPosition.x - _initialPosition.x);

            // Drag를 움직인 길이가 Screen의 40% 이하면
            // 다시 원래 위치로 이동
            if (distanceMoved < 0.4 * Screen.width)
            {
                transform.DOLocalMoveX(_initialPosition.x, 0.5f);
                return;
            }

            // Drag를 움직인 방향을 확인
            // 오른쪽으로 Drag를 움직였으면 이전 Screen으로 이동
            var direction = transform.localPosition.x - _initialPosition.x > 0 ? -1 : 1;
            var nextScreenIndex = views.IndexOf(views.FirstOrDefault(s => s == _initialScreen)) + (int) direction;

            // 다음 Screen이 없으면 원래 위치로 이동
            if (nextScreenIndex < 0 || nextScreenIndex >= views.Count)
            {
                transform.DOLocalMoveX(_initialPosition.x, 0.5f);
                return;
            }

            // 다음 스크린으로 이동
            var nextScreen = views[nextScreenIndex] as MonoBehaviour;
            transform.DOLocalMoveX(GetFixedViewerPositionByIndex(nextScreenIndex).x, 0.5f).OnComplete(() =>
            {
                views.ForEach(s => s.OnScreenExited());
                nextScreen.GetComponent<IView>().OnScreenEntered();
            });
        }
#endregion
        
#region Linear Drag
        public void OnBeginDragLinear(PointerEventData eventData)
        {
            _initialPosition = transform.localPosition;
        }

        public void OnDragLinear(PointerEventData eventData)
        {
            transform.localPosition = new Vector2(transform.localPosition.x + eventData.delta.x, transform.localPosition.y);
        }

        public void OnEndDragLinear(PointerEventData eventData)
        {
            // Drag를 움직인 방향을 확인
            var direction = transform.localPosition.x - _initialPosition.x > 0 ? -1 : 1;

            if (transform.localPosition.x < -Screen.width * (views.Count - 1) / 2)
            {
                transform.DOLocalMoveX(-Screen.width * (views.Count - 1) / 2, 0.5f);
                return;
            }
            else if (transform.localPosition.x > Screen.width * (views.Count - 1) / 2)
            {
                transform.DOLocalMoveX(Screen.width * (views.Count - 1) / 2, 0.5f);
                return;
            }
        }
#endregion

        private Vector2 GetFixedViewerPositionByIndex(int index)
        {
            int viewWidth = Screen.width * views.Count;
            var firstScreenPosition = new Vector2(-(viewWidth / 2) + (Screen.width / 2), 0);
            return -new Vector2(firstScreenPosition.x + (Screen.width * index), 0);
        }

        void Start()
        {
            // player의 위치를 화면(View)의 첫번째 Screen으로 이동한다.
            // player의 크기를 화면(Screen)의 크기 * 화면(Screen)의 갯수로 변경한다.
            int viewerWidth = Screen.width * views.Count;
            int viewerHeight = Screen.height;
            var firstViewPosition = new Vector2(0 - (viewerWidth / 2) + (Screen.width / 2), 0);
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(viewerWidth, viewerHeight);
            rectTransform.localPosition = -firstViewPosition;

            // 각 Screen의 위치를 화면 player의 크기에 따라 연결되게 배치
            for (int i = 0; i < views.Count; i++)
            {
                var view = views[i];
                view.transform.localPosition = new Vector2(firstViewPosition.x + (Screen.width * i), 0);
            }

            if (views.Count > 0)
            {
                views[0].OnScreenEntered();
            }
        }
    }
}
