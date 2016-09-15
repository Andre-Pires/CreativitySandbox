using System.Threading;
using Assets.Scripts.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Classes.UI
{
    public class UIManager
    {
        private static UIManager _instance;

        private GameObject _audioRecordingStartInfo;
        private GameObject _audioRecordingStoppedInfo;
        private GameObject _availableAgentPiecesList;
        private bool _recordingStoppedInfoTimeout;

        public delegate void OnDestroyPieceEvent(string pieceName);
        public event OnDestroyPieceEvent DestroyAgentPiece;

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
            _availableAgentPiecesList = GameObject.Find("AvailableAgentPieces/List").gameObject;
        }


        // ReSharper disable once InconsistentNaming
        //necessary since if gameObjects start inactive their associated scripts aren't accessible
        public void SetupUI()
        {
            GameObject.Find("MainMenu").gameObject.SetActive(false);
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

        public void AddNewAgentPieceUI(string name)
        {
            GameObject newPiece = Object.Instantiate(Resources.Load("Prefabs/UISettings/AgentPieceItem")) as GameObject;
            newPiece.GetComponentInChildren<Text>().text = name + "_Button";
            newPiece.name = name + "_Button";
            newPiece.GetComponent<ManagePieceInstances>().OnSelect += DestroyAgentPieceUI;
            newPiece.GetComponent<ManagePieceInstances>().PieceName = name;
            newPiece.transform.SetParent(_availableAgentPiecesList.transform, false);
        }

        public void DestroyAgentPieceUI(string name)
        {
            DestroyAgentPiece(name);
            Object.Destroy(GameObject.Find(name + "_Button"));
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