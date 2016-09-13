using System;
using System.Collections.Generic;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.IO;
using Assets.Scripts.Classes.UI;
using Assets.Scripts.Scripts.UI;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Classes.Agent
{
    public class Piece
    {
        private readonly Body _body;
        private bool _clickForStartStop;
        private AudioSource _clipPlayer;
        private Dictionary<int, AudioClip> _clips;
        private readonly GameObject _cubeObject;
        private int _currentClipIndex;

        //double click - play recordings
        private bool _firstClick;
        private bool _firstClickPopup;
        private float _initialTime;
        private float _initialTimePopup;
        private readonly float _interval = 0.6f;
        private bool _isRecording;
        private readonly int _maxNumberOfStoredClips = 1;

        private MicrophoneInput _micInput;
        private readonly Mind _mind;
        private bool _popupActive;
        private readonly GameObject _root;

        //double click - to show settings popup
        private readonly GameObject _settingsPopup;
        // Check for mouse input for speech recording
        private Transform _speechButton;

        public Piece(string name, Configuration.Personality personality, Configuration.Size size)
        {
            Debug.Log("New agent part added: part " + name + ". " + size + " size and " + personality + " personality");

            Name = name;

            _root = GameObject.Find("Scene");
            _cubeObject = Object.Instantiate(Resources.Load("Prefabs/Agent/Cube")) as GameObject;
            SetupPiecePrefab(_cubeObject);

            _cubeObject.AddComponent<Body>();
            _body = _cubeObject.GetComponent<Body>();
            _body.Init(size, _cubeObject.transform);
            _mind = new Mind(personality, _body);

            _settingsPopup = Object.Instantiate(Resources.Load("Prefabs/UISettings/SettingsPopup")) as GameObject;
            SetupPopupPrefab(_settingsPopup);
        }

        public string Name { get; set; }

        private void SetupPiecePrefab(GameObject cubePrefab)
        {
            if (cubePrefab != null)
            {
                cubePrefab.name = Name;
                cubePrefab.tag = "Cube";
                cubePrefab.transform.parent = _root.transform;
                _speechButton = Utility.GetChild(cubePrefab, "Button").transform;

                //get recording script
                _micInput = _root.GetComponent<MicrophoneInput>();
                _clips = new Dictionary<int, AudioClip>(_maxNumberOfStoredClips);

                //for accurate sound clip playback
                cubePrefab.AddComponent<AudioSource>();
                _clipPlayer = cubePrefab.GetComponent<AudioSource>();
            }
            else
            {
                throw new NullReferenceException("The cube's prefab wasn't properly loaded");
            }
        }

        private void SetupPopupPrefab(GameObject settingsPopup)
        {
            if (settingsPopup != null)
            {
                settingsPopup.transform.SetParent(GameObject.Find("Canvas").transform, false);
                settingsPopup.GetComponent<UpdateSettings>().Piece = this;
                settingsPopup.name = Name + "_Popup";
                settingsPopup.SetActive(false);
            }
            else
            {
                throw new NullReferenceException("The settings popup prefab wasn't properly loaded");
            }
        }

        public void Update()
        {
            //Update Mind and Body
            _mind.Update();
            _body.Update();

            HandleSettingsPopup();
            HandleSoundInputStatus();
        }

        private void HandleSettingsPopup()
        {
            if (_popupActive)
            {
                /*if (Input.GetMouseButtonDown(0) && !Utility.Instance.CheckIfClicked(_settingsPopup.transform))
                {
                    _popupActive = false;
                    _settingsPopup.SetActive(false);
                    return;
                }*/

                var popupSize = _settingsPopup.GetComponent<RectTransform>().rect.height;
                var screenPos = Camera.main.WorldToScreenPoint(_cubeObject.transform.position);
                screenPos.y += popupSize;
                _settingsPopup.transform.position = screenPos;

                return;
            }

            // On double click play recorded messages
            if (_firstClickPopup)
            {
                if (Time.time - _initialTimePopup > _interval)
                {
                    _firstClickPopup = false;
                }
                else if (Input.GetMouseButtonDown(0) && Utility.Instance.CheckIfClicked(_cubeObject.transform))
                {
                    _firstClickPopup = false;
                    _popupActive = true;
                    _settingsPopup.SetActive(true);
                }
            }
            else if (Input.GetMouseButtonDown(0) && Utility.Instance.CheckIfClicked(_cubeObject.transform))
            {
                _firstClickPopup = true;
                _initialTimePopup = Time.time;
            }
        }

        private void HandleSoundInputStatus()
        {
            // On double click play recorded messages
            if (_firstClick)
            {
                if (Time.time - _initialTime > _interval)
                {
                    _firstClick = false;
                    _clickForStartStop = true;
                }
                else if (Input.GetMouseButtonDown(0) && Utility.Instance.CheckIfClicked(_speechButton))
                {
                    _firstClick = false;

                    if (!_clipPlayer.isPlaying && _clips.Count > 0)
                    {
                        //random range max is exclusive
                        var recordingIndex = Random.Range(0, _clips.Count);
                        _clipPlayer.clip = _clips[recordingIndex];
                        _clipPlayer.Play();
                        Debug.Log("Playing recording number " + recordingIndex);
                    }

                    return;
                }
            }
            else if (Input.GetMouseButtonDown(0) && Utility.Instance.CheckIfClicked(_speechButton))
            {
                _firstClick = true;
                _initialTime = Time.time;

                return;
            }


            // Single click to stop and start recording
            if (_clickForStartStop || Input.GetMouseButtonDown(0) && Utility.Instance.CheckIfClicked(_speechButton))
            {
                //check if device's microphone and the piece itself aren't already recording
                if (!Microphone.IsRecording(_micInput.SelectedDevice) && !_isRecording)
                {
                    _micInput.StartMicrophone();
                    _isRecording = true;
                    Debug.Log("Started recording");
                    UIManager.Instance.DisplayRecordingStarted();
                }
                else if (Microphone.IsRecording(_micInput.SelectedDevice) && _isRecording)
                {
                    _micInput.StopMicrophone(Name + _currentClipIndex);
                    _clips[_currentClipIndex] = _micInput.GetLastRecording();
                    Debug.Log("Stopped recording clip " + _currentClipIndex);

                    _currentClipIndex = (_currentClipIndex + 1)%_maxNumberOfStoredClips;
                    _isRecording = false;
                    UIManager.Instance.DisplayRecordingStopped();
                }

                _clickForStartStop = false;
            }
        }

        public void OnDrawGizmos()
        {
            /*
            Gizmos.color = new Color(1, 0, 0, 0.5F);
            Gizmos.DrawCube(cubeObject.transform.position + Vector3.up * 10, new Vector3(1, 1, 1));
            */
        }

        public void OnGUI()
        {
            /*if (!_showPopup) return;

            // Get the screen position of the NPC's origin:
            Vector3 screenPos = Camera.main.WorldToScreenPoint(_cubeObject.transform.position);
            // Define a 100x100 pixel rect going up and to the right:
            GameObject.Find("UI Button").transform.position = screenPos;
            // Draw a label in the rect:*/
        }

        public void UpdateSettings(Configuration.Size size)
        {
            Debug.Log("New Size: " + size);
        }

        public void UpdateSettings(Color color)
        {
            Debug.Log("New color: " + color);
        }

        public void UpdateSettings(Configuration.BlinkingSpeed speed)
        {
            Debug.Log("New blink speed: " + speed);
        }
    }
}