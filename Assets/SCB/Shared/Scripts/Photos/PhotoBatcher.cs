using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCB.Shared.Components
{
    [ExecuteAlways]
    public class PhotoBatcher : MonoBehaviour
    {
        public List<Image> images = new List<Image>();

        public int Margin = 0;
        public int BatcherWidth = 0;
        public int NumColumn = 4;
        public bool ApplyingOriginalRatio = true;

        private void Update()
        {
            if (images.Count == 0)
                return;

            int accumulatedHeight = 0;
            for (int i = 0; i < Math.Floor((float)images.Count / NumColumn); i++)
            {
                int maxHeightInRow = 0;
                for (int j = 0; j < NumColumn; j++)
                {
                    var index = i * NumColumn + j;
                    if (index >= images.Count)
                        break;
                    
                    if (!images[index] || images[index] == null)
                        break;

                    var image = images[index];
                    
                    var imageWidth = images[0].mainTexture.width;
                    var imageHeight = images[0].mainTexture.height;
                    var ratio = (float)imageWidth / imageHeight;
                    var width = BatcherWidth / NumColumn;
                    var height = width / ratio;

                    if (ApplyingOriginalRatio)
                    {
                        if (height > maxHeightInRow)
                            maxHeightInRow = (int)height;
                    }
                    else
                    {
                        maxHeightInRow = width;
                    }

                    var rectTransform = image.GetComponent<RectTransform>();
                    rectTransform.localPosition = new Vector2(j * (width + Margin), -i * Margin - accumulatedHeight);
                    rectTransform.sizeDelta = new Vector2(width, height);
                }
                accumulatedHeight += maxHeightInRow;
            }
        }
    }
}
