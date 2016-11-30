using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.UI;
using Assets.Scripts.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Classes.IO
{
    public class ScreenRecorder : MonoBehaviour
    {

        public float OverlayOpacity = 0.2f;
        private const string FileName = "image";
        private const string FileExtension = ".png";
        private string _filePath = Constants.ImageFilePath;

        private GameObject _screenFlashPanel;
        private bool _flashActive;
        private int _numberOfShots = 0;
        private bool _recording = false;
        private bool _readySingleShot = false;
        private float _startShotTime = 0;
        public float ShotInterval = 0.8f;
        private Texture2D _latestScreenshot;
        private int _maxWidth = 1920;
        private int _maxHeight = 1080;

        //custom title screen
        private bool _actScreenMovieDone;
        private bool _confirmationScreenActive;

        //padd the remainder of photo number with zeros
        private int paddingLength = 4;

        //to overlay image on screen
        private Rect _rect;

        public void Start()
        {
            //in order to reduce the load of taking very high resolution screenshots in higher res devices
            int _applicationResWidth = Mathf.Min(_maxWidth, Screen.width);
            int _applicationResHeight = Mathf.Min(_maxHeight, Screen.height);
            
            Screen.SetResolution(_applicationResWidth, _applicationResHeight, true);

            //must use local variables since SetResolution isn't instant
            _latestScreenshot = new Texture2D(_applicationResWidth, _applicationResHeight, TextureFormat.RGB24, false);

            SetupCustomActScreen(_applicationResWidth, _applicationResHeight);
            //

            _screenFlashPanel = AppUIManager.Instance.ScreenFlashOverlay;
            if (_screenFlashPanel == null)
            {
                throw new NullReferenceException("The panel used for flash is deactivated in the editor");
            }

            AppUIManager.Instance.TakeSnapshot.GetComponent<Button>().onClick.AddListener(TakeSingleSnapshot);
            AppUIManager.Instance.ClearVideoRecordings.GetComponent<Button>().onClick.AddListener(ClearMovieRecordings);
            //clear empty directories
            ClearEmptyDirectories();

            // arrange snapshots in different folders
            _filePath = _filePath + "video(" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss", CultureInfo.InvariantCulture) +")/";
            Directory.CreateDirectory(_filePath);

            Debug.Log("Path" + _filePath);
        }

        private void SetupCustomActScreen(int width, int height)
        {
            
            AppUIManager.Instance.MovieActScreen.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            Button captureScreen = Utility.GetChild(AppUIManager.Instance.ActScreenInput, "Button").GetComponent<Button>();
            
            captureScreen.onClick.AddListener(
                () =>
                {
                    CaptureMovieActScreen();
                    _confirmationScreenActive = true;
                });

            Button keepScreenButton = Utility.GetChild(AppUIManager.Instance.ActScreenSave, "YesButton").GetComponent<Button>();

            keepScreenButton.onClick.AddListener(() => KeepMovieActScreen(true));

            Button eraseScreenButton = Utility.GetChild(AppUIManager.Instance.ActScreenSave, "NoButton").GetComponent<Button>();

            eraseScreenButton.onClick.AddListener(() => KeepMovieActScreen(false));
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

        public void KeepMovieActScreen(bool keepScreen)
        {
            if (!keepScreen)
            {
                string stringShotNumber = _numberOfShots.ToString("D" + paddingLength);
                File.Delete(_filePath + FileName + stringShotNumber + FileExtension);
                _numberOfShots--;
            }

            _actScreenMovieDone = true;
        }

        public void CaptureMovieActScreen()
        {
            InputField inputField =
                Utility.GetChild(AppUIManager.Instance.ActScreenInput, "InputField").GetComponent<InputField>();
            string titleMessage = inputField.text;
            inputField.text = "";
            AppUIManager.Instance.ActScreenInput.SetActive(false);

            Utility.GetChild(AppUIManager.Instance.MovieActScreen, "ActTitle").GetComponent<Text>().text = titleMessage;

            _numberOfShots++;
            string stringShotNumber = _numberOfShots.ToString("D" + paddingLength);
            UnityEngine.Application.CaptureScreenshot(_filePath + FileName + stringShotNumber + FileExtension);

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
            DirectoryInfo directory = new DirectoryInfo(_filePath);
            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in directory.GetDirectories())
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

            if (_confirmationScreenActive)
            {
                AppUIManager.Instance.ActScreenInput.SetActive(false);
                AppUIManager.Instance.ActScreenSave.SetActive(true);
                _confirmationScreenActive = false;
            }

            if (_actScreenMovieDone)
            {
                AppUIManager.Instance.ActScreenInput.SetActive(true);
                AppUIManager.Instance.ActScreenSave.SetActive(false);
                _actScreenMovieDone = false;
            }
        }

        
        private void CaptureScreenshot()
        {
            FlashScreen();

            //takes the screenshot and saves a file. It's stored as a Texture2D meanwhile
            _latestScreenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            _latestScreenshot.Apply();

            //ensuring the "0001" format
            string stringShotNumber = _numberOfShots.ToString("D" + paddingLength); 
            string fileName = _filePath + FileName + stringShotNumber + FileExtension;
            byte[] bytes = _latestScreenshot.EncodeToPNG();

            new Thread(() =>
            {
                File.WriteAllBytes(fileName, bytes);
                Thread.Sleep(500);
            }).Start();
        }

        public void FlashScreen()
        {
            _flashActive = true;
            AppUIManager.Instance.ScreenFlashOverlay.SetActive(true);
        }

        public void HandleScreenFlash()
        {
            if (_flashActive)
            {
                Image image = _screenFlashPanel.GetComponent<Image>();
                image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - 1.5f * Time.deltaTime);

                if (image.color.a <= 0)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
                    _flashActive = false;
                    AppUIManager.Instance.ScreenFlashOverlay.SetActive(false);
                }
            }
        }


        public void Update()
        {
            #if (UNITY_STANDALONE || UNITY_EDITOR)
                _rect = new Rect(0, 0, Screen.width, Screen.height);
            #endif

            HandleScreenFlash();
        }

        // ReSharper disable once InconsistentNaming
        public void OnGUI()
        {
            //only show if there is a previous shot and is pointing at scenario
            if (_numberOfShots > 0 && !(AppUIManager.Instance.MovieActScreen.activeSelf || AppUIManager.Instance.ColorPicker.activeSelf))
            {
                GUI.color = new Color(1.0f, 1.0f, 1.0f, OverlayOpacity);
                GUI.DrawTexture(_rect, _latestScreenshot, ScaleMode.StretchToFill, true);
                GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }


    }
}