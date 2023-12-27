using UnityEngine;

namespace SCB.Shared.Viewers
{
    public class View : MonoBehaviour, IView
    {
        public virtual void OnScreenEntered()
        {
        }

        public virtual void OnScreenExited()
        {
        }
    }
}
