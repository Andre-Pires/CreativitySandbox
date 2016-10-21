using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.IO;
using Assets.Scripts.Classes.UI;
using Assets.Scripts.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Classes.Agent
{
    public class Piece
    {
        public string Name { get; set; }
        private GameObject _root;
        private GameObject _cubeObject;
        public readonly Body Body;
        private SoundRecorder _soundRecorder;
        public Configuration.Personality Personality { get; private set; }

        //Piece cloner
        public Piece(string name, Piece piece)
        {
            Personality = piece.Personality;
            Name = name;
            PieceComponentInitializer();

            _cubeObject.AddComponent<Body>();
            Body = _cubeObject.GetComponent<Body>();
            Body.InitializeParameters(_cubeObject.transform, piece.Body);

            Debug.Log("New agent part added: part " + name + ". " + piece.Body.Size + " size and " + Personality + " personality");
        }

        public Piece(string name, Configuration.Personality personality, Configuration.Size size)
        {
            Personality = personality;
            Name = name;
            PieceComponentInitializer();

            _cubeObject.AddComponent<Body>();
            Body = _cubeObject.GetComponent<Body>();
            Body.InitializeParameters(size, _cubeObject.transform, personality);

            Debug.Log("New agent part added: part " + name + ". " + size + " size and " + personality + " personality");
        }

        private void PieceComponentInitializer()
        {
            _root = GameObject.Find("Scene");
            _cubeObject = Object.Instantiate(Resources.Load("Prefabs/Agent/Cube")) as GameObject;

            if (_cubeObject != null)
            {
                _cubeObject.name = Name;
                _cubeObject.tag = "Cube";
                _cubeObject.transform.SetParent(_root.transform, false);
            }
            else
            {
                throw new NullReferenceException("The cube's prefab wasn't properly loaded");
            }

            if (Configuration.Instance.SoundRecordingActive)
            {
                _soundRecorder = new SoundRecorder(Name, _cubeObject, _root);
                Utility.GetChild(_cubeObject, "3DButton").SetActive(true);
            }
        }

        public void Update()
        {
            Body.Update();

            if (Configuration.Instance.SoundRecordingActive)
            {
                _soundRecorder.Update();
            }
        }

        public void ToggleVisibility()
        {
            //check if piece is recording and stop recording before hiding it
            if (Configuration.Instance.SoundRecordingActive && _soundRecorder.IsRecording)
            {
                _soundRecorder.StopRecording();
            }

            _cubeObject.SetActive(_cubeObject.activeSelf != true);
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
        }
    }
}