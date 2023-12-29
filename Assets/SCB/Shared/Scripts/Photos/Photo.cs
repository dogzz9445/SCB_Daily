using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCB.PhotoViewers
{
    [CreateAssetMenu(fileName = "Photo", menuName = "SCB/PhotoViewer/Photo", order = 1)]
    [Serializable]
    public class Photo : ScriptableObject
    {
        public Texture2D texture;
        public string title;
        public string description;
    }
}
