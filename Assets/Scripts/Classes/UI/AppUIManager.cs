using System.Threading;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Classes.UI
{
    public class AppUIManager : MonoBehaviour
    {
        private static AppUIManager _instance;

        //General UI
        public GameObject AudioRecordingStartInfo;
        public GameObject AudioRecordingStoppedInfo;
        private bool _recordingStoppedInfoTimeout;

        //Application
        public GameObject ApplicationMode;

        //Agent
        public GameObject PieceSelection;
        public GameObject AvailableAgentPiecesList;

        //Screen Recorder
        public GameObject ClearVideoRecordings;
        public GameObject TakeSnapshot;
        public GameObject MovieActScreen;
        public GameObject ActScreenInput;
        public GameObject ActScreenSave;
        public GameObject ScreenFlashOverlay;

        //Scenario Color Picker
        public GameObject ColorPickerBackground;
        public GameObject ColorMenuCloseButton;
        public GameObject SkyboxColorPicker;
        public GameObject SetColorPicker;
        public GameObject ColorPicker;

        //Canvases
        public GameObject MainCanvas;
        public GameObject ScreenOverlays;

        public void Awake()
        {
            //due to the random order of execution in Unity's scripts, this assigment is required in the Awake function
            _instance = FindObjectOfType(typeof(AppUIManager)) as AppUIManager;

            SetupUI();
        }

        // Construct 	
        private AppUIManager()
        {
        }

        //  Instance 	
        public static AppUIManager Instance
        {
            get
            {
                if (_instance == null)
                    //due to the random order of execution in Unity's scripts, this assigment is required in the Awake function
                    _instance = FindObjectOfType(typeof(AppUIManager)) as AppUIManager;
                return _instance;
            }
        }

        // ReSharper disable once InconsistentNaming
        //necessary since if gameObjects start inactive their associated scripts aren't accessible
        public void SetupUI()
        {
            AudioRecordingStartInfo.SetActive(false);
            AudioRecordingStoppedInfo.SetActive(false);
            MovieActScreen.SetActive(false);
            ActScreenSave.SetActive(false);

            //inactive canvas
            ScreenOverlays.SetActive(false);

            if (!Configuration.Instance.CameraMovementActive)
            {
                GameObject.Find("CameraModeToggle").SetActive(false);
            }
        }

        public void DisplayRecordingStarted()
        {
            AudioRecordingStartInfo.SetActive(true);
        }

        public void DisplayRecordingStopped()
        {
            AudioRecordingStartInfo.SetActive(false);
            AudioRecordingStoppedInfo.SetActive(true);

            new Thread(() =>
            {
                Thread.Sleep(1500);
                _recordingStoppedInfoTimeout = true;
            }).Start();
        }

        public void SwitchUIApplicationMode(Configuration.ApplicationMode applicationMode)
        {
            if (applicationMode == Configuration.ApplicationMode.AutonomousAgent)
            {
                ApplicationMode.GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("Images/ColorfulButtons/OnRobot");
            }
            else
            {
                ApplicationMode.GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("Images/ColorfulButtons/OffRobot");
            }
        }

        public void Update()
        {
            if (_recordingStoppedInfoTimeout)
            {
                AudioRecordingStoppedInfo.SetActive(false);
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