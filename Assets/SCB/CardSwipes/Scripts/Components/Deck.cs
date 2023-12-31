using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SCB.CardSwipes
{

    #if UNITY_EDITOR
    [CustomEditor(typeof(Deck))]
    public class DeckGUI : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Only In Editor");

            if (GUILayout.Button("Set All Sections Average Image"))
            {
                var deck = target as Deck;
                deck.SetAllSectionsAverageImage();
            }
            GUILayout.EndVertical();
        }
    }
    #endif


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
        public int DebugSelectedCardIndex = 0;
        public int DebugSelectedSectionIndex = 0;
        public (Card, int) GetCard(int index)
        {
            if (Cards.Count == 0)
                return default;

            if (index < 0)
            {
                index += Cards.Count;
            }
            if (index >= Cards.Count)
            {
                index %= Cards.Count;
            }
            DebugSelectedCardIndex = index;
            return (Cards[index], index);
        }

        public (Section, int) GetSection(int index)
        {
            if (Sections.Count == 0)
                return default;
            int count = countRotation;

            if (index < 0)
            {
                int c = (int)Math.Ceiling(Math.Abs((float)index) / (float)Cards.Count);
                index += Cards.Count * c;
                count -= c;
            }
            if (index >= Cards.Count)
            {
                int c = (int)Math.Floor((float)index / (float)Cards.Count);
                index %= Cards.Count;
                count += c;
            }
            if (count < 0)
            {
                index += count * Cards.Count;
            }
            if (count > 0)
            {
                index -= count * Cards.Count;
            }

            if (index < 0)
            {
                index += Sections.Count * (int)Math.Ceiling(Math.Abs((float)index) / (float)Sections.Count);
            }
            if (index >= Sections.Count)
            {
                index %= Sections.Count;
            }
            DebugSelectedSectionIndex = index;
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

        public int previousTopCardIndex = 0;
        public int deltaTopCardIndex = 0;
        public int countRotation = 0;

        void Update()
        {
            degreeCard = 360f / Cards.Count;
            float degreeClockwise = -transform.localEulerAngles.z;
            degreeTop = degreeClockwise < 0 ? degreeClockwise + 360 : degreeClockwise;
            degreeTop = AddDegreeFit360(degreeTop, 90f);
            topCardIndex = (int)Math.Round(degreeTop / degreeCard);
            if (topCardIndex == Cards.Count)
            {
                topCardIndex = 0;
            }

            deltaTopCardIndex = topCardIndex - previousTopCardIndex;
            previousTopCardIndex = topCardIndex;

            if (deltaTopCardIndex < -Cards.Count / 2)
            {
                countRotation += 1;
            }
            else if (deltaTopCardIndex > Cards.Count / 2)
            {
                countRotation -= 1;
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
 
                Color32[] texColors = texture.GetPixels32();
        
                int total = texColors.Length;
        
                float r = 0;
                float g = 0;
                float b = 0;
        
                for(int i = 0; i < total; i++)
                {
                    r += texColors[i].r;
                    g += texColors[i].g;
                    b += texColors[i].b;
                }
 
                section.ImageColor = new Color32((byte)(r / total) , (byte)(g / total) , (byte)(b / total) , 0xff);
                section.ImageColor = SCB.DominantColors.DominantColor.GetDominantColors(texture).First();
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Done.. Set All Sections Average Image");
        }
#endif
    }
}