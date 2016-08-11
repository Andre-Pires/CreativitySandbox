using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Classes.Helpers
{
    public class Configuration : MonoBehaviour
    {
        public enum Personality { Shy, Sociable, Grumpy, Friendly, Realist, Imaginative, Foreigner }
        public enum Size { Small, Medium, Large }
        public enum BlinkingSpeed { Stopped, VerySlow, Slow, Normal, Fast, VeryFast }

        public List<Personality> AvailablePersonalities;
        public Dictionary<Size, float> AvailableSizes;
        public Dictionary<BlinkingSpeed, float> AvailableBlinkingSpeeds;
        public Dictionary<Personality, Color> PersonalityColors;
        public Dictionary<Personality, BlinkingSpeed> PersonalityBlinkingSpeeds;

        // Singleton 	
        private static Configuration _instance;

        // Construct 	
        private Configuration()
        {
            AvailableBlinkingSpeeds = new Dictionary<BlinkingSpeed, float>()
            {
                { BlinkingSpeed.VeryFast, 0.3f},
                { BlinkingSpeed.Fast, 0.6f},
                { BlinkingSpeed.Normal, 1.0f},
                { BlinkingSpeed.Slow, 1.5f},
                { BlinkingSpeed.VerySlow, 3.0f},
                { BlinkingSpeed.Stopped, 0.0f}
            };

            //TODO - change values to appropriate colors
            PersonalityColors = new Dictionary<Personality, Color>()
            {
                {Personality.Shy, Color.blue},
                {Personality.Sociable, Color.magenta},
                {Personality.Grumpy, Color.red},
                {Personality.Friendly, Color.green},
                {Personality.Realist, Color.black},
                {Personality.Imaginative, Color.yellow},
                {Personality.Foreigner, Color.black}
            };

            //TODO - change values to appropriate values
            PersonalityBlinkingSpeeds = new Dictionary<Personality, BlinkingSpeed>()
            {
                {Personality.Shy, BlinkingSpeed.Slow},
                {Personality.Sociable, BlinkingSpeed.Normal},
                {Personality.Grumpy, BlinkingSpeed.VeryFast},
                {Personality.Friendly, BlinkingSpeed.Fast},
                {Personality.Realist, BlinkingSpeed.VerySlow},
                {Personality.Imaginative, BlinkingSpeed.VeryFast},
                {Personality.Foreigner, BlinkingSpeed.Normal}
            };

            AvailableSizes = new Dictionary<Size, float>()
            {
                {Size.Small, 3.0f },
                {Size.Medium, 4.5f },
                {Size.Large, 6.0f },
            };
    }

        //  Instance 	
        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType(typeof(Configuration)) as Configuration;
                return _instance;
            }

        }
    }
}