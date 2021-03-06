using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.Utilities.Serialization.Surrogates;
using UnityEngine;

namespace Assets.Scripts.Utilities.Serialization
{
    public static class SaveLoad
    {
        //You may define any path you like, such as "c:/Saved Games"
        //remember to use slashes instead of backslashes! ("/" instead of "\")
        //Application.DataPath: http://docs.unity3d.com/ScriptReference/Application-dataPath.html
        //Application.persistentDataPath: http://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
        public static string saveGamePath = Application.persistentDataPath + "/Saved Games/";

        public static void Save(SaveGame saveGame)
        {
            var bf = new BinaryFormatter();

            // 1. Construct a SurrogateSelector object
            var ss = new SurrogateSelector();
            // 2. Add the ISerializationSurrogates to our new SurrogateSelector
            AddSurrogates(ref ss);
            // 3. Have the formatter use our surrogate selector
            bf.SurrogateSelector = ss;

            //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
            //You can also use any path you like
            CheckPath(saveGamePath);

            var file = File.Create(saveGamePath + saveGame.savegameName + ".sav");
                //you can call it anything you want including the file extension
            bf.Serialize(file, saveGame);
            file.Close();
            Debug.Log("Saved Game: " + saveGame.savegameName);
        }

        public static SaveGame Load(string gameToLoad)
        {
            if (File.Exists(saveGamePath + gameToLoad + ".sav"))
            {
                var bf = new BinaryFormatter();
                // 1. Construct a SurrogateSelector object
                var ss = new SurrogateSelector();
                // 2. Add the ISerializationSurrogates to our new SurrogateSelector
                AddSurrogates(ref ss);
                // 3. Have the formatter use our surrogate selector
                bf.SurrogateSelector = ss;

                var file = File.Open(saveGamePath + gameToLoad + ".sav", FileMode.Open);
                var loadedGame = (SaveGame) bf.Deserialize(file);
                file.Close();
                Debug.Log("Loaded Game: " + loadedGame.savegameName);
                return loadedGame;
            }
            Debug.Log(gameToLoad + " does not exist!");
            return null;
        }

        private static void AddSurrogates(ref SurrogateSelector ss)
        {
            var Vector2_SS = new Vector2Surrogate();
            ss.AddSurrogate(typeof(Vector2),
                new StreamingContext(StreamingContextStates.All),
                Vector2_SS);

            var Vector3_SS = new Vector3Surrogate();
            ss.AddSurrogate(typeof(Vector3),
                new StreamingContext(StreamingContextStates.All),
                Vector3_SS);

            var Vector4_SS = new Vector4Surrogate();
            ss.AddSurrogate(typeof(Vector4),
                new StreamingContext(StreamingContextStates.All),
                Vector4_SS);

            var Color_SS = new ColorSurrogate();
            ss.AddSurrogate(typeof(Color),
                new StreamingContext(StreamingContextStates.All),
                Color_SS);

            var Quaternion_SS = new QuaternionSurrogate();
            ss.AddSurrogate(typeof(Quaternion),
                new StreamingContext(StreamingContextStates.All),
                Quaternion_SS);

            //Reserved for future implementation
            //Texture2DSurrogate Texture2D_SS = new Texture2DSurrogate();
            //ss.AddSurrogate(typeof(Texture2D),
            //                new StreamingContext(StreamingContextStates.All),
            //                Texture2D_SS);
            //GameObjectSurrogate GameObject_SS = new GameObjectSurrogate();
            //ss.AddSurrogate(typeof(GameObject),
            //                new StreamingContext(StreamingContextStates.All),
            //                GameObject_SS);
            //TransformSurrogate Transform_SS = new TransformSurrogate();
            //ss.AddSurrogate(typeof(Transform),
            //                new StreamingContext(StreamingContextStates.All),
            //                Transform_SS);
        }

        private static void CheckPath(string path)
        {
            try
            {
                // Determine whether the directory exists. 
                if (Directory.Exists(path))
                {
                    //Debug.Log("That path exists already.");
                    return;
                }

                // Try to create the directory.
                //DirectoryInfo dir = Directory.CreateDirectory(path);
                Directory.CreateDirectory(path);
                Debug.Log("The directory was created successfully at " + path);
            }
            catch (Exception e)
            {
                Debug.Log("The process failed: " + e);
            }
            finally
            {
            }
        }
    }
}