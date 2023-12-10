using UnityEngine;
using UnityEngine.UI;

using KMeans;

namespace SCB.DominantColors
{
    /// <summary>
    /// ColorVec는 CIE 색공간으로 변환하여 색을 비교함
    /// </summary>
    public class ColorVec : DataVec
    {
        public ColorVec(double L, double A, double B) : base(3)
        {
            Components[0] = L;
            Components[1] = A;
            Components[2] = B;
        }

        public override double GetDistance(DataVec other)
        {
            ColorVec otherColor = (ColorVec)other;
            return Mathf.Sqrt(Mathf.Pow((float)(Components[0] - otherColor.Components[0]), 2) + Mathf.Pow((float)(Components[1] - otherColor.Components[1]), 2) + Mathf.Pow((float)(Components[2] - otherColor.Components[2]), 2));
        }
    }
}
