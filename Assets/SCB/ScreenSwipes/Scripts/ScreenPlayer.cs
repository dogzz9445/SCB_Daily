using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCB.ScreenSwipes
{
    public class ScreenPlayer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private Vector3 _initialPosition;
        private ScreenContent _initialScreen;
        public List<ScreenContent> screens = new List<ScreenContent>();

        public void OnBeginDrag(PointerEventData eventData)
        {
            _initialPosition = transform.localPosition;
            _initialScreen = screens.FirstOrDefault(s => s.transform.position.x > 0 && s.transform.position.x < Screen.width);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.localPosition = new Vector2(transform.localPosition.x+eventData.delta.x, transform.localPosition.y);
        }

        // Drag가 끝나면
        public void OnEndDrag(PointerEventData eventData)
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
            var nextScreenIndex = screens.IndexOf(screens.FirstOrDefault(s => s == _initialScreen)) + (int)direction;

            // 다음 Screen이 없으면 원래 위치로 이동
            if (nextScreenIndex < 0 || nextScreenIndex >= screens.Count)
            {
                transform.DOLocalMoveX(_initialPosition.x, 0.5f);
                return;
            }

            // 다음 스크린으로 이동
            var nextScreen = screens[nextScreenIndex] as MonoBehaviour;
            transform.DOLocalMoveX(GetScreenPositionForPlayerLocalPosition(nextScreenIndex).x, 0.5f).OnComplete(() =>
            {
                screens.ForEach(s => s.OnScreenExited());
                nextScreen.GetComponent<IScreen>().OnScreenEntered();
            });

            // if (distanceMoved > 0)
            // {
            //     var direction = Mathf.Sign(transform.localPosition.x - _initialPosition.x);
            //     var nextScreenIndex = screens.IndexOf(screens.First(s => s as MonoBehaviour == this)) + (int)direction;
            //     if (nextScreenIndex >= 0 && nextScreenIndex < screens.Count)
            //     {
            //         var nextScreen = screens[nextScreenIndex] as MonoBehaviour;
            //         nextScreen.transform.localPosition = new Vector3(Screen.width * -direction, 0, 0);
            //         nextScreen.transform.DOLocalMoveX(0, 0.5f);
            //         transform.DOLocalMoveX(Screen.width * direction, 0.5f).OnComplete(() =>
            //         {
            //             nextScreen.transform.localPosition = new Vector3(0, 0, 0);
            //             transform.localPosition = new Vector3(Screen.width * -direction, 0, 0);
            //             screens.ForEach(s => s.OnScreenExited());
            //             nextScreen.GetComponent<IScreen>().OnScreenEntered();
            //         });
            //     }
            // }
            // // 왼쪽으로 Drag를 움직였으면 이전 Screen으로 이동한다.
            // else if (distanceMoved < 0)
            // {
            //     var direction = Mathf.Sign(transform.localPosition.x - _initialPosition.x);
            //     var nextScreenIndex = screens.IndexOf(screens.First(s => s as MonoBehaviour == this)) + (int)direction;
            //     if (nextScreenIndex >= 0 && nextScreenIndex < screens.Count)
            //     {
            //         var nextScreen = screens[nextScreenIndex] as MonoBehaviour;
            //         nextScreen.transform.localPosition = new Vector3(Screen.width * -direction, 0, 0);
            //         nextScreen.transform.DOLocalMoveX(0, 0.5f);
            //         transform.DOLocalMoveX(Screen.width * direction, 0.5f).OnComplete(() =>
            //         {
            //             nextScreen.transform.localPosition = new Vector3(0, 0, 0);
            //             transform.localPosition = new Vector3(Screen.width * -direction, 0, 0);
            //             screens.ForEach(s => s.OnScreenExited());
            //             nextScreen.GetComponent<IScreen>().OnScreenEntered();
            //         });
            //     }
            // }
        }

        private Vector2 GetScreenPositionForPlayerLocalPosition(int index)
        {
            int playerWidth = Screen.width * screens.Count;
            var firstScreenPosition = new Vector2(-(playerWidth / 2) + (Screen.width / 2), 0);
            return -new Vector2(firstScreenPosition.x + (Screen.width * index), 0);
        }

        // Start is called before the first frame update
        void Start()
        {
            // player의 위치를 화면(View)의 첫번째 Screen으로 이동한다.
            // player의 크기를 화면(Screen)의 크기 * 화면(Screen)의 갯수 로 변경한다.
            int playerWidth = Screen.width * screens.Count;
            int playerHeight = Screen.height;
            var firstScreenPosition = new Vector2(-(playerWidth / 2) + (Screen.width / 2), 0);
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(playerWidth, playerHeight);
            rectTransform.localPosition = -firstScreenPosition;

            // 각 Screen의 위치를 player의 크기에 따라 연결되게 배치
            for (int i = 0; i < screens.Count; i++)
            {
                var screen = screens[i];
                var screenTransform = screen as MonoBehaviour;
                screenTransform.transform.localPosition = 
                    new Vector3(firstScreenPosition.x + (Screen.width * i), 0, 0);
            }

            if (screens.Count > 0)
            {
                screens[0].OnScreenEntered();
            }
        }
    }
}
