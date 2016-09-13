///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/

using UnityEngine;

namespace Assets.Scripts.HSVPicker
{
    public class ColorPickerTester : MonoBehaviour
    {
        public HSVPicker picker;
        public Renderer pickerRenderer;

        private void Awake()
        {
            pickerRenderer = GetComponent<Renderer>();
        }

        // Use this for initialization
        private void Start()
        {
            picker.onValueChanged.AddListener(color => { pickerRenderer.material.color = color; });
        }
    }
}