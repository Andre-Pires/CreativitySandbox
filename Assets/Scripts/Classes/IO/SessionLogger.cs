using System;
using System.Globalization;
using System.IO;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.IO
{
    public class SessionLogger : MonoBehaviour
    {
        private static SessionLogger _instance;
        private const string FileName = "sessionLog";
        private const string FileExtension = ".txt";
        private readonly string _filePath = Constants.ImageFilePath;
        private readonly StreamWriter _fileWriter = null;

        public void Awake()
        {
            if (_instance == null)
                _instance = FindObjectOfType(typeof(SessionLogger)) as SessionLogger;
        }

        // Construct 	
        private SessionLogger()
        {
            try
            {
                string date = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss", CultureInfo.InvariantCulture);
                _fileWriter = File.CreateText(_filePath + "/" + FileName + "(" + date + ")" + FileExtension);
                _fileWriter.WriteLine("Log starts at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
                _fileWriter.WriteLine("");
                _fileWriter.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) + ": Storing images to path: " + Constants.ImageFilePath + ".");
                _fileWriter.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) + ": Storing sound messages to path: " + Constants.SoundFilePath + ".");

            }
            catch (Exception e)
            {
                Debug.Log("Exception thrown in logger: " + e.Message);
            }
            
        }

        //  Instance 	
        public static SessionLogger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType(typeof(SessionLogger)) as SessionLogger;
                return _instance;
            }
        }

        public void WriteToLogFile(string logEntry)
        {
            try
            {
                _fileWriter.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) + ": " + logEntry);
            }
            catch (Exception e )
            {
                Debug.Log("Exception thrown in logger: " + e.Message);
            }
        }

        public void OnDestroy()
        {
            try
            {
                _fileWriter.WriteLine("");
                _fileWriter.WriteLine("Final log entry: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) + ".");
                _fileWriter.Dispose();
            }
            catch (Exception e)
            {
                Debug.Log("Exception thrown in logger: " + e.Message);
            }
        }

    }
}
