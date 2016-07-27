using System;
using System.Collections.Generic;
using Assets.Scripts.Scripts;
using Assets.Scripts.Scripts.UI;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Classes
{
    public class Piece
    {
        public string Name { get; set; }
        private GameObject _cubeObject;
        private GameObject _root;
        

        //TODO: should be integrated into a body and mind structure
        // it should allow to apply abstract behaviors to the cube
        //private Mind personality;
        private Configuration.Personality _personality;
        private Body _body;

        private MicrophoneInput _micInput;
        private AudioSource _clipPlayer;
        private List<AudioClip> _clips;
        private int _maxNumberOfStoredClips = 3;
        private int _currentClipIndex = 0;

        // Check for mouse input for speech recording
        private RaycastHit _hit;
        private Transform _speechButton;
        private Ray _ray;

        public Piece(string name, Configuration.Personality personality, Configuration.Size size)
        {
            Debug.Log("New agent part added: part " + name + ". " + size + " size and " + personality + " personality");
            //TODO criar um body que inicializa tamanho, cor, etc
            //TODO criar uma mente em pôr a personalidade por enquanto

            _personality = personality;
            Name = name;

            _root = GameObject.Find("Scene");
            _cubeObject = Object.Instantiate(Resources.Load("Prefabs/Cube")) as GameObject;
            SetupPiecePrefab(_cubeObject);

            _body = new Body(size, Color.cyan, _cubeObject.transform);
        }

        private void SetupPiecePrefab(GameObject cubePrefab)
        {
            
            if (cubePrefab != null)
            {
                cubePrefab.name = Name;
                cubePrefab.tag = "Cube";
                cubePrefab.transform.parent = _root.transform;
                cubePrefab.gameObject.AddComponent<DragCube>();
                _speechButton = GetChild(cubePrefab, "Button").transform;

                

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
            //TODO should be removed eventually
            Color colorStart = Color.red;
            Color colorEnd = Color.green;
            float duration = 1.0f;
            float lerp = Mathf.PingPong(Time.time, duration) / duration;
            _cubeObject.GetComponent<Renderer>().material.color = Color.Lerp(colorStart, colorEnd, lerp);

            if (!Microphone.IsRecording(_micInput.SelectedDevice) && Input.GetMouseButtonDown(0))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(_ray, out _hit, 100) && _hit.transform == _speechButton)
                {
                    _micInput.StartMicrophone();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(_ray, out _hit, 100) && _hit.transform == _speechButton)
                {
                    _micInput.StopMicrophone(Name + _currentClipIndex);
                    _clips.Insert(_currentClipIndex, _micInput.GetLastRecording());
                    _currentClipIndex = (_currentClipIndex + 1) % _maxNumberOfStoredClips;
                }
            }

            //BUG: Only for testing, its firing on every cube even if they didn't record anything, just testing rotation before porting to touchscreen
            if (Input.GetKey("p") && !_clipPlayer.isPlaying && _clips.Count > 0)
            {
                _clipPlayer.clip = _clips[Random.Range(0, _clips.Count)];
                _clipPlayer.Play();
            }



            //BUG: will rotate every cube in the scene, just testing rotation before porting to touchscreen
            if (Input.GetKey("q"))
            {
                float rotationSpeed = 100;  //This will determine max rotation speed, you can adjust in the inspector
                //Get mouse position
                Vector3 mousePos = Input.mousePosition;

                //Adjust mouse z position
                mousePos.z = Camera.main.transform.position.y - _cubeObject.transform.position.y;

                //Get a world position for the mouse
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

                //Get the angle to rotate and rotate
                float angle = -Mathf.Atan2(_cubeObject.transform.position.z - mouseWorldPos.z, _cubeObject.transform.position.x - mouseWorldPos.x) * Mathf.Rad2Deg;
                _cubeObject.transform.rotation = Quaternion.Slerp(_cubeObject.transform.rotation, Quaternion.Euler(0, angle, 0), rotationSpeed * Time.deltaTime);
            }
        }

        

        private GameObject GetChild(GameObject parent, string name)
        {
            Component[] transforms = parent.GetComponentsInChildren(typeof(Transform), true);

            foreach (Transform transform in transforms)
            {
                if (transform.gameObject.name == name)
                {
                    return transform.gameObject;
                }
            }

            return null;
        }

        public void OnDrawGizmos()
        {
            /*
            Gizmos.color = new Color(1, 0, 0, 0.5F);
            Gizmos.DrawCube(cubeObject.transform.position + Vector3.up * 10, new Vector3(1, 1, 1));
            */
            _body.OnDrawGizmos();

        }

    }
}