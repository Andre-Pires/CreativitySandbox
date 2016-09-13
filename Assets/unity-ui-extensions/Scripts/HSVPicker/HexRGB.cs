///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/

using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HSVPicker
{
    public class HexRGB : MonoBehaviour
    {
        // Unity 5.1/2 needs an InputFiled vs grabbing the text component
        public InputField hexInput;

        public HSVPicker hsvpicker;

        public void ManipulateViaRGB2Hex()
        {
            var color = hsvpicker.currentColor;
            var hex = ColorToHex(color);
            hexInput.text = hex;
        }

        public static string ColorToHex(Color color)
        {
            var r = (int) (color.r*255);
            var g = (int) (color.g*255);
            var b = (int) (color.b*255);
            return string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
        }

        public void ManipulateViaHex2RGB()
        {
            var hex = hexInput.text;
            var rgb = Hex2RGB(hex);
            var color = NormalizeVector4(rgb, 255f, 1f);
            print(rgb);

            hsvpicker.AssignColor(color);
        }

        private static Color NormalizeVector4(Vector3 v, float r, float a)
        {
            var red = v.x/r;
            var green = v.y/r;
            var blue = v.z/r;
            return new Color(red, green, blue, a);
        }

        private Vector3 Hex2RGB(string hexColor)
        {
            //Remove # if present
            if (hexColor.IndexOf('#') != -1)
                hexColor = hexColor.Replace("#", "");

            var red = 0;
            var green = 0;
            var blue = 0;

            if (hexColor.Length == 6)
            {
                //#RRGGBB
                red = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                green = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                blue = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
            }
            else if (hexColor.Length == 3)
            {
                //#RGB
                red = int.Parse(hexColor[0] + hexColor[0].ToString(), NumberStyles.AllowHexSpecifier);
                green = int.Parse(hexColor[1] + hexColor[1].ToString(), NumberStyles.AllowHexSpecifier);
                blue = int.Parse(hexColor[2] + hexColor[2].ToString(), NumberStyles.AllowHexSpecifier);
            }

            return new Vector3(red, green, blue);
        }
    }
}