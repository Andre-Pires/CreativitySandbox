using System.ComponentModel;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.IO;
using Assets.Scripts.Classes.UI;
using Assets.Scripts.Scripts.CameraControl;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Classes
{
    public class Application : MonoBehaviour
    {
        private Agent.Agent _agent;
        private Configuration _configuration;
        private GameObject _scene;
        private AppUIManager _UIManager;
        private Configuration.ApplicationMode _activeMode = Configuration.ApplicationMode.AutonomousAgent;

        // Use this for initialization
        public void Start()
        {
            //to allow the recording of messages
            _scene = GameObject.Find("Scene");

            if (Configuration.Instance.SoundRecordingActive)
            {
                _scene.AddComponent<MicrophoneInput>();
            }

            //Camera control script
            Camera.main.gameObject.AddComponent<ThirdPersonCamera>();

            //in order to control what is drawn this script needs to be associated with the camera object
            Camera.main.gameObject.AddComponent<ScreenRecorder>();

            //retrieve in-editor configurations of a few aspects
            _configuration = _scene.GetComponent<Configuration>();

            //NOTE: should run last to allow the remaining components to setup first
            _UIManager = _scene.GetComponent<AppUIManager>();

            AppUIManager.Instance.ApplicationMode.GetComponent<Button>().onClick.AddListener(UpdateApplicationMode);

            _agent = new Agent.Agent(_activeMode);
        }

        // Update is called once per frame
        public void Update()
        {
            if (_agent != null)
                _agent.Update();

            if (Input.GetKeyDown(KeyCode.Escape))
                UnityEngine.Application.Quit();
        }

        public void UpdateApplicationMode()
        {
            switch (_activeMode)
            {
                case Configuration.ApplicationMode.AutonomousAgent:
                    _activeMode = Configuration.ApplicationMode.ManuallyActivatedAgent;
                    break;
                case Configuration.ApplicationMode.ManuallyActivatedAgent:
                    _activeMode = Configuration.ApplicationMode.AutonomousAgent;
                    break;
                default:
                    throw new InvalidEnumArgumentException("Didn't pair with the implemented application modes");
            }

            _agent.CurrentApplicationMode = _activeMode;
        }

        public void OnDrawGizmos()
        {
            if (!UnityEngine.Application.isPlaying) return;

            if (_agent != null)
                _agent.OnDrawGizmos();
        }

        public void OnGUI()
        {
            if (_agent != null)
                _agent.OnGUI();
        }
    }
}