using UnityEngine;

namespace Assets.Scripts.Classes.UI
{
    public class UIManager
    {
        private static UIManager _instance;

        private GameObject _audioRecordingWarning;
        
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

        // Construct 	
        private UIManager()
        {
            BindUIObjects();
            SetupUI();
        }

        private void BindUIObjects()
        {
            _audioRecordingWarning = GameObject.Find("AudioRecordingWarning").gameObject;
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

            _audioRecordingWarning.SetActive(false);
        }

        public void DisplayRecordingWarning()
        {
            _audioRecordingWarning.SetActive(true);
        }

        public void HideRecordingWarning()
        {
            _audioRecordingWarning.SetActive(false);
        }
    }
}