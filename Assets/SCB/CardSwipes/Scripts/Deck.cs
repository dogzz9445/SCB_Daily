using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SCB.CardSwipes
{
    public class Deck : MonoBehaviour
    {
        [Header("Required Initial Settings")]
        public GameObject CardPrefab;
        public int NumCards = 30;
        public float CardWidth = 150;
        public float DeckRadius = 1200.0f;
        public float DeckPositionY = -1200;

        private List<GameObject> Cards = new List<GameObject>();


        [Header("Debug Only")]
        public bool IsRotation = false;
        public float RotationSpeed = 0.5f;
        public bool IsWireFrame = false;


        private void Start()
        {
            if (CardPrefab != null)
            {
                for (int i = 0; i < NumCards; i++)
                {
                    var card = GameObject.Instantiate(CardPrefab, transform);
                    Cards.Add(card);
                }
            }
        }

        [Header("Debug Only")]
        public float selectionDegree = 0;
        public float angle360 = 0;
        public int selectedCard = 0;

        void Update()
        {
            #if UNITY_EDITOR
            if (IsRotation)
            {
                transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z + RotationSpeed);
            }
            selectionDegree = 360f / Cards.Count;
            float ang = -transform.localEulerAngles.z;
            angle360 = ang < 0 ? ang + 360 : ang;
            selectedCard = (int)(angle360 / selectionDegree);

            for (int i = 0; i < Cards.Count; i++)
            {
                if (i == selectedCard)
                {
                    Cards[i].GetComponent<Image>().color = Color.red;
                    Cards[i].transform.SetAsLastSibling();
                }
                else
                {
                    Cards[i].GetComponent<Image>().color = Color.white;
                }
                float angle = i * Mathf.PI * 2 / Cards.Count;
                float degree = angle * 180 / Mathf.PI;
                float x = DeckRadius * Mathf.Cos(angle);
                float y = DeckRadius * Mathf.Sin(angle);
                Cards[i].GetComponent<RectTransform>().sizeDelta = new Vector2(CardWidth, CardWidth);
                Cards[i].transform.localPosition = new Vector3(x, y, 0);
                Cards[i].transform.localEulerAngles = new Vector3(0, 0, degree - 90f);
            }
            #endif

        }
    }
}