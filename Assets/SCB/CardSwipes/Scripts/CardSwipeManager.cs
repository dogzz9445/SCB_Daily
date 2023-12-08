using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using SCB.Cores;
using UnityEngine;
using UnityEngine.UI;

namespace SCB.CardSwipes
{
    public class CardSwipeManager : AbstractSingleton<CardSwipeManager>
    {
        [HideInInspector]
        public bool isShowingDeck = false;

        public GameObject deck;
        public GameObject hideShowButton;
        public GameObject hideShowButtonShadow;

        [Header("Initial Settings")]
        public float MaxCardHeight = 624f;
        public float DeckRadius = 1156.0f;
        public float DeckGlobalPositionY = -600.0f;
        [Range(0.0f, 3.0f)]
        public float CardSizeRatio = 0.8f;

        private void Start()
        {
            deck.transform.localPosition = new Vector3(0, -DeckRadius - MaxCardHeight, 0);
            hideShowButton.transform.localEulerAngles = new Vector3(0, 0, 180);
            hideShowButtonShadow.transform.localEulerAngles = new Vector3(0, 0, 180);
            hideShowButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (isShowingDeck)
                {
                    HideDeck();
                }
                else
                {
                    ShowDeck();
                }
            });
        }


        public void HideDeck()
        {
            if (!isShowingDeck)
                return;
            isShowingDeck = false;
            
            deck.transform.DOLocalMoveY(-DeckRadius - MaxCardHeight, 1.0f);
            hideShowButton.transform.DOLocalRotate(new Vector3(0, 0, 180), 0.2f);
            hideShowButtonShadow.transform.DOLocalRotate(new Vector3(0, 0, 180), 0.2f);
        }

        public void ShowDeck()
        {
            if (isShowingDeck)
                return;
            isShowingDeck = true;

            deck.transform.DOLocalMoveY(DeckGlobalPositionY, 1.0f);
            hideShowButton.transform.DOLocalRotate(new Vector3(0, 0, 360), 0.2f);
            hideShowButtonShadow.transform.DOLocalRotate(new Vector3(0, 0, 360), 0.2f);
        }

        public void ShowCardDetail(int cardIndex)
        {
            GameObject originalCard = deck.GetComponent<Deck>().GetCard(cardIndex).Item1.gameObject;
            var popupCard = Instantiate(originalCard);
            popupCard.transform.position = originalCard.transform.position;
            popupCard.transform.rotation = originalCard.transform.rotation;
            popupCard.transform.SetParent(transform.parent);
            popupCard.GetComponent<Card>().isMovingToCenter = true;

            popupCard.transform
                .DOLocalRotate(new Vector3(0.0f, 0.0f, 0.0f), 1.0f)
                .SetEase(Ease.OutCubic);
            popupCard.transform
                .DOMove(new Vector3(Screen.width / 2, Screen.height / 2, 0), 1.0f)
                .SetEase(Ease.OutCubic);
            popupCard.transform
                .GetComponent<RectTransform>()
                .DOSizeDelta(new Vector3(0, Screen.height * 0.640614f), 1.0f);
            StartCoroutine(DestroyCard(popupCard, 2.0f));
        }

        public IEnumerator DestroyCard(GameObject card, float delay = 0.0f)
        {
            yield return new WaitForSeconds(delay);
            Destroy(card);
        }
    }
}