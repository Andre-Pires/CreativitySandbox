using Assets.Scripts.Classes.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scripts.UI
{
    public class ChangeCameraMode : MonoBehaviour
    {
        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

        // Singleton 	
        private static ChangeCameraMode _instance;
        private bool _scenarioMode = true;
        
        // Construct 	
        private ChangeCameraMode()
        {
        }

        //  Instance 	
        public static ChangeCameraMode Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType(typeof(ChangeCameraMode)) as ChangeCameraMode;
                return _instance;
            }
        }

        public void Awake()
        {
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/ColorfulButtons/CameraModeScenario");
            GetComponentInChildren<Text>().text = Constants.ScenarioCameraMode;
            _scenarioMode = true;
        }


        // Handle our Ray and Hit
        private void Update()
        {
        }

        public void OnClick()
        {
            if (_scenarioMode)
            {
                GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/ColorfulButtons/CameraModeCharacter");
                GetComponentInChildren<Text>().text = Constants.CharacterCameraMode;
                _scenarioMode = false;
            }
            else
            {
                GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/ColorfulButtons/CameraModeScenario");
                GetComponentInChildren<Text>().text = Constants.ScenarioCameraMode;
                _scenarioMode = true;
            }
            // Notify of the event!
            OnSelect();
        }
    }
}