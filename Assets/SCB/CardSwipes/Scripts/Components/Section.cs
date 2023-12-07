using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SCB.CardSwipes
{
    [CreateAssetMenu(fileName = "Section", menuName = "CardSwipes/Crate Section")]
    [System.Serializable]
    public class Section : ScriptableObject
    {
        public string Name;
        public string Description;
        public Sprite Image;
        public GameObject Prefab;
        public Color ImageColor;
    }
}