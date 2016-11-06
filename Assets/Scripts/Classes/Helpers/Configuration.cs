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
            Anger,
            Random
        }

        public enum ActiveBehaviors
        {
            StandardBehavior,
            ExcitedBehavior
        }

        public enum ProxemicDistance
        {
            Personal,
            Social,
            Intimate
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
        [HideInInspector]
        public List<Color> AvailableColors;


        // hack that allow editing color associations in editor
        [Serializable]
        private struct ColorPair
        {
            public Colors Name;
            public Color Color;
        }
        [SerializeField]
        private ColorPair[] _colorPairs;

        public Dictionary<BlinkingSpeed, float> BlinkingSpeedsValues;
        public Dictionary<Personality, BlinkingSpeed> PersonalityBlinkingSpeeds;
        public Dictionary<Personality, Color> PersonalityColors;
        public Dictionary<Personality, ComposedBehaviors> Personalitybehaviors;
        public Dictionary<Colors, Color> ColorNames;
        public Dictionary<Size, float> SizeValues;
        //due to the random order of execution in Unity's scripts, this assigment is required in the Awake function
        public void Awake()
        {
            _instance = FindObjectOfType(typeof(Configuration)) as Configuration;

            ColorNames = new Dictionary<Colors, Color>();
            AvailableColors = new List<Color>();

            foreach (ColorPair pair in _colorPairs)
            {
                ColorNames.Add(pair.Name, pair.Color);
                AvailableColors.Add(pair.Color);
            }

            PersonalityColors = new Dictionary<Personality, Color>
            {
                {Personality.Shy, ColorNames[Colors.Purple]},
                {Personality.Sociable, ColorNames[Colors.Pink]},
                {Personality.Grumpy, ColorNames[Colors.Orange]},
                {Personality.Friendly, ColorNames[Colors.Green]},
                {Personality.Realist, ColorNames[Colors.Gray]},
                {Personality.Imaginative, ColorNames[Colors.Yellow]},
                {Personality.Foreigner, ColorNames[Colors.Brown]}
            };

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
                { Colors.Yellow, Color.yellow},
                { Colors.Gray, Color.gray},
            };

            BlinkingSpeedsValues = new Dictionary<BlinkingSpeed, float>
            {
                {BlinkingSpeed.VeryFast, 0.3f},
                {BlinkingSpeed.Fast, 0.6f},
                {BlinkingSpeed.Normal, 1.0f},
                {BlinkingSpeed.Slow, 1.5f},
                {BlinkingSpeed.VerySlow, 3.0f}
            };

            //for now these have been disregarded
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

            Personalitybehaviors = new Dictionary<Personality, ComposedBehaviors>
            {
                {Personality.Shy, ComposedBehaviors.Fear},
                {Personality.Sociable, ComposedBehaviors.Joy},
                {Personality.Grumpy, ComposedBehaviors.Anger},
                {Personality.Friendly, ComposedBehaviors.Joy},
                {Personality.Realist, ComposedBehaviors.Sadness},
                {Personality.Imaginative, ComposedBehaviors.Joy},
                {Personality.Foreigner, ComposedBehaviors.Random}
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