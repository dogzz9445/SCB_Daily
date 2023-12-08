using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SCB.CardSwipes
{
    public class Card : MonoBehaviour, 
        IDragHandler, 
        IBeginDragHandler, 
        IEndDragHandler, 
        IPointerUpHandler,
        IPointerDownHandler,
        IPointerClickHandler
    {
        public int Index;
        public bool IsSelected;
        public float MaxHeight;
        public float Height;

        private Deck deck;

        public Image cardImage;
        public Image cardImageShadow;

        public Section section;

        public RectTransform rect;

        public TMP_Text text;

        public bool IsWireFrame { get; internal set; }

        public bool IsClicked { get; internal set; }

        [HideInInspector]
        public bool isMovingToCenter = false;

        #region Drag Handlers
        private void Start()
        {
            deck = GetComponentInParent<Deck>();
            rect = GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, MaxHeight);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            deck.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            deck.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            deck.OnEndDrag(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isMovingToCenter)
                return;

            if (eventData.dragging)
            {
                return;
            }
            IsClicked = true;
            rect.DOSizeDelta(new Vector2(0, MaxHeight+50), 0.5f)
            .OnComplete(() => 
            rect.DOSizeDelta(new Vector2(0, Height), 0.5f)
            .OnComplete(() => IsClicked = false));

            deck.OnClickCard(eventData, Index);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }

        public void SetSection()
        {
            cardImage.sprite = section.Image;
            cardImage.color = Color.white;
        }

        private void Update()
        {
            if (isMovingToCenter)
                return;

            // 카드 크기 조절
            if (IsSelected)
            {
                Height = MaxHeight;
            }
            else
            {
                // var ratio = Mathf.Abs(transform.localPosition.x) / 
                //  + Mathf.Abs(transform.localPosition.y) / (Screen.height / 2);
                float ratioY = 
                    (CardSwipeManager.Instance.DeckGlobalPositionY + CardSwipeManager.Instance.DeckRadius - transform.position.y) 
                    / (CardSwipeManager.Instance.DeckRadius * 2)
                    * CardSwipeManager.Instance.CardSizeRatio;
                ratioY = 1 - ratioY;

                float ratioX =
                    MathF.Abs((Screen.width / 2) - transform.position.x) 
                    / (Screen.width / 2) 
                    * CardSwipeManager.Instance.CardSizeRatio;
                ratioX = 1 - ratioX;

                Height = MaxHeight * ratioX * ratioY;
            }
            if (!IsClicked)
            {
                rect.sizeDelta = new Vector2(0, Height);
            }
            else
            {
                transform.SetAsLastSibling();
            }

            if (section != null)
            {
                GetComponent<AspectRatioFitter>().aspectRatio = section.Image.textureRect.width / section.Image.textureRect.height;
                cardImage.sprite = section.Image;
                cardImage.color = Color.white;
                if (text != null)
                {
                    text.text = section.Description;
                }
            }
        }
        #endregion
    }
}
