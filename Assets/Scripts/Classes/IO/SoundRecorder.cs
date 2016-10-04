using System.Collections.Generic;
using System.Threading;
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
        private const float ClickInterval = 0.6f;
        private const int MaxNumberOfStoredClips = 1;
        private int _currentClipIndex;
        public bool IsRecording;

        private Dictionary<int, AudioClip> _clips;
        private MicrophoneInput _micInput;
        private bool _clickForPlay;
        private AudioSource _clipPlayer;
        private Transform _speechButton;

        private readonly string _recordingName;

        //managing playing only a segment
        private bool _timeToStop;
        public readonly float MinClipDuration = 0.5f;
        public readonly float MaxClipDuration = 2.0f;

        public SoundRecorder(string name, GameObject cubePrefab, GameObject root)
        {
            _recordingName = name;

            _speechButton = Utility.GetChild(cubePrefab, "Button").transform;

            //get recording script
            _micInput = root.GetComponent<MicrophoneInput>();

            _clips = new Dictionary<int, AudioClip>(MaxNumberOfStoredClips);

            //for accurate sound clip playback
            cubePrefab.AddComponent<AudioSource>();
            _clipPlayer = cubePrefab.GetComponent<AudioSource>();
        }


        public void Update()
        {
            HandleSoundInputStatus();
            HandleSoundOutputStatus();
        }

        private void HandleSoundInputStatus()
        {
            // On double click play recorded messages
            if (_firstClick)
            {
                if (Time.time - _initialTime > ClickInterval)
                {
                    _firstClick = false;
                    _clickForPlay = true;
                }
                else if (Input.GetMouseButtonDown(0) && Utility.Instance.CheckIfClicked(_speechButton))
                {
                    _firstClick = false;

                    //check if device's microphone and the piece itself aren't already recording
                    if (!Microphone.IsRecording(_micInput.SelectedDevice) && !IsRecording)
                    {
                        StartRecording();
                    }
                    else if (Microphone.IsRecording(_micInput.SelectedDevice) && IsRecording)
                    {
                        StopRecording();
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
            if (_clickForPlay || Input.GetMouseButtonDown(0) && Utility.Instance.CheckIfClicked(_speechButton))
            {
                //PlayRecording();
                PlayRecordingSegment();
                _clickForPlay = false;
            }
        }

        private void HandleSoundOutputStatus()
        {

            if (_timeToStop)
            {
                Debug.Log("Cut time now.");
                _clipPlayer.Stop();
                _timeToStop = false;
            }
        }

        public void StopRecording()
        {
            _clipPlayer.PlayOneShot(Resources.Load<AudioClip>("Sounds/RecordingStartStop/beepbeep"));
            _micInput.StopMicrophone(_recordingName + _currentClipIndex);
            _clips[_currentClipIndex] = _micInput.GetLastRecording();
            Debug.Log("Stopped recording clip " + _currentClipIndex);

            _currentClipIndex = (_currentClipIndex + 1)%MaxNumberOfStoredClips;
            IsRecording = false;
            AppUIManager.Instance.DisplayRecordingStopped();
        }

        public void StartRecording()
        {
            _clipPlayer.PlayOneShot(Resources.Load<AudioClip>("Sounds/RecordingStartStop/beepbeep"));
            _micInput.StartMicrophone();
            IsRecording = true;
            AppUIManager.Instance.DisplayRecordingStarted();
            Debug.Log("Started recording");
        }

        private void PlayRecording()
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

        private void PlayRecordingSegment()
        {
            if (!_clipPlayer.isPlaying && _clips.Count > 0)
            {
                //random range max is exclusive
                var recordingIndex = Random.Range(0, _clips.Count);
                _clipPlayer.clip = _clips[recordingIndex];
                float playTimeSeconds = Random.Range(MinClipDuration, MaxClipDuration);
                while (_clipPlayer.clip.length < playTimeSeconds)
                {
                    playTimeSeconds -= 0.5f;
                }
                float playStartTime = Random.Range(0.0f, _clipPlayer.clip.length - playTimeSeconds);

                Debug.Log("clip has " + _clipPlayer.clip.length + " we start at " + playStartTime + ", the interval is " + playTimeSeconds);

                float timeReal = _clipPlayer.clip.length; //The number we will use to offset the audioclip will not actually be in seconds, it will be in the segments the compression slices it into.
                int segmentNumber = _clipPlayer.clip.samples;  //This is the actual number of segments of the audio clip.
                var timePerSegment = timeReal / segmentNumber;
                _clipPlayer.timeSamples = (int)(playStartTime / timePerSegment);
                _clipPlayer.Play(); //Play the audio and hear the effect.

                new Thread(() =>
                {
                    Thread.Sleep((int) (playTimeSeconds * 1000));
                    _timeToStop = true;
                }).Start();

                Debug.Log("Playing recording number " + recordingIndex);
            }
        }
    }
}