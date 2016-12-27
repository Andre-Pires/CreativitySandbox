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
        public Configuration.ApplicationMode ActiveMode = Configuration.ApplicationMode.AutonomousAgent;

        // Use this for initialization
        public void Start()
        {

            _scene = GameObject.Find("Scene");

            _scene.AddComponent<SessionLogger>();
            SessionLogger.Instance.WriteToLogFile("Application initialization started.");

            if (Configuration.Instance.SoundRecordingActive)
            {
                //to allow the recording of messages
                _scene.AddComponent<MicrophoneInput>();
            }
            else
            {
                SessionLogger.Instance.WriteToLogFile("Sound recording deactivated.");
            }

            //Camera control script
            Camera.main.gameObject.AddComponent<ThirdPersonCamera>();

            //in order to control what is drawn this script needs to be associated with the camera object
            Camera.main.gameObject.AddComponent<ScreenRecorder>();

            //retrieve in-editor configurations of a few aspects
            _configuration = _scene.GetComponent<Configuration>();

            //NOTE: should run last to allow the remaining components to setup first
            _UIManager = _scene.GetComponent<AppUIManager>();
            SessionLogger.Instance.WriteToLogFile("Application UI Manager and Configuration bound.");

            AppUIManager.Instance.ApplicationMode.GetComponent<Button>().onClick.AddListener(UpdateApplicationMode);
            AppUIManager.Instance.SwitchUIApplicationMode(ActiveMode);

            _agent = new Agent.Agent(ActiveMode);

            SessionLogger.Instance.WriteToLogFile("Application initialization complete.");
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
            switch (ActiveMode)
            {
                case Configuration.ApplicationMode.AutonomousAgent:
                    ActiveMode = Configuration.ApplicationMode.ManuallyActivatedAgent;
                    break;
                case Configuration.ApplicationMode.ManuallyActivatedAgent:
                    ActiveMode = Configuration.ApplicationMode.AutonomousAgent;
                    break;
                default:
                    throw new InvalidEnumArgumentException("Didn't pair with the implemented application modes");
            }

            SessionLogger.Instance.WriteToLogFile("Application mode switched to " + ActiveMode);

            AppUIManager.Instance.SwitchUIApplicationMode(ActiveMode);
            _agent.CurrentApplicationMode = ActiveMode;
        }
    }
}