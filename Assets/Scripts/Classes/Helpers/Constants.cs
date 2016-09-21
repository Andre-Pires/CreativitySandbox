using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Classes.Helpers
{
    public enum Scenario
    {
        ForestDay,
        ForestNight,
        ForestDusk,
        CityDay,
        CityNight,
        CityDusk
    }

    public class Constants
    {
        public Dictionary<Configuration.Personality, string> PersonalitiesStrings;
        public Dictionary<Configuration.BlinkingSpeed, string> SpeedStrings;

        //file paths
        public static string ImageFilePath = UnityEngine.Application.platform == RuntimePlatform.Android
            ? "sdcard/StopMotion/"
            : "../" + AppDomain.CurrentDomain.BaseDirectory + "/StopMotion/";

        public static string SoundFilePath = UnityEngine.Application.platform == RuntimePlatform.Android
            ? UnityEngine.Application.persistentDataPath + "/ProjectData/Sounds/"
            : "../" + AppDomain.CurrentDomain.BaseDirectory + "/ProjectData/Sounds/";

        // Singleton 	
        private static Constants _instance;
        //set related constants
        public Dictionary<Scenario, string> ScenarioPath;
        public Dictionary<Scenario, Material> ScenarioSkybox;

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
                {Scenario.ForestDay, "Prefabs/Forest/ForestSet(Clear)"},
                {Scenario.ForestNight, "Prefabs/Forest/ForestSet(Night)"},
                {Scenario.ForestDusk, "Prefabs/Forest/ForestSet(Dusk)"},
                {Scenario.CityDay, "Prefabs/City/CitySet(Clear)"},
                {Scenario.CityNight, "Prefabs/City/CitySet(Night)"},
                {Scenario.CityDusk, "Prefabs/City/CitySet(Dusk)"}
            };

            var skyboxDay = Object.Instantiate(Resources.Load("Skyboxes/sky5x3")) as Material;
            var skyboxNight = Object.Instantiate(Resources.Load("Skyboxes/sky5x5")) as Material;
            var skyboxDusk = Object.Instantiate(Resources.Load("Skyboxes/sky5x4")) as Material;

            ScenarioSkybox = new Dictionary<Scenario, Material>
            {
                {Scenario.ForestDay, skyboxDay},
                {Scenario.CityDay, skyboxDay},
                {Scenario.ForestNight, skyboxNight},
                {Scenario.CityNight, skyboxNight},
                {Scenario.ForestDusk, skyboxDusk},
                {Scenario.CityDusk, skyboxDusk}
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