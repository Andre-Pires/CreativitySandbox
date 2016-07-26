///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/

using UnityEngine;

namespace Assets.Scripts.HSVPicker
{
    public class ColorPickerTester : MonoBehaviour
    {
        public Renderer pickerRenderer;
        public HSVPicker picker;

        void Awake()
        {
            pickerRenderer = GetComponent<Renderer>();
        }
        // Use this for initialization
        void Start()
        {
            picker.onValueChanged.AddListener(color =>
            {
                pickerRenderer.material.color = color;
            });
        }
    }
}