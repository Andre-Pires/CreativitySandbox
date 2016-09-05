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
        public static readonly string[] PersonalitiesStrings =
        {
            "Tímido", "Sociável", "Resmungão", "Amigável",
            "Realista", "Imaginativo", "Estrangeiro"
        };

        //set related constants
        public Dictionary<Scenario, string> ScenarioPath;
        public Dictionary<Scenario, Material> ScenarioSkybox;

        //file paths
        public static string ImageFilePath = UnityEngine.Application.platform == RuntimePlatform.Android
            ? "sdcard/StopMotion/"
            : "../" + AppDomain.CurrentDomain.BaseDirectory + "/StopMotion/";

        public static string SoundFilePath = UnityEngine.Application.platform == RuntimePlatform.Android
            ? UnityEngine.Application.persistentDataPath + "/ProjectData/Sounds/"
            : "../" + AppDomain.CurrentDomain.BaseDirectory + "/ProjectData/Sounds/";

        // Singleton 	
        private static Constants _instance;

        // Construct 	
        private Constants()
        {
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

        public string GetPersonalityString(Configuration.Personality personality)
        {
            return PersonalitiesStrings[(int) personality];
        }
    }
}
