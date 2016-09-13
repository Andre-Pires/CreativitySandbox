using System;
using UnityEngine;

namespace Assets.Scripts.Utilities.Serialization.ExampleScriptsAndClasses
{
    [Serializable]
    public class TestClass
    {
        public Color color;
        public GameObject go;
        public string go_id;
        public int[] myArray = {2, 43, 12};

        public string myString;
        public Vector3 somePosition;
    }
}