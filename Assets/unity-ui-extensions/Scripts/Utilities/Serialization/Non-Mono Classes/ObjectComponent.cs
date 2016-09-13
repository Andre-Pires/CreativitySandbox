//The ObjectComponent class holds all data of a gameobject's component.
//The Dictionary holds the actual data of a component; A field's name as key and the corresponding value (object) as value. Confusing, right?

using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utilities.Serialization
{
    [Serializable]
    public class ObjectComponent
    {
        public string componentName;
        public Dictionary<string, object> fields;
    }
}