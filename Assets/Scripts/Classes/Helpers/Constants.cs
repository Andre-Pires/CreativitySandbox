using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Classes.Helpers
{
    public enum Scenario
    {
        SoftBlankSet,
        SpikyBlankSet,
        SkylineBlankSet,
        StraightBlankSet
    }

    public class Constants
    {
        public Dictionary<Configuration.Personality, string> PersonalitiesStrings;
        public Dictionary<Configuration.BlinkingSpeed, string> SpeedStrings;
        public const string VideoFolderName = "O meu filme";
        //file paths
        public static string ImageFilePath = UnityEngine.Application.platform == RuntimePlatform.Android
            ? "sdcard/"+ VideoFolderName + "/"
            : "../" + AppDomain.CurrentDomain.BaseDirectory + "/" + VideoFolderName + "/";

        public static string SoundFilePath = UnityEngine.Application.platform == RuntimePlatform.Android
            ? UnityEngine.Application.persistentDataPath + "/ProjectData/Sounds/RecordedMessages/"
            : "../" + AppDomain.CurrentDomain.BaseDirectory + "/ProjectData/Sounds/RecordedMessages/";

        //set related constants
        public Dictionary<Scenario, string> ScenarioPath;

        public static string ScenarioCameraMode = "Fixada no cenário";
        public static string CharacterCameraMode = "Segue o personagem";
        //in case a name needs to be added
        public static string CharacterName = "";

        // Singleton 	
        private static Constants _instance;

        // Construct 	
        private Constants()
        {
            PersonalitiesStrings = new Dictionary<Configuration.Personality, string>
            {
                {Configuration.Personality.Shy, "Tímido"},
                {Configuration.Personality.Sociable, "Sociável"},
                {Configuration.Personality.Grumpy, "Resmungão"},
                {Configuration.Personality.Friendly, "Amigável"},
                {Configuration.Personality.Realist, "Realista"},
                {Configuration.Personality.Imaginative, "Imaginativo"},
                {Configuration.Personality.Foreigner, "Estrangeiro"}
            };

            SpeedStrings = new Dictionary<Configuration.BlinkingSpeed, string>
            {
                {Configuration.BlinkingSpeed.Stopped, "Parado"},
                {Configuration.BlinkingSpeed.VerySlow, "Muito lento"},
                {Configuration.BlinkingSpeed.Slow, "Lento"},
                {Configuration.BlinkingSpeed.Normal, "Normal"},
                {Configuration.BlinkingSpeed.Fast, "Rápido"},
                {Configuration.BlinkingSpeed.VeryFast, "Muito rápido"}
            };

            ScenarioPath = new Dictionary<Scenario, string>
            {
                {Scenario.SoftBlankSet, "Prefabs/SoftBlank/SoftBlankSet"},
                {Scenario.SpikyBlankSet, "Prefabs/SpikyBlank/SpikyBlankSet"},
                {Scenario.SkylineBlankSet, "Prefabs/SkylineBlank/SkylineBlankSet"},
                {Scenario.StraightBlankSet, "Prefabs/StraightBlank/StraightBlankSet"}
            };
        }

        //  Instance 	
        public static Constants Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Constants();
                }
                return _instance;
            }
        }

    }
}