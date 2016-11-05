using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Classes.Helpers
{
    public class Configuration : MonoBehaviour
    {
        public enum BlinkingSpeed
        {
            Stopped,
            VerySlow,
            Slow,
            Normal,
            Fast,
            VeryFast
        }

        public enum Personality
        {
            Shy,
            Sociable,
            Grumpy,
            Friendly,
            Realist,
            Imaginative,
            Foreigner,
            CustomPersonality
        }

        public enum Colors
        {
            White,
            Gray,
            Black,
            Yellow,
            Orange,
            Red,
            Pink,
            Purple,
            Blue,
            Cyan,
            Green,
            Brown
        }

        public enum Size
        {
            Small,
            Medium,
            Large
        }

        public enum Transitions
        {
            Linear,
            EaseIn,
            Instant,
            EaseInOut
        }

        public enum Behaviors
        {
            Blink,
            Resize,
            Rotate
        }

        public enum ComposedBehaviors
        {
            Joy,
            Sadness,
            Disgust,
            Fear,
            Anger
        }

        // Singleton 	
        private static Configuration _instance;

        //NOTE: These parameters can be tweaked from unity's editor
        public bool SoundRecordingActive;
        public bool CameraMovementActive;
        public bool BlinkingBehaviorActive;

        public List<BlinkingSpeed> AvailableBlinkSpeeds;
        public List<Personality> AvailablePersonalities;
        public List<Size> AvailableSizes;
        public List<Transitions> AvailableTransitions;

        // hack that allow editing color associations in editor
        [Serializable]
        public struct ColorPair
        {
            public Colors Name;
            public Color Color;
        }
        public ColorPair[] ColorPairs;

        public Dictionary<BlinkingSpeed, float> BlinkingSpeedsValues;
        public Dictionary<Personality, BlinkingSpeed> PersonalityBlinkingSpeeds;
        public Dictionary<Personality, Color> PersonalityColors;
        public Dictionary<Colors, Color> ColorNames;
        public Dictionary<Size, float> SizeValues;

        //due to the random order of execution in Unity's scripts, this assigment is required in the Awake function
        public void Awake()
        {
            _instance = FindObjectOfType(typeof(Configuration)) as Configuration;

            ColorNames = new Dictionary<Colors, Color>();
            foreach (ColorPair pair in ColorPairs)
            {
                ColorNames.Add(pair.Name, pair.Color);
            }
        }

        // Construct 	
        private Configuration()
        {
            AvailableSizes = new List<Size>
            {
                Size.Small,
                Size.Medium,
                Size.Large
            };

            AvailableBlinkSpeeds = new List<BlinkingSpeed>
            {
                BlinkingSpeed.Stopped,
                BlinkingSpeed.VerySlow,
                BlinkingSpeed.Slow,
                BlinkingSpeed.Normal,
                BlinkingSpeed.Fast,
                BlinkingSpeed.VeryFast
            };

            //Indexes colors to allow an easy pairing with custom color picked from the editor - set in editor
            ColorNames = new Dictionary<Colors, Color>
            {
                { Colors.Blue, Color.blue},
                { Colors.Red, Color.red},
                { Colors.Green, Color.green},
                { Colors.Black, Color.black},
                { Colors.White, Color.white},
                { Colors.Yellow, Color.yellow}
            };

            BlinkingSpeedsValues = new Dictionary<BlinkingSpeed, float>
            {
                {BlinkingSpeed.VeryFast, 0.3f},
                {BlinkingSpeed.Fast, 0.6f},
                {BlinkingSpeed.Normal, 1.0f},
                {BlinkingSpeed.Slow, 1.5f},
                {BlinkingSpeed.VerySlow, 3.0f}
            };

            //TODO - change values to appropriate colors
            PersonalityColors = new Dictionary<Personality, Color>
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
            PersonalityBlinkingSpeeds = new Dictionary<Personality, BlinkingSpeed>
            {
                {Personality.Shy, BlinkingSpeed.Slow},
                {Personality.Sociable, BlinkingSpeed.Normal},
                {Personality.Grumpy, BlinkingSpeed.VeryFast},
                {Personality.Friendly, BlinkingSpeed.Fast},
                {Personality.Realist, BlinkingSpeed.VerySlow},
                {Personality.Imaginative, BlinkingSpeed.VeryFast},
                {Personality.Foreigner, BlinkingSpeed.Normal}
            };

            SizeValues = new Dictionary<Size, float>
            {
                {Size.Small, 4.5f},
                {Size.Medium, 6.0f},
                {Size.Large, 7.5f}
            };

            //NOTE: for our application we're only using 2 for now, set in-editor
            AvailableTransitions = new List<Transitions>
            {
                Transitions.EaseIn,
                Transitions.EaseInOut,
                Transitions.Instant,
                Transitions.Linear
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