using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Classes.IO
{
    public class ScreenRecorder : MonoBehaviour
    {
        private const string FileName = "image";
        private const string FileExtension = ".jpg";
        private const string TextureExtension = ".t2d";
        private string _filePath = Constants.ImageFilePath;
        private Texture2D _latestScreenshot;

        private int _numberOfShots;
        private bool _readySingleShot;
        private bool _recording;

        //to overlay image on screen
        private Rect _rect;
        private float _startShotTime;

        public float OverlayOpacity = 0.2f;
        public float ShotInterval = 0.8f;

        public void Awake()
        {
            TakeSnapshot.Instance.OnSelect += TakeSingleSnapshot;
            StartVideoRecording.Instance.OnSelect += StartRecordingMovie;
            PauseVideoRecording.Instance.OnSelect += PauseRecordingMovie;
            ClearVideoRecordings.Instance.OnSelect += ClearMovieRecordings;
            SaveVideoRecordings.Instance.OnSelect += EncodeRecordedImages;


            //clear empty directories
            ClearEmptyDirectories();

            // arrange snapshots in different folders
            _filePath = _filePath + "video(" +
                        DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss", CultureInfo.InvariantCulture) + ")/";
            Directory.CreateDirectory(_filePath);

            Debug.Log("Path" + _filePath);
        }

        private void ClearEmptyDirectories()
        {
            if (Directory.Exists(_filePath))
            {
                var directoriesInFolder = Directory.GetDirectories(_filePath);

                foreach (var directory in directoriesInFolder)
                {
                    var filesFound = Directory.GetFiles(directory + "/");
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
            var directory = new DirectoryInfo(_filePath);
            foreach (var file in directory.GetFiles())
            {
                file.Delete();
            }
            foreach (var dir in directory.GetDirectories())
            {
                dir.Delete(true);
            }

            _numberOfShots = 0;
        }

        public void OnPostRender()
        {
            if (_recording && Time.time - _startShotTime >= ShotInterval)
            {
                _numberOfShots++;
                CaptureScreenshot();
                _startShotTime = Time.time;
            }

            if (_readySingleShot)
            {
                _numberOfShots++;
                CaptureScreenshot();
                _readySingleShot = false;
            }
        }


        private void CaptureScreenshot()
        {
            //takes the screenshot, but doesn't save a file. It's stored as a Texture2D instead
            _latestScreenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            _latestScreenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            _latestScreenshot.Apply();

            //TODO - rollback if needed - saves a PNG file to the path specified above
            /*string fileName = _filePath + _fileName + _numberOfShots + _fileExtension;
            byte[] bytes = _latestScreenshot.EncodeToPNG();*/

            var fileName = _filePath + FileName + _numberOfShots + TextureExtension;
            var bytes = _latestScreenshot.GetRawTextureData();

            new Thread(() =>
            {
                File.WriteAllBytes(fileName, bytes);
                Thread.Sleep(100);
            }).Start();
        }


        // constantly doing this, should only do on screen resize
        public void Update()
        {
            _rect = new Rect(0, 0, Screen.width, Screen.height);
        }

        public void OnApplicationQuit()
        {
            //removing listeners when destroyed
            TakeSnapshot.Instance.OnSelect -= TakeSingleSnapshot;
            StartVideoRecording.Instance.OnSelect -= StartRecordingMovie;
            PauseVideoRecording.Instance.OnSelect -= PauseRecordingMovie;
            ClearVideoRecordings.Instance.OnSelect -= ClearMovieRecordings;
            SaveVideoRecordings.Instance.OnSelect -= EncodeRecordedImages;
        }

        public void EncodeRecordedImages()
        {
            var directory = new DirectoryInfo(_filePath);
            var t = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            var currentShot = 0;
            foreach (var file in directory.GetFiles())
            {
                //ignore alreadt encoded screenshots
                if (Path.GetExtension(file.DirectoryName + file.Name) == FileExtension)
                {
                    continue;
                }

                t.LoadRawTextureData(File.ReadAllBytes(_filePath + file.Name));
                var bytes = t.EncodeToJPG();

                //extracting the index of the current file
                var resultString = Regex.Match(file.Name, @"\d+").Value;
                var shot = int.Parse(resultString);
                new Thread(() =>
                {
                    File.WriteAllBytes(_filePath + FileName + shot + FileExtension, bytes);
                    Thread.Sleep(100);
                }).Start();
                file.Delete();

                currentShot++;
            }
            Debug.Log(string.Format("Took " + currentShot + " screenshot(s) to: {0}", _filePath));
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