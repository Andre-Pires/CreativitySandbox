using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Classes.Agent;
using Assets.Scripts.Scripts.UI;
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
        }

        // ReSharper disable once InconsistentNaming
        //necessary since if gameObjects start inactive their associated scripts aren't accessible
        public void SetupUI()
        {
            GameObject.Find("MainMenu").gameObject.SetActive(false);
            GameObject.Find("ForestSetup").gameObject.SetActive(false);
            GameObject.Find("CitySetup").gameObject.SetActive(false);
            GameObject.Find("BlankSetup").gameObject.SetActive(false);
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