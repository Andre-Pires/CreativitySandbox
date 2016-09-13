///Credit perchik
///Sourced from - http://forum.unity3d.com/threads/receive-onclick-event-and-pass-it-on-to-lower-ui-elements.293642/

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ComboBox
{
    [RequireComponent(typeof(RectTransform), typeof(Button))]
    public class DropDownListButton
    {
        public Button btn;
        public Image btnImg;
        public GameObject gameobject;
        public Image img;
        public RectTransform rectTransform;
        public Text txt;

        public DropDownListButton(GameObject btnObj)
        {
            gameobject = btnObj;
            rectTransform = btnObj.GetComponent<RectTransform>();
            btnImg = btnObj.GetComponent<Image>();
            btn = btnObj.GetComponent<Button>();
            txt = rectTransform.FindChild("Text").GetComponent<Text>();
            img = rectTransform.FindChild("Image").GetComponent<Image>();
        }
    }
}