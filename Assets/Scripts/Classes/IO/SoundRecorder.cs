using System.Collections.Generic;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Classes.IO
{
    public class SoundRecorder
    {
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

        private string _recordingName;
        
        public SoundRecorder(string name, GameObject cubePrefab, GameObject root)
        {
            _recordingName = name;

            _speechButton = Utility.GetChild(cubePrefab, "Button").transform;

            //get recording script
            _micInput = root.GetComponent<MicrophoneInput>();

            _clips = new Dictionary<int, AudioClip>(_maxNumberOfStoredClips);

            //for accurate sound clip playback
            cubePrefab.AddComponent<AudioSource>();
            _clipPlayer = cubePrefab.GetComponent<AudioSource>();
        }


        public void Update()
        {
            HandleSoundInputStatus();
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

                    PlayLastRecording();
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
                    StartRecording();
                }
                else if (Microphone.IsRecording(_micInput.SelectedDevice) && _isRecording)
                {
                    StopRecording();
                }

                _clickForStartStop = false;
            }
        }

        private void StopRecording()
        {
            _clipPlayer.PlayOneShot(Resources.Load<AudioClip>("Sounds/RecordingStartStop/beepbeep"));
            _micInput.StopMicrophone(_recordingName + _currentClipIndex);
            _clips[_currentClipIndex] = _micInput.GetLastRecording();
            Debug.Log("Stopped recording clip " + _currentClipIndex);

            _currentClipIndex = (_currentClipIndex + 1)%_maxNumberOfStoredClips;
            _isRecording = false;
            AppUIManager.Instance.DisplayRecordingStopped();
        }

        private void StartRecording()
        {
            _clipPlayer.PlayOneShot(Resources.Load<AudioClip>("Sounds/RecordingStartStop/beepbeep"));
            _micInput.StartMicrophone();
            _isRecording = true;
            AppUIManager.Instance.DisplayRecordingStarted();
            Debug.Log("Started recording");
        }

        private void PlayLastRecording()
        {
            if (!_clipPlayer.isPlaying && _clips.Count > 0)
            {
                //random range max is exclusive
                var recordingIndex = Random.Range(0, _clips.Count);
                _clipPlayer.clip = _clips[recordingIndex];
                _clipPlayer.Play();
                Debug.Log("Playing recording number " + recordingIndex);
            }
        }
    }
}