using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace SCB.Shared.UI
{
    public class GradientBackground : MonoBehaviour
    {
        public GameObject Left;
        public GameObject Center;
        public GameObject Right;

        public Color LeftColor;
        public Color RightColor;

        private float ScreenWidth;
        private float ScreenHeight;

        // Start is called before the first frame update
        void Start()
        {
            ScreenWidth = Screen.width;
            ScreenHeight = Screen.height;
            
            Left = transform.Find("Left").gameObject;
            Center = transform.Find("Center").gameObject;
            Right = transform.Find("Right").gameObject;

            GetComponent<RectTransform>().sizeDelta = new Vector2(ScreenWidth * 3, ScreenHeight);
            Left.GetComponent<RectTransform>().sizeDelta = new Vector2(ScreenWidth, ScreenHeight);
            Center.GetComponent<RectTransform>().sizeDelta = new Vector2(ScreenWidth, ScreenHeight);
            Right.GetComponent<RectTransform>().sizeDelta = new Vector2(ScreenWidth, ScreenHeight);

            Left.transform.localPosition = new Vector3(-ScreenWidth, 0, 0);
            Center.transform.localPosition = new Vector3(0, 0, 0);
            Right.transform.localPosition = new Vector3(ScreenWidth, 0, 0);

            SetColor(LeftColor, RightColor);
            SetRight();
        }

        public void SetColor(Color left, Color right)
        {
            LeftColor = new Color(left.r, left.g, left.b, 1.0f);
            RightColor = new Color(right.r, right.g, right.b, 1.0f);;
            Left.GetComponent<UnityEngine.UI.Image>().color = LeftColor;
            Right.GetComponent<UnityEngine.UI.Image>().color = RightColor;
            Center.GetComponent<UnityEngine.UI.Image>().material.SetColor("_Color", LeftColor);
            Center.GetComponent<UnityEngine.UI.Image>().material.SetColor("_Color2", RightColor);
        }

        public void SetRight()
        {
            transform.localPosition = new Vector3(-ScreenWidth, 0, 0);
        }

        public void SetLeft()
        {
            transform.localPosition = new Vector3(ScreenWidth, 0, 0);
        }

        public void SetAlpha0()
        {
            Left.GetComponent<UnityEngine.UI.Image>().color = new Color(LeftColor.r, LeftColor.g, LeftColor.b, 0.0f);
            Right.GetComponent<UnityEngine.UI.Image>().color = new Color(RightColor.r, RightColor.g, RightColor.b, 0.0f);
            Center.GetComponent<UnityEngine.UI.Image>().material.SetColor("_Color", new Color(LeftColor.r, LeftColor.g, LeftColor.b, 0.0f));
            Center.GetComponent<UnityEngine.UI.Image>().material.SetColor("_Color2", new Color(RightColor.r, RightColor.g, RightColor.b, 0.0f));
        }

        public DG.Tweening.Core.TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> FadeIn()
        {
            Left.GetComponent<UnityEngine.UI.Image>().DOFade(1.0f, 0.5f);
            return Right.GetComponent<UnityEngine.UI.Image>().DOFade(1.0f, 0.5f);
        }

        public DG.Tweening.Core.TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> FadeOut()
        {
            Left.GetComponent<UnityEngine.UI.Image>().DOFade(0.0f, 0.5f);
            return Right.GetComponent<UnityEngine.UI.Image>().DOFade(0.0f, 0.5f);
        }

        public DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> MoveRight(float duration = 0.0f, float delay = 0.0f)
        {
            return transform.DOLocalMoveX(-ScreenWidth, duration).SetDelay(delay);
        }

        public DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> MoveLeft(float duration = 0.0f, float delay = 0.0f)
        {
            return transform.DOLocalMoveX(ScreenWidth, duration).SetDelay(delay);
        }
    }
}
