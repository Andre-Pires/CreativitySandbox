using System;

namespace Assets.Scripts.Classes.Helpers
{
    public class Constants
    {
        public static readonly String[] PersonalitiesStrings =
        {
            "Tímido", "Sociável", "Resmungão", "Amigável",
            "Realista", "Imaginativo", "Estrangeiro"
        };

        // Singleton 	
        private static Constants _instance;

        // Construct 	
        private Constants()
        {
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
