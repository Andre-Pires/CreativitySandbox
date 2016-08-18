using System.Collections.Generic;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.IO;
using Assets.Scripts.Scripts;
using Assets.Scripts.Scripts.CameraControl;
using UnityEngine;

namespace Assets.Scripts.Classes
{
    public class Application : MonoBehaviour
    {
        
        private Agent.Agent _agent;
        private GameObject _scene;
        private Configuration _configuration;
        private ScreenRecorder _screenRecorder;

        // Use this for initialization
        private void Start()
        {
            //to allow the recording of messages
            _scene = GameObject.Find("Scene");
            _scene.AddComponent<MicrophoneInput>();

            _agent = new Agent.Agent();

            //Camera control script
            Camera.main.gameObject.AddComponent<ThirdPersonCamera>();

            //in order to control what is drawn this script needs to be associated with the camera object
            Camera.main.gameObject.AddComponent<ScreenRecorder>();
            _screenRecorder = Camera.main.gameObject.GetComponent<ScreenRecorder>();

            //retrieve in-editor configurations of a few aspects
            _configuration = _scene.GetComponent<Configuration>();

            //NOTE: should run last to allow the remaining components to setup first
            SetupUI();
        }

        // Update is called once per frame
        private void Update()
        {
            if(_agent != null)
                _agent.Update();

            if (Input.GetKeyDown(KeyCode.Escape))
                UnityEngine.Application.Quit();
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
        }

        void OnDrawGizmos()
        {
            if(!UnityEngine.Application.isPlaying) return;

            _agent.OnDrawGizmos();
        }
    }
}