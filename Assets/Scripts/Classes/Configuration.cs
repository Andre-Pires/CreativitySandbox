using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.Classes
{
    public class Configuration : MonoBehaviour
    {
        public enum Personality { Shy, Sociable, Grumpy, Friendly, Realist, Imaginative, Foreigner }
        public enum Size { Small, Medium, Large }

        public List<Personality> AvailablePersonalities;
        public List<Size> AvailableSizes;

        // Singleton 	
        private static Configuration _instance;

        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

        // Construct 	
        private Configuration()
        {
        }

        //  Instance 	
        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType(typeof(Configuration)) as Configuration;
                return _instance;
            }

        }
    }
}