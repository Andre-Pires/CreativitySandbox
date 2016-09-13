///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/

using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.HSVPicker
{
    public class HsvSliderPicker : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        public HSVPicker picker;


        public void OnDrag(PointerEventData eventData)
        {
            PlacePointer(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PlacePointer(eventData);
        }

        private void PlacePointer(PointerEventData eventData)
        {
            var pos = new Vector2(eventData.position.x - picker.hsvSlider.rectTransform.position.x,
                picker.hsvSlider.rectTransform.position.y - eventData.position.y);

            pos.y /= picker.hsvSlider.rectTransform.rect.height*picker.hsvSlider.canvas.transform.lossyScale.y;

            //Debug.Log(eventData.position.ToString() + " " + picker.hsvSlider.rectTransform.position + " " + picker.hsvSlider.rectTransform.rect.height);
            pos.y = Mathf.Clamp(pos.y, 0, 1f);

            picker.MovePointer(pos.y);
        }
    }
}