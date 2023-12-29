using UnityEngine;
using UnityEngine.Events;

namespace SCB.Shared.Viewers
{
    public class View : MonoBehaviour, IView
    {
        public UnityEvent onScreenEntered = new UnityEvent();
        public UnityEvent onScreenExited = new UnityEvent();

        public virtual void OnScreenEntered()
        {
            onScreenEntered.Invoke();
        }

        public virtual void OnScreenExited()
        {
            onScreenExited.Invoke();
        }
    }
}
