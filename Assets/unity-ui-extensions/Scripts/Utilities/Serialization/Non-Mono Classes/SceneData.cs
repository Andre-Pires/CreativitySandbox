using System.Collections.Generic;

namespace Assets.Scripts.Utilities.Serialization
{
    [System.Serializable]
    public class SaveGame
    {

        public string savegameName = "New SaveGame";
        public List<SceneObject> sceneObjects = new List<SceneObject>();

        public SaveGame()
        {

        }

        public SaveGame(string s, List<SceneObject> list)
        {
            savegameName = s;
            sceneObjects = list;
        }
    }
}
