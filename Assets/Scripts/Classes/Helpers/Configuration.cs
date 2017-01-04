using System;
using System.Collections.Generic;
using Assets.Scripts.Classes.IO;
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
            Joy,
            Sadness,
            Disgust,
            Fear,
            Anger,
            CustomPersonality
        }

        public enum Colors
        {
            White,
            Gray,
            Black,
            Yellow,
            DarkYellow,
            Orange,
            Red,
            DarkRed,
            Purple,
            DarkPurple,
            Blue,
            DarkBlue,
            Cyan,
            Green,
            DarkGreen,
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
            EaseOut,
            Instant,
            EaseInOut
        }

        public enum RotationDirection
        {
            Left,
            Right,
            Alternating,
            Random
        }

        public enum Behaviors
        {
            Blink,
            Resize,
            Rotate,
            Animate
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

        public enum ApplicationMode
        {
            AutonomousAgent,
            ManuallyActivatedAgent,
            ConfigurableAgent
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
        public Dictionary<Personality, ComposedBehaviors> PersonalityBehaviors;
        public Dictionary<Personality, Size> PersonalitySizes;
        public Dictionary<Colors, Color> ColorNames;
        public Dictionary<Size, float> SizeValues;

        public void Awake()
        {
            //due to the random order of execution in Unity's scripts, this assigment is required in the Awake function
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
                {Personality.Joy, ColorNames[Colors.DarkYellow]},
                {Personality.Sadness, ColorNames[Colors.Blue]},
                {Personality.Disgust, ColorNames[Colors.Green]},
                {Personality.Anger, ColorNames[Colors.Red]},
                {Personality.Fear, ColorNames[Colors.Purple]}
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
                {Personality.Sadness, BlinkingSpeed.Slow},
                {Personality.Fear, BlinkingSpeed.Fast},
                {Personality.Anger, BlinkingSpeed.VeryFast},
                {Personality.Joy, BlinkingSpeed.Fast},
                {Personality.Disgust, BlinkingSpeed.Normal}
            };

            PersonalityBehaviors = new Dictionary<Personality, ComposedBehaviors>
            {
                {Personality.Fear, ComposedBehaviors.Fear},
                {Personality.Joy, ComposedBehaviors.Joy},
                {Personality.Anger, ComposedBehaviors.Anger},
                {Personality.Sadness, ComposedBehaviors.Sadness},
                {Personality.CustomPersonality, ComposedBehaviors.Random}
            };

            PersonalitySizes = new Dictionary<Personality, Size>
            {
                {Personality.Sadness, Size.Medium},
                {Personality.Fear, Size.Medium},
                {Personality.Anger, Size.Medium},
                {Personality.Joy, Size.Medium},
                {Personality.Disgust, Size.Medium}
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