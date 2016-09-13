using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Classes.UI
{
    public class UIManager
    {
        private static UIManager _instance;

        private GameObject _audioRecordingStartInfo;
        private GameObject _audioRecordingStoppedInfo;
        private bool _recordingStoppedInfoTimeout;

        // Construct 	
        private UIManager()
        {
            BindUIObjects();
            SetupUI();
        }

        //  Instance 	
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UIManager();
                return _instance;
            }
        }

        private void BindUIObjects()
        {
            _audioRecordingStartInfo = GameObject.Find("AudioRecordingWarning").gameObject;
            _audioRecordingStoppedInfo = GameObject.Find("AudioRecordingSuccessful").gameObject;
        }


        // ReSharper disable once InconsistentNaming
        //necessary since if gameObjects start inactive their associated scripts aren't accessible
        public void SetupUI()
        {
            GameObject.Find("MainMenu").gameObject.SetActive(false);
            GameObject.Find("AgentSetup").gameObject.SetActive(false);
            GameObject.Find("ForestDaySetup").gameObject.SetActive(false);
            GameObject.Find("CityDaySetup").gameObject.SetActive(false);
            GameObject.Find("Pause").gameObject.SetActive(false);
            GameObject.Find("RecordingControls").gameObject.SetActive(false);
            GameObject.Find("SceneSelector").gameObject.SetActive(false);

            _audioRecordingStartInfo.SetActive(false);
            _audioRecordingStoppedInfo.SetActive(false);
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
        }

        public void OnGUI()
        {
        }
    }
}