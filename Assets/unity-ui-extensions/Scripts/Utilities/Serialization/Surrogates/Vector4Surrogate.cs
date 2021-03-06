using System.Runtime.Serialization;
using UnityEngine;

namespace Assets.Scripts.Utilities.Serialization.Surrogates
{
    public sealed class Vector4Surrogate : ISerializationSurrogate
    {
        // Method called to serialize a Vector4 object
        public void GetObjectData(object obj,
            SerializationInfo info, StreamingContext context)
        {
            var v4 = (Vector4) obj;
            info.AddValue("x", v4.x);
            info.AddValue("y", v4.y);
            info.AddValue("w", v4.w);
            info.AddValue("z", v4.z);
        }

        // Method called to deserialize a Vector4 object
        public object SetObjectData(object obj,
            SerializationInfo info, StreamingContext context,
            ISurrogateSelector selector)
        {
            var v4 = (Vector4) obj;
            v4.x = (float) info.GetValue("x", typeof(float));
            v4.y = (float) info.GetValue("y", typeof(float));
            v4.w = (float) info.GetValue("w", typeof(float));
            v4.z = (float) info.GetValue("z", typeof(float));
            obj = v4;
            return obj;
        }
    }
}