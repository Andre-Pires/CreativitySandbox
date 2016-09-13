/// Original Credit Korindian
/// Sourced from - http://forum.unity3d.com/threads/rts-style-drag-selection-box.265739/
/// Updated Credit BenZed
/// Sourced from - http://forum.unity3d.com/threads/color-picker.267043/


using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.SelectionBox
{
    public class ExampleSelectable : MonoBehaviour, IBoxSelectable
    {
        private Image image;

        //We want the test object to be either a UI element, a 2D element or a 3D element, so we'll get the appropriate components
        private SpriteRenderer spriteRenderer;
        private Text text;

        private void Start()
        {
            spriteRenderer = transform.GetComponent<SpriteRenderer>();
            image = transform.GetComponent<Image>();
            text = transform.GetComponent<Text>();
        }

        private void Update()
        {
            //What the game object does with the knowledge that it is selected is entirely up to it.
            //In this case we're just going to change the color.

            //White if deselected.
            var color = Color.white;

            if (preSelected)
            {
                //Yellow if preselected
                color = Color.yellow;
            }
            if (selected)
            {
                //And green if selected.
                color = Color.green;
            }

            //Set the color depending on what the game object has.
            if (spriteRenderer)
            {
                spriteRenderer.color = color;
            }
            else if (text)
            {
                text.color = color;
            }
            else if (image)
            {
                image.color = color;
            }
            else if (GetComponent<Renderer>())
            {
                GetComponent<Renderer>().material.color = color;
            }
        }

        #region Implemented members of IBoxSelectable

        public bool selected { get; set; }

        public ExampleSelectable()
        {
            preSelected = false;
            selected = false;
        }

        public bool preSelected { get; set; }

        #endregion
    }
}