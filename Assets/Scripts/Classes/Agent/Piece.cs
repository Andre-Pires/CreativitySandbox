using System;
using System.Collections.Generic;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.IO;
using Assets.Scripts.Classes.UI;
using Assets.Scripts.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Classes.Agent
{
    public class Piece
    {
        public string Name { get; set; }
        private readonly GameObject _root;
        private readonly GameObject _cubeObject;
        private readonly Mind _mind;
        private readonly Body _body;

        //double click - play recordings
        private float _initialTime;
        private bool _firstClick;
        private readonly float _interval = 0.6f;
        private readonly int _maxNumberOfStoredClips = 1;
        private int _currentClipIndex;
        private bool _isRecording;

        private Dictionary<int, AudioClip> _clips;
        private MicrophoneInput _micInput;
        private bool _clickForStartStop;
        private AudioSource _clipPlayer;
        private Transform _speechButton;
        
        //double click - to show settings popup
        private bool _popupActive;
        private readonly GameObject _settingsPopup;
        private bool _firstClickPopup;
        private float _initialTimePopup;

        private RaycastHit _hit;
        private Ray _ray;

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

                //since there's no way to raycast UI properly, a clickable backdrop was 
                // added to close the popup - a fixed resolution is sufficient for our application
                GameObject closeBackdrop = Utility.GetChild(settingsPopup, "Background");
                closeBackdrop.GetComponent<Button>().onClick.AddListener(ClosePopup);
                closeBackdrop.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);

                settingsPopup.SetActive(false);
            }
            else
            {
                throw new NullReferenceException("The settings popup prefab wasn't properly loaded");
            }
        }

        private void ClosePopup()
        {
            _popupActive = false;
            _settingsPopup.SetActive(false);
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
        }

        public void DestroyPiece()
        {
            Object.Destroy(_cubeObject);
            Object.Destroy(_settingsPopup);
        }

        public void UpdateBodySize(Configuration.Size size)
        {
            _body.UpdateSize(size);
            Debug.Log("New Size: " + size);
        }

        public void UpdateBodyColor(Color color)
        {
            _body.UpdateColor(color);
            Debug.Log("New color: " + color);
        }

        public void UpdateBlinkSpeed(Configuration.BlinkingSpeed speed)
        {
            _body.UpdateBlinkSpeed(speed);
            Debug.Log("New blink speed: " + speed);
        }

        public void UpdateBlinkColor(Color color)
        {
            _body.UpdateBlinkColor(color);
            Debug.Log("New blink color: " + color);
        }
    }
}