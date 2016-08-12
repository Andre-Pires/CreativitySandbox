using System;
using System.Collections.Generic;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.IO;
using Assets.Scripts.Scripts;
using Assets.Scripts.Scripts.UI;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Classes.Agent
{
    public class Piece
    {
        public string Name { get; set; }
        private GameObject _cubeObject;
        private GameObject _root;
        private float _currentRotation = 0.0f;
        
        private Body _body;
        private Mind _mind;

        private MicrophoneInput _micInput;
        private AudioSource _clipPlayer;
        private List<AudioClip> _clips;
        private int _maxNumberOfStoredClips = 3;
        private int _currentClipIndex = 0;

        // Check for mouse input for speech recording
        private Transform _speechButton;

        public Piece(string name, Configuration.Personality personality, Configuration.Size size)
        {
            Debug.Log("New agent part added: part " + name + ". " + size + " size and " + personality + " personality");

            Name = name;

            _root = GameObject.Find("Scene");
            _cubeObject = Object.Instantiate(Resources.Load("Prefabs/Cube")) as GameObject;
            SetupPiecePrefab(_cubeObject);

            _cubeObject.AddComponent<Body>();
            _body = _cubeObject.GetComponent<Body>();
            _body.Init(size, _cubeObject.transform);
            _mind = new Mind(personality, _body);
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
                _clips = new List<AudioClip>();
                _clips.Capacity = 3;

                //for accurate sound clip playback
                cubePrefab.AddComponent<AudioSource>();
                _clipPlayer = cubePrefab.GetComponent<AudioSource>();
            }
            else
            {
                throw new NullReferenceException("The cube's prefab wasn't properly loaded");
            }
        }

        public void Update()
        {
            //Update Mind and Body
            _mind.Update();
            _body.Update();

            CheckSoundInputStatus();
            CheckRotationInput();
            
        }

        private void CheckRotationInput()
        {

            if (Input.GetMouseButton(0))
            {
                if (Utility.Instance.CheckIfClicked(_cubeObject.transform))
                {
                    float lerpSpeed = 100.0f;  //This will determine lerp speed
                    float rotationSpeed = 50.0f;  //This will determine rotation speed

                    //Keeping this approach for touch implementation
                    /*
                    //Get mouse position
                    Vector3 mousePos = Input.mousePosition;

                    //Adjust mouse z position
                    mousePos.z = Camera.main.transform.position.y - _cubeObject.transform.position.y;

                    //Get a world position for the mouse
                    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

                    //Get the angle to rotate and rotate
                    Debug.Log("wheel " + Input.GetAxis("Mouse ScrollWheel"));
                    float angle = -Mathf.Atan2(_cubeObject.transform.position.z - mouseWorldPos.z, _cubeObject.transform.position.x - mouseWorldPos.x) * Mathf.Rad2Deg;
                    _cubeObject.transform.rotation = Quaternion.Slerp(_cubeObject.transform.rotation, Quaternion.Euler(0, Input.GetAxis("Mouse ScrollWheel") * 50, 0), rotationSpeed * Time.deltaTime);
                    */

                    _currentRotation += Input.GetAxis("Mouse ScrollWheel") * rotationSpeed;
                    _cubeObject.transform.rotation = Quaternion.Slerp(_cubeObject.transform.rotation, Quaternion.Euler(0, _currentRotation , 0), lerpSpeed * Time.deltaTime);
                }
            }
        }

        private void CheckSoundInputStatus()
        {
            if (!Microphone.IsRecording(_micInput.SelectedDevice) && Input.GetMouseButtonDown(0))
            {
                if (Utility.Instance.CheckIfClicked(_speechButton))
                {
                    _micInput.StartMicrophone();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (Utility.Instance.CheckIfClicked(_speechButton))
                { 
                    _micInput.StopMicrophone(Name + _currentClipIndex);
                    _clips.Insert(_currentClipIndex, _micInput.GetLastRecording());
                    _currentClipIndex = (_currentClipIndex + 1) % _maxNumberOfStoredClips;
                }
            }

            //BUG: Only for testing, its firing on every cube even if they didn't record anything, just testing 
            if (Input.GetKey("p") && !_clipPlayer.isPlaying && _clips.Count > 0)
            {
                _clipPlayer.clip = _clips[Random.Range(0, _clips.Count)];
                _clipPlayer.Play();
            }
        }

        public void OnDrawGizmos()
        {
            /*
            Gizmos.color = new Color(1, 0, 0, 0.5F);
            Gizmos.DrawCube(cubeObject.transform.position + Vector3.up * 10, new Vector3(1, 1, 1));
            */
        }
    }
}