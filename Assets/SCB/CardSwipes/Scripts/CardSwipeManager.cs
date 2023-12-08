using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using SCB.Cores;
using SCB.Shared.UI;
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

        public GradientBackground gradientBackground;
        public GameObject Background;

        [Header("Initial Settings")]
        public float MaxCardHeight = 624f;
        public float DeckRadius = 1156.0f;
        public float DeckGlobalPositionY = -600.0f;
        [Range(0.0f, 3.0f)]
        public float CardSizeRatio = 0.8f;

        public GameObject SectionRoot;
        public Section selectedSection;
        public GameObject goSection;

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

            selectedSection = popupCard.GetComponent<Card>().section;


            // 카드를 중앙으로 움직임
            popupCard.GetComponent<Card>().text.DOColor(Color.clear, 0.5f);
            popupCard.transform
                .DOLocalRotate(new Vector3(0.0f, 0.0f, 0.0f), 1.0f)
                .SetEase(Ease.OutCubic);
            popupCard.transform
                .DOMove(new Vector3(Screen.width / 2, Screen.height / 2, 0), 1.0f)
                .SetEase(Ease.OutCubic);
            popupCard.transform
                .GetComponent<RectTransform>()
                .DOSizeDelta(new Vector3(0, Screen.height * 0.640614f), 1.0f);

            // 카드를 삭제함
            popupCard
                .GetComponent<Card>()
                .cardImageShadow
                    .DOColor(new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.4f)
                    .SetEase(Ease.OutCubic)
                    .SetDelay(2.0f);
            popupCard
                .GetComponent<Card>()
                .cardImage
                    .DOColor(new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.5f)
                    .SetDelay(2.0f)
                    .OnComplete(() =>
                    Destroy(popupCard));

            gradientBackground.SetColor(selectedSection.ImageColor, Color.white);
            gradientBackground.MoveLeft(1.0f, 0.1f).OnComplete(() =>
            {
                StartCoroutine(InstantiateSection(1.4f));
            });
        }

        public void ClearCard()
        {
            if (goSection)
            {
                Destroy(goSection);
            }
        }

        public IEnumerator InstantiateSection(float delay = 0.0f)
        {
            yield return new WaitForSeconds(delay);
            if (selectedSection.Prefab)
            {
                goSection = Instantiate(selectedSection.Prefab, SectionRoot.transform);
            }
        }
    }
}