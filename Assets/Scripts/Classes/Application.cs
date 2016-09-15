using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.IO;
using Assets.Scripts.Classes.UI;
using Assets.Scripts.Scripts.CameraControl;
using UnityEngine;

namespace Assets.Scripts.Classes
{
    public class Application : MonoBehaviour
    {
        private Agent.Agent _agent;
        private Configuration _configuration;
        private GameObject _scene;
        private UIManager _UIManager;

        // Use this for initialization
        public void Start()
        {
            //to allow the recording of messages
            _scene = GameObject.Find("Scene");
            _scene.AddComponent<MicrophoneInput>();

            _agent = new Agent.Agent();

            //Camera control script
            Camera.main.gameObject.AddComponent<ThirdPersonCamera>();

            //in order to control what is drawn this script needs to be associated with the camera object
            Camera.main.gameObject.AddComponent<ScreenRecorder>();

            //retrieve in-editor configurations of a few aspects
            _configuration = _scene.GetComponent<Configuration>();

            //NOTE: should run last to allow the remaining components to setup first
            _UIManager = UIManager.Instance;
        }

        // Update is called once per frame
        public void Update()
        {
            if (_agent != null)
                _agent.Update();

            if (Input.GetKeyDown(KeyCode.Escape))
                UnityEngine.Application.Quit();

            _UIManager.Update();
        }

        public void OnDrawGizmos()
        {
            if (!UnityEngine.Application.isPlaying) return;

            _agent.OnDrawGizmos();
        }

        public void OnGUI()
        {
            _agent.OnGUI();
            _UIManager.OnGUI();
        }
    }
}