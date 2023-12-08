using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;




#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SCB.CardSwipes
{
    public class Deck : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [Header("Required Initial Settings")]
        public GameObject CardPrefab;
        public int NumCards = 30;
        [HideInInspector]
        public float DeckRadius = 1156.0f;

        private List<Card> Cards = new List<Card>();

        [Header("Section Information")]
        public List<Section> Sections = new List<Section>();

        private void Start()
        {
            if (CardPrefab == null)
                return;

            DeckRadius = CardSwipeManager.Instance.DeckRadius;
            transform.localEulerAngles = new Vector3(0, 0, 90f);

            // NumCard에 따라 Card를 생성하고 초기 세팅 값을 설정
            for (int i = 0; i < NumCards; i++)
            {
                var card = GameObject.Instantiate(CardPrefab, transform);
                var cardComponent = card.GetComponent<Card>();
                if (cardComponent != null)
                {
                    cardComponent.MaxHeight = CardSwipeManager.Instance.MaxCardHeight;
                    cardComponent.Index = i;
                    Cards.Add(cardComponent);
                }
            }
            ReorderCards(0);
        }

        private void ReorderCards(int index)
        {
            for (int i = index; i <= index + (Cards.Count / 2) + 1; i++)
            {
                GetCard(i).Item1.transform.SetAsFirstSibling();
            }
            for (int i = index; i >= index - (Cards.Count / 2); i--)
            {
                GetCard(i).Item1.transform.SetAsFirstSibling();
            }
        }

        public (Card, int) GetCard(int index)
        {
            if (Cards.Count == 0)
                return default;

            if (index >= Cards.Count)
            {
                index %= Cards.Count;
            }
            if (index < 0)
            {
                index = Cards.Count + (index % Cards.Count);
                if (index == Cards.Count)
                {
                    index = 0;
                }
            }
            return (Cards[index], index);
        }

        public (Section, int) GetSection(int index)
        {
            if (Sections.Count == 0)
                return default;

            if (index >= Sections.Count)
            {
                index %= Sections.Count;
            }
            if (index < 0)
            {
                index = Sections.Count + (index % Sections.Count);
                if (index == Sections.Count)
                {
                    index = 0;
                }
            }
            return (Sections[index], index);
        }

        [Header("Debug Only")]
        public float degreeCard = 0;
        private float degreeTop = 0;
        private float degreeBottom = 0;
        public int topCardIndex = 0;

        private float _dragInitialPositionY;
        private float _dragVelocity;

        private bool isSelecting;

        void Update()
        {
            // Refresh Debugging Settings
            // foreach (var card in Cards)
            // {
            //     card.MaxWidth = CardWidth;
            //     card.IsWireFrame = IsWireFrame;
            // }

            degreeCard = 360f / Cards.Count;
            float degreeClockwise = -transform.localEulerAngles.z;
            degreeTop = degreeClockwise < 0 ? degreeClockwise + 360 : degreeClockwise;
            degreeTop = AddDegreeFit360(degreeTop, 90f);
            topCardIndex = (int)Math.Round(degreeTop / degreeCard);
            if (topCardIndex == Cards.Count)
            {
                topCardIndex = 0;
            }

            degreeBottom = degreeClockwise < 0 ? degreeClockwise + 360 : degreeClockwise;
            degreeBottom = AddDegreeFit360(degreeBottom, -90f);
            float bottomCardIndex = (int)Math.Round(degreeBottom / degreeCard);
            if (bottomCardIndex == Cards.Count)
            {
                bottomCardIndex = 0;
            }

            for (int i = 0; i < Cards.Count; i++)
            {
                // Card들의 보이는 순서 조정
                if (!IsClickingOnCard)
                {
                    if (i == topCardIndex)
                    {
                        Cards[i].transform.SetAsLastSibling();
                    }
                    else
                    {
                        if (bottomCardIndex == i)
                        {
                            Cards[i].transform.SetAsFirstSibling();
                        }
                    }
                }

                // Card 위치 조정
                float angle = i * Mathf.PI * 2 / Cards.Count;
                float degree = angle * 180 / Mathf.PI;
                float x = DeckRadius * Mathf.Cos(angle);
                float y = DeckRadius * Mathf.Sin(angle);
                Cards[i].transform.localPosition = new Vector3(x, y, 0);
                Cards[i].transform.localEulerAngles = new Vector3(0, 0, degree - 90f);
            }

            // Card에 Section 적용
            if (Sections.Count > 7)
            {
            }
            for (int i = topCardIndex - 4; i <= topCardIndex + 4; i++)
            {
                GetCard(i).Item1.section = GetSection(i).Item1;
            }
        }

        private float AddDegreeFit360(float degree, float add)
        {
            degree += add;
            if (degree < 0)
            {
                return degree + 360;
            }
            else if (degree > 360)
            {
                return degree - 360;
            }
            else
            {
                return degree;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            var deltaDegree = Mathf.Atan2(eventData.delta.x, _dragInitialPositionY-transform.position.y) * 180 / Mathf.PI;
            transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z - deltaDegree);
            _dragVelocity = deltaDegree;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragInitialPositionY = eventData.position.y;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform
                .DOLocalRotate(new Vector3(0, 0, transform.localEulerAngles.z - (_dragVelocity * 2)), 0.5f)
                .SetEase(Ease.OutCubic);
                // .OnStart(() => _dragVelocity = 0);
        }

        private bool IsClickingOnCard = false;
        public void OnClickCard(PointerEventData eventData, int cardIndex)
        {
            IsClickingOnCard = true;
            float degree = ((float)cardIndex / (float)Cards.Count) * 360;
            ReorderCards(cardIndex);
            transform
                .DORotate(new Vector3(0, 0, (-degree) + 90), 1.0f)
                .OnComplete(() => IsClickingOnCard = false);

            if (topCardIndex != cardIndex)
                return;

            CardSwipeManager.Instance.HideDeck();
            CardSwipeManager.Instance.ShowCardDetail(cardIndex);
            GetCard(cardIndex).Item1.gameObject.SetActive(false);
            StartCoroutine(ResetActive(cardIndex));
        }

        private IEnumerator ResetActive(int cardIndex)
        {
            yield return new WaitForSeconds(1.0f);
            GetCard(cardIndex).Item1.gameObject.SetActive(true);
        }

        public void Select(int cardIndex)
        {
            if (cardIndex < 0 || cardIndex >= Cards.Count)
            {
                return;
            }
            var card = Cards[cardIndex];
            card.transform.SetAsLastSibling();
            card.IsSelected = true;
        }

#if UNITY_EDITOR
        public void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                SetAllSectionsAverageImage();
            }
        }

        public void SetAllSectionsAverageImage()
        {
            foreach (var section in Sections)
            {
                if (section.Image == null)
                    continue;
                var texture = section.Image.texture;
                var colors = texture.GetPixels();
                var averageColor = new Color();
                foreach (var color in colors)
                {
                    averageColor += color;
                }
                averageColor /= colors.Length;
                section.ImageColor = averageColor;
            }
            EditorUtility.SetDirty(this);
        }
#endif
    }
}