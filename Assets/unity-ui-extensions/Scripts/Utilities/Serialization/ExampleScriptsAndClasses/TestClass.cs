using UnityEngine;

namespace Assets.Scripts.Utilities.Serialization.ExampleScriptsAndClasses
{
    [System.Serializable]
    public class TestClass
    {

        public string myString;
        public GameObject go;
        public string go_id;
        public Vector3 somePosition;
        public Color color;
        public int[] myArray = new int[] { 2, 43, 12 };
    }
}

