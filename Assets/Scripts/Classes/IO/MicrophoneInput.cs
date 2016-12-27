using System;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.IO
{
    [RequireComponent(typeof(AudioSource))]
    public class MicrophoneInput : MonoBehaviour
    {
        public enum MicActivation
        {
            HoldToSpeak,
            PushToSpeak,
            ConstantSpeak
        }

        private readonly int _amountSamples = 256;
            //increase to get better average, but will decrease performance. Best to leave it

        private AudioSource _audio;
        //
        private bool _micSelected;
        private int _minFreq, _maxFreq;
        private float _ramFlushStartTime;
        private float _ramFlushTimer;

        public string FileName = "default";
        public string FilePath = Constants.SoundFilePath;
        public bool GuiSelectDevice = true;

        //NOTE: change to switch talking mode
        public MicActivation MicControl = MicActivation.HoldToSpeak;

        public float RamFlushSpeed = 30;
            //The smaller the number the faster it flush's the ram, but there might be performance issues...

        public float Sensitivity = 100;

        [Range(0, 100)] public float SourceVolume = 100; //Between 0 and 100
        public int ClipMaxLength = 60;

        //
        public string SelectedDevice { get; private set; }
        public float Loudness { get; private set; } //dont touch

        private void Start()
        {
            _audio = gameObject.GetComponent<AudioSource>();
            _audio.loop = true; // Set the AudioClip to loop
            _audio.mute = false; // Mute the sound, we don't want the player to hear it
            SelectedDevice = Microphone.devices[0];
            _micSelected = true;
            GetMicCaps();

            Debug.Log("Sound storage path: " + FilePath);
            SessionLogger.Instance.WriteToLogFile("Sound recording activate and initialization complete.");
        }

        private void OnGui()
        {
            MicDeviceGui(Screen.width/2 - 150, Screen.height/2 - 75, 300, 100, 10, -300);
            if (Microphone.IsRecording(SelectedDevice))
            {
                _ramFlushTimer = Time.time - _ramFlushStartTime;
                RamFlush();
            }
        }


        public void MicDeviceGui(float left, float top, float width, float height, float buttonSpaceTop,
            float buttonSpaceLeft)
        {
            if (Microphone.devices.Length > 1 && GuiSelectDevice || _micSelected == false)
                //If there is more than one device, choose one.
                for (var i = 0; i < Microphone.devices.Length; ++i)
                    if (
                        GUI.Button(
                            new Rect(left + (width + buttonSpaceLeft)*i, top + (height + buttonSpaceTop)*i, width,
                                height), Microphone.devices[i]))
                    {
                        StopMicrophone(FileName);
                        SelectedDevice = Microphone.devices[i];
                        GetMicCaps();
                        StartMicrophone();
                        _micSelected = true;
                    }
            if (Microphone.devices.Length < 2 && _micSelected == false)
            {
                //If there is only 1 device make it default
                SelectedDevice = Microphone.devices[0];
                GetMicCaps();
                _micSelected = true;
            }
        }

        public void GetMicCaps()
        {
            Microphone.GetDeviceCaps(SelectedDevice, out _minFreq, out _maxFreq); //Gets the frequency of the device
            if (_minFreq + _maxFreq == 0) //These 2 lines of code are mainly for windows computers
                _maxFreq = 44100;
        }


        public void StartMicrophone()
        {
            _audio.clip = Microphone.Start(SelectedDevice, true, ClipMaxLength, _maxFreq); //Starts recording
            while (!(Microphone.GetPosition(SelectedDevice) > 0))
            {
            } // Wait until the recording has started

            _ramFlushStartTime = Time.time;
        }

        public AudioClip StopMicrophone(string fileName)
        {
            if (Microphone.IsRecording(SelectedDevice))
            {
                _audio.Stop(); //Stops the audio
                var lastSample = Microphone.GetPosition(null); //using default
                Microphone.End(SelectedDevice); //Stops the recording of the device

                _audio.clip = TrimAudioClip(_audio.clip, lastSample);

                SavWav.Save(FilePath, fileName, _audio.clip);

                return _audio.clip;
            }

            return null;
        }

        public AudioClip GetLastRecording()
        {
            return _audio.clip;
        }

        private void Update()
        {
            _audio.volume = SourceVolume/100;
            Loudness = GetAveragedVolume()*Sensitivity*(SourceVolume/10);

            //NOTE - might be interesting in the future, for now behavior is in SoundRecorder Class
            /*
        //Hold To Speak!!
        if (micControl == micActivation.HoldToSpeak)
        {
            if (Microphone.IsRecording(selectedDevice) && Input.GetKey(KeyCode.T) == false)
                StopMicrophone();
            //
            if (Input.GetKeyDown(KeyCode.T)) //Push to talk
                StartMicrophone();
            //
            if (Input.GetKeyUp(KeyCode.T))
                StopMicrophone();
            //
        }
        //Push To Talk!!
        if (micControl == micActivation.PushToSpeak)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (Microphone.IsRecording(selectedDevice))
                    StopMicrophone();

                else if (!Microphone.IsRecording(selectedDevice))
                    StartMicrophone();
            }

        }
        //Constant Speak!!
        if (micControl == micActivation.ConstantSpeak)
            if (!Microphone.IsRecording(selectedDevice))
                StartMicrophone();
        //
        if (Input.GetKeyDown(KeyCode.G))
            micSelected = false;
        */
        }

        private void RamFlush()
        {
            if (_ramFlushTimer >= RamFlushSpeed && Microphone.IsRecording(SelectedDevice))
            {
                StopMicrophone(FileName);
                StartMicrophone();
                _ramFlushTimer = 0;
            }
        }

        private float GetAveragedVolume()
        {
            var data = new float[_amountSamples];
            float a = 0;
            _audio.GetOutputData(data, 0);
            foreach (var s in data)
            {
                a += Mathf.Abs(s);
            }
            return a/_amountSamples;
        }

        private AudioClip TrimAudioClip(AudioClip originalClip, int lastSample)
        {
            var samples = new float[originalClip.samples]; //
            originalClip.GetData(samples, 0);
            var clipSamples = new float[lastSample];
            Array.Copy(samples, clipSamples, clipSamples.Length - 1);
            var clip = new AudioClip();
            clip = AudioClip.Create("playRecordClip", clipSamples.Length, 1, 44100, false);
            clip.SetData(clipSamples, 0);

            return clip;
        }
    }
}
