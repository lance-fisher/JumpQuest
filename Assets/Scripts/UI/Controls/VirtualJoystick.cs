using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JumpQuest.UI
{
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("References")]
        public RectTransform Background;
        public RectTransform Handle;

        [Header("Settings")]
        public float HandleRange = 60f;
        public float DeadZone = 0.15f;

        private Vector2 input = Vector2.zero;
        private Canvas canvas;
        private Camera cam;

        public Vector2 Direction => input;

        private void Start()
        {
            canvas = GetComponentInParent<Canvas>();
            cam = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Background, eventData.position, cam, out pos);

            pos = pos / (Background.sizeDelta / 2f);
            input = new Vector2(pos.x, pos.y);

            if (input.magnitude > 1f)
                input = input.normalized;

            if (input.magnitude < DeadZone)
                input = Vector2.zero;

            Handle.anchoredPosition = input * HandleRange;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            input = Vector2.zero;
            Handle.anchoredPosition = Vector2.zero;
        }
    }
}
