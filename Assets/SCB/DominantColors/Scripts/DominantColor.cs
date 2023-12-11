using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using KMeans;
using SCB.ColorSpaces;

// https://github.com/indragiek/DominantColor?tab=readme-ov-file 를 참고함
// CIE 색공간으로 변환하여 색을 비교함
// https://en.wikipedia.org/wiki/CIE_1931_color_space
// https://en.wikipedia.org/wiki/CIELAB_color_space
// KMeans 클러스터링을 사용하여 색을 분류함
// https://en.wikipedia.org/wiki/K-means_clustering
// KMeans 계산 시 Distance 계산을 위해 CIE76 공식을 사용함

namespace SCB.DominantColors
{
    public class DominantColor
    {
        public static Color[] GetDominantColors(Texture2D texture)
        {
            Color[] pixels = texture.GetPixels();
            List<DataVec> points = new List<DataVec>();
            foreach (Color pixel in pixels)
            {
                var lab = ColorConversion.RGBtoLab((int)(pixel.r * 255), (int)(pixel.g * 255), (int)(pixel.b * 255));
                points.Add(new ColorVec(lab.L, lab.A, lab.B));
            }
            KMeansClustering cl = new KMeansClustering(points.ToArray(), 3);
            Cluster[] clusters = cl.Compute();
            List<Color> colors = new List<Color>();
            foreach (Cluster cluster in clusters)
            {
                var rgb = ColorConversion.LabtoRGB(cluster.Centroid.Components[0], cluster.Centroid.Components[1], cluster.Centroid.Components[2]);
                colors.Add(new Color(rgb.Red / 255f, rgb.Green / 255f, rgb.Blue / 255f));
            }
            return colors.ToArray();
        }
    }
}
