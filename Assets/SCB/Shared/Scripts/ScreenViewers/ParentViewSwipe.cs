using UnityEngine;
using UnityEngine.EventSystems;

namespace SCB.Shared.Viewers
{
    public class ParentViewSwipe : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private IDragHandler _dragHandler;
        public IDragHandler DragHandler => _dragHandler ??= GetComponentInParent<IDragHandler>();

        private IBeginDragHandler _beginDragHandler;
        public IBeginDragHandler BeginDragHandler => _beginDragHandler ??= GetComponentInParent<IBeginDragHandler>();

        private IEndDragHandler _endDragHandler;
        public IEndDragHandler EndDragHandler => _endDragHandler ??= GetComponentInParent<IEndDragHandler>();

        public void OnBeginDrag(PointerEventData eventData)
        {
            BeginDragHandler.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            DragHandler.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EndDragHandler.OnEndDrag(eventData);
        }
    }
}
