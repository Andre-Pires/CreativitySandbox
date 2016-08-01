using System;
using System.Globalization;
using System.IO;
using Assets.Scripts.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Classes.IO
{
    public class ScreenRecorder : MonoBehaviour
    {
        public float OverlayOpacity = 0.2f;
        private string _fileName = "default";
        private string _filePath = "../" + AppDomain.CurrentDomain.BaseDirectory + "/StopMotion/";
        private string _fileExtension = ".png";

        private int _numberOfShots = 0;
        private bool _recording = false;
        private bool _readySingleShot = false;
        private float _startShotTime = 0;
        public float ShotInterval = 0.8f;
        Texture2D _latestScreenshot;

        private Canvas _canvas;

        //to overlay image on screen
        private Rect _rect;
        private Texture2D _image;

        public delegate void OnDestroyEvent();
        public event OnDestroyEvent OnGameObjectDestroy;

        public void Awake()
        {
            
            TakeSnapshot.Instance.OnSelect += TakeSingleSnapshot;
            StartVideoRecording.Instance.OnSelect += StartRecordingMovie;
            PauseVideoRecording.Instance.OnSelect += PauseRecordingMovie;
            ClearVideoRecordings.Instance.OnSelect += ClearMovieRecordings;
            
            _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

            //clear empty directories
            ClearEmptyDirectories();

            // arrange snapshots in different folders
            _filePath = _filePath + "video(" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss", CultureInfo.InvariantCulture) +")/";
            Directory.CreateDirectory(_filePath);
        }

        private void ClearEmptyDirectories()
        {
            if(Directory.Exists(_filePath))
            {
                string[] directoriesInFolder = Directory.GetDirectories(_filePath);

                foreach (string directory in directoriesInFolder)
                {
                    string[] filesFound = Directory.GetFiles(directory + "/");
                    if (filesFound.Length == 0)
                    {
                        Directory.Delete(directory + "/", true);
                    }
                }
            }
        }

        public void TakeSingleSnapshot()
        {
            _readySingleShot = true;
        }

        public void StartRecordingMovie()
        {
            if (!_recording)
            {
                _recording = true;
                _startShotTime = Time.time;
            }
        }

        public void PauseRecordingMovie()
        {
            _recording = false;
        }

        public void ClearMovieRecordings()
        {
            for (int shotIndex = 0; shotIndex < _numberOfShots; shotIndex++)
            {
                Directory.Delete(_filePath + _fileName + shotIndex + _fileExtension, true);
            }

            _numberOfShots = 0;
        }

        public void OnPostRender()
        {

            if (_recording && Time.time - _startShotTime >= ShotInterval)
            {
                _canvas.enabled = false;
                CaptureScreenshot();
                _canvas.enabled = true;
                _startShotTime = Time.time;
                _numberOfShots++;
            }

            if (_readySingleShot)
            {
                _canvas.enabled = false;
                _numberOfShots++;
                CaptureScreenshot();
                _canvas.enabled = true;
                _readySingleShot = false;
            }
        }

        private void CaptureScreenshot()
        {
            //takes the screenshot, but doesn't save a file. It's stored as a Texture2D instead
            _latestScreenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            _latestScreenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            _latestScreenshot.Apply();
            //saves a PNG file to the path specified above
            byte[] bytes = _latestScreenshot.EncodeToPNG();
            File.WriteAllBytes(_filePath + _fileName + _numberOfShots + _fileExtension, bytes);
        }

        // constantly doing this, should only do on screen resize
        public void Update()
        {
            _rect = new Rect(0, 0, Screen.width, Screen.height);
        }

        public void OnDestroy()
        {
            OnGameObjectDestroy();
        }

        public void OnApplicationQuit()
        {
            //removing listeners when destroyed
            TakeSnapshot.Instance.OnSelect -= TakeSingleSnapshot;
            StartVideoRecording.Instance.OnSelect -= StartRecordingMovie;
            PauseVideoRecording.Instance.OnSelect -= PauseRecordingMovie;
            ClearVideoRecordings.Instance.OnSelect -= ClearMovieRecordings;
        }

        // ReSharper disable once InconsistentNaming
        public void OnGUI()
        {
            if (_numberOfShots > 0)
            {
                GUI.color = new Color(1.0f, 1.0f, 1.0f, OverlayOpacity);
                GUI.DrawTexture(_rect, _latestScreenshot, ScaleMode.StretchToFill, true);
                GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }
}