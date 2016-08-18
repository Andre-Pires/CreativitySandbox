using System;
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

        //NOTE: change to switch talking mode
        public MicActivation MicControl = MicActivation.HoldToSpeak;

        public string FileName = "default";
        public string FilePath;
        
        public float Sensitivity = 100;
        public float RamFlushSpeed = 30;//The smaller the number the faster it flush's the ram, but there might be performance issues...
        [Range(0, 100)]
        public float SourceVolume = 100;//Between 0 and 100
        public bool GuiSelectDevice = true;
        //
        public string SelectedDevice { get; private set; }
        public float Loudness { get; private set; } //dont touch
        //
        private bool _micSelected = false;
        private float _ramFlushTimer;
        private float _ramFlushStartTime;
        private int _amountSamples = 256; //increase to get better average, but will decrease performance. Best to leave it
        private int _minFreq, _maxFreq;
        private AudioSource _audio;

        void Start()
        {

            #if UNITY_ANDROID
                FilePath = UnityEngine.Application.persistentDataPath + "/ProjectData/Sounds/";
            #endif

            #if UNITY_STANDALONE || UNITY_EDITOR
            FilePath = "../" + AppDomain.CurrentDomain.BaseDirectory + "/ProjectData/Sounds/";
            #endif
            Debug.Log("Mic setup - OK");
            _audio = gameObject.GetComponent<AudioSource>();
            _audio.loop = true; // Set the AudioClip to loop
            _audio.mute = false; // Mute the sound, we don't want the player to hear it
            SelectedDevice = Microphone.devices[0].ToString();
            _micSelected = true;
            GetMicCaps();
        }

        void OnGui()
        {
            MicDeviceGui((Screen.width / 2) - 150, (Screen.height / 2) - 75, 300, 100, 10, -300);
            if (Microphone.IsRecording(SelectedDevice))
            {
                _ramFlushTimer = Time.time - _ramFlushStartTime;
                RamFlush();
            }
        }

    
        public void MicDeviceGui(float left, float top, float width, float height, float buttonSpaceTop, float buttonSpaceLeft)
        {
            if (Microphone.devices.Length > 1 && GuiSelectDevice == true || _micSelected == false)//If there is more than one device, choose one.
                for (int i = 0; i < Microphone.devices.Length; ++i)
                    if (GUI.Button(new Rect(left + ((width + buttonSpaceLeft) * i), top + ((height + buttonSpaceTop) * i), width, height), Microphone.devices[i].ToString()))
                    {
                        StopMicrophone(FileName);
                        SelectedDevice = Microphone.devices[i].ToString();
                        GetMicCaps();
                        StartMicrophone();
                        _micSelected = true;
                    }
            if (Microphone.devices.Length < 2 && _micSelected == false)
            {//If there is only 1 decive make it default
                SelectedDevice = Microphone.devices[0].ToString();
                GetMicCaps();
                _micSelected = true;
            }
        }

        public void GetMicCaps()
        {
            Microphone.GetDeviceCaps(SelectedDevice, out _minFreq, out _maxFreq);//Gets the frequency of the device
            if ((_minFreq + _maxFreq) == 0)//These 2 lines of code are mainly for windows computers
                _maxFreq = 44100;
        }



        public void StartMicrophone()
        {
            _audio.clip = Microphone.Start(SelectedDevice, true, 30, _maxFreq);//Starts recording
            while (!(Microphone.GetPosition(SelectedDevice) > 0)) { } // Wait until the recording has started

            //For debugging purposes - Play the audio source!
            //_audio.Play();
            _ramFlushStartTime = Time.time;
        }

        public AudioClip StopMicrophone(string fileName)
        {
            if (Microphone.IsRecording(SelectedDevice))
            {
                _audio.Stop();//Stops the audio
                int lastSample  = Microphone.GetPosition(null); //using default
                Microphone.End(SelectedDevice);//Stops the recording of the device

                _audio.clip = TrimAudioClip(_audio.clip, lastSample);

                SavWav.Save(FilePath,fileName, _audio.clip);

                return _audio.clip;
            }

            return null;
        }

        public AudioClip GetLastRecording()
        {
            return _audio.clip;
        }

        void Update()
        {
            _audio.volume = (SourceVolume / 100);
            Loudness = GetAveragedVolume() * Sensitivity * (SourceVolume / 10);

            //TODO - might be interesting in the future, for now behavior is in Piece Class
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

        float GetAveragedVolume()
        {
            float[] data = new float[_amountSamples];
            float a = 0;
            _audio.GetOutputData(data, 0);
            foreach (float s in data)
            {
                a += Mathf.Abs(s);
            }
            return a / _amountSamples;
        }

        private AudioClip TrimAudioClip(AudioClip originalClip, int lastSample)
        {
            float[] samples = new float[originalClip.samples]; //
            originalClip.GetData(samples, 0);
            float[] clipSamples = new float[lastSample];
            System.Array.Copy(samples, clipSamples, clipSamples.Length - 1);
            AudioClip clip = new AudioClip();
            clip = AudioClip.Create("playRecordClip", clipSamples.Length, 1, 44100, false);
            clip.SetData(clipSamples, 0);

            return clip;
        }
    }
}