///Credit Martin Nerurkar // www.martin.nerurkar.de // www.sharkbombs.com
///Sourced from - http://www.sharkbombs.com/2015/02/10/tooltips-with-the-new-unity-ui-ugui/

using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.ToolTips.BoundTooltip
{
    [AddComponentMenu("UI/Extensions/Bound Tooltip/Tooltip Trigger")]
    public class BoundTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler,
        IDeselectHandler
    {
        public Vector3 offset;

        [TextArea] public string text;

        public bool useMousePosition = false;

        public void OnDeselect(BaseEventData eventData)
        {
            StopHover();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (useMousePosition)
            {
                StartHover(new Vector3(eventData.position.x, eventData.position.y, 0f));
            }
            else
            {
                StartHover(transform.position + offset);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopHover();
        }

        public void OnSelect(BaseEventData eventData)
        {
            StartHover(transform.position);
        }

        private void StartHover(Vector3 position)
        {
            BoundTooltipItem.Instance.ShowTooltip(text, position);
        }

        private void StopHover()
        {
            BoundTooltipItem.Instance.HideTooltip();
        }
    }
}