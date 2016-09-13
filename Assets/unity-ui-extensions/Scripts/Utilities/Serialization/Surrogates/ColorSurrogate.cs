using System.Runtime.Serialization;
using UnityEngine;

namespace Assets.Scripts.Utilities.Serialization.Surrogates
{
    public sealed class ColorSurrogate : ISerializationSurrogate
    {
        // Method called to serialize a Vector3 object
        public void GetObjectData(object obj,
            SerializationInfo info, StreamingContext context)
        {
            var color = (Color) obj;
            info.AddValue("r", color.r);
            info.AddValue("g", color.g);
            info.AddValue("b", color.b);
            info.AddValue("a", color.a);
        }

        // Method called to deserialize a Vector3 object
        public object SetObjectData(object obj,
            SerializationInfo info, StreamingContext context,
            ISurrogateSelector selector)
        {
            var color = (Color) obj;
            color.r = (float) info.GetValue("r", typeof(float));
            color.g = (float) info.GetValue("g", typeof(float));
            color.b = (float) info.GetValue("b", typeof(float));
            color.a = (float) info.GetValue("a", typeof(float));
            obj = color;
            return obj;
        }
    }
}