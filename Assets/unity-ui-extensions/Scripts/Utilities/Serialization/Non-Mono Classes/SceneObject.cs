//This class holds is meant to hold all the data of a GameObject in the scene which has an ObjectIdentifier component. 
//The values from the OI component are mirrored here, along with misc. stuff like the activation state of the gameObect (bool active), and of course it's components.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utilities.Serialization
{
    [Serializable]
    public class SceneObject
    {
        public bool active;
        public string id;
        public string idParent;
        public Vector3 localScale;
        public string name;

        public List<ObjectComponent> objectComponents = new List<ObjectComponent>();
        public Vector3 position;
        public string prefabName;
        public Quaternion rotation;
    }
}