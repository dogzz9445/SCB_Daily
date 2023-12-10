using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCB.DominantColors
{
    public class DominantColor
    {
        public static Color[] GetDominantColor(Texture texture)
        {
            Color[] pixels = texture.GetPixels();
            List<DataVec> points = new List<DataVec>();
            foreach (Color pixel in pixels)
            {
                points.Add(new ColorVec(pixel));
            }
            KMeansClustering cl = new KMeansClustering(points.ToArray(), 3);
            Cluster[] clusters = cl.Compute();
            List<Color> colors = new List<Color>();
            foreach (Cluster cluster in clusters)
            {
                colors.Add(new Color(cluster.Centroid.Components[0], cluster.Centroid.Components[1], cluster.Centroid.Components[2]));
            }
            return colors.ToArray();
        }
    }
}
