using System.Threading;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Classes.UI
{
    public class AppUIManager
    {
        private static AppUIManager _instance;

        private GameObject _audioRecordingStartInfo;
        private GameObject _audioRecordingStoppedInfo;
        public GameObject AvailableAgentPiecesList;
        public GameObject ColorMenuCloseButton;
        public GameObject SkyboxColorPicker;
        public GameObject SetColorPicker;
        public GameObject MovieActScreen;
        public GameObject ActScreenInput;
        public GameObject ActScreenSave;

        private bool _recordingStoppedInfoTimeout;

        // Construct 	
        private AppUIManager()
        {
            BindUIObjects();
            SetupUI();
        }

        //  Instance 	
        public static AppUIManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AppUIManager();
                return _instance;
            }
        }

        private void BindUIObjects()
        {
            _audioRecordingStartInfo = GameObject.Find("AudioRecordingWarning").gameObject;
            _audioRecordingStoppedInfo = GameObject.Find("AudioRecordingSuccessful").gameObject;
            AvailableAgentPiecesList = GameObject.Find("AvailableAgentPieces/Image/List").gameObject;
            SetColorPicker = GameObject.Find("SetColors").gameObject;
            SkyboxColorPicker = GameObject.Find("SkyboxColors").gameObject;
            ColorMenuCloseButton = GameObject.Find("Scenario Color controls/CloseButton").gameObject;
            MovieActScreen = GameObject.Find("CreateNewActScreen");
            ActScreenInput = GameObject.Find("CreateNewActScreen/WriteMessage");
            ActScreenSave = GameObject.Find("CreateNewActScreen/SaveScreen");
        }


        // ReSharper disable once InconsistentNaming
        //necessary since if gameObjects start inactive their associated scripts aren't accessible
        public void SetupUI()
        {
            GameObject.Find("PieceMenu").gameObject.SetActive(false);
            GameObject.Find("PieceSelection").gameObject.SetActive(false);
            Debug.Log("desactivou");
            GameObject.Find("SceneSelector").gameObject.SetActive(false);
            GameObject.Find("Settings Menu/Options").gameObject.SetActive(false);

            _audioRecordingStartInfo.SetActive(false);
            _audioRecordingStoppedInfo.SetActive(false);
            SetColorPicker.SetActive(false);
            SkyboxColorPicker.SetActive(false);
            ColorMenuCloseButton.SetActive(false);
            MovieActScreen.SetActive(false);
            ActScreenSave.SetActive(false);

            if (!Configuration.Instance.CameraMovementActive)
            {
                GameObject.Find("CameraModeToggle").SetActive(false);
            }
        }

        public void DisplayRecordingStarted()
        {
            _audioRecordingStartInfo.SetActive(true);
        }

        public void DisplayRecordingStopped()
        {
            _audioRecordingStartInfo.SetActive(false);
            _audioRecordingStoppedInfo.SetActive(true);

            new Thread(() =>
            {
                Thread.Sleep(1500);
                _recordingStoppedInfoTimeout = true;
            }).Start();
        }

        public void Update()
        {
            if (_recordingStoppedInfoTimeout)
            {
                _audioRecordingStoppedInfo.SetActive(false);
                _recordingStoppedInfoTimeout = false;
            }

            if (AvailableAgentPiecesList.activeSelf && AvailableAgentPiecesList.transform.childCount > 0)
            {
                Text text = AvailableAgentPiecesList.GetComponent<Text>();
                if (text.IsActive())
                {
                    text.enabled = false;
                }
            }
            else if (AvailableAgentPiecesList.activeSelf && AvailableAgentPiecesList.transform.childCount == 0)
            {
                Text text = AvailableAgentPiecesList.GetComponent<Text>();
                if (!text.IsActive())
                {
                    text.enabled = true;
                }
            }
        }

        public void OnGUI()
        {
        }

        
    }
}