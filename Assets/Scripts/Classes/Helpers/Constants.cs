using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Classes.Helpers
{
    public enum Scenario { ForestDay, ForestNight, ForestDusk, CityDay, CityNight, CityDusk}

    public class Constants
    {
        public static readonly String[] PersonalitiesStrings =
        {
            "Tímido", "Sociável", "Resmungão", "Amigável",
            "Realista", "Imaginativo", "Estrangeiro"
        };

        //set related constants
        public Dictionary<Scenario, string> ScenarioPath;
        public Dictionary<Scenario, Material> ScenarioSkybox;

        //file paths
        public static string ImageFilePath;
        public static string SoundFilePath;
        // Singleton 	
        private static Constants _instance;

        // Construct 	
        private Constants()
        {
            if (UnityEngine.Application.platform == RuntimePlatform.Android)
            {
                ImageFilePath = "sdcard/StopMotion/";
                SoundFilePath = UnityEngine.Application.persistentDataPath + "/ProjectData/Sounds/";
                
            }
            else
            {
                ImageFilePath = "../" + AppDomain.CurrentDomain.BaseDirectory + "/StopMotion/";
                SoundFilePath = "../" + AppDomain.CurrentDomain.BaseDirectory + "/ProjectData/Sounds/";
            }

            ScenarioPath = new Dictionary<Scenario, string>()
            {
                { Scenario.ForestDay, "Prefabs/Forest/ForestSet(Clear)"},
                { Scenario.ForestNight, "Prefabs/Forest/ForestSet(Night)"},
                { Scenario.ForestDusk, "Prefabs/Forest/ForestSet(Dusk)"},
                { Scenario.CityDay, "Prefabs/City/CitySet(Clear)"},
                { Scenario.CityNight, "Prefabs/City/CitySet(Night)"},
                { Scenario.CityDusk, "Prefabs/City/CitySet(Dusk)"},
            };

            Material skyboxDay = Object.Instantiate(Resources.Load("Skyboxes/sky5x3")) as Material;
            Material skyboxNight = Object.Instantiate(Resources.Load("Skyboxes/sky5x5")) as Material;
            Material skyboxDusk = Object.Instantiate(Resources.Load("Skyboxes/sky5x4")) as Material;

            ScenarioSkybox = new Dictionary<Scenario, Material>()
            {
                {Scenario.ForestDay, skyboxDay },
                {Scenario.CityDay, skyboxDay },
                {Scenario.ForestNight, skyboxNight },
                {Scenario.CityNight, skyboxNight },
                {Scenario.ForestDusk, skyboxDusk },
                {Scenario.CityDusk, skyboxDusk }
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
