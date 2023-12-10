using UnityEngine;
using UnityEngine.UI;

using KMeans;

namespace SCB.DominantColors
{
    public class ColorVec : DataVec
    {
        public ColorVec(Color color) : base(3)
        {
            Components[0] = color.r;
            Components[1] = color.g;
            Components[2] = color.b;
        }

        public ColorVec(float r, float g, float b) : base(3)
        {
            Components[0] = r;
            Components[1] = g;
            Components[2] = b;
        }
    }
}
