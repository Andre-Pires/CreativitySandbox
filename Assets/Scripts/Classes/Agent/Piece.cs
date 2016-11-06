using System;
using System.Collections.Generic;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Classes.Agent
{
    public class Piece
    {
        public string Name { get; set; }
        private GameObject _root;
        private GameObject _cubeObject;
        public readonly Body Body;
        public readonly Mind Mind;
        private SoundRecorder _soundRecorder;
        public Configuration.Personality Personality { get; private set; }

        //Piece cloner
        public Piece(string name, Piece piece, Dictionary<string, Piece> allPieces)
        {
            Personality = piece.Personality;
            Name = name;
            PieceComponentInitializer();

            List<Piece> otherPieces = new List<Piece>();
            foreach (KeyValuePair<string, Piece> keyValuePair in allPieces)
            {
                if (keyValuePair.Key == Name)
                {
                    continue;
                }

                otherPieces.Add(keyValuePair.Value);
            }

            _cubeObject.AddComponent<Body>();
            Body = _cubeObject.GetComponent<Body>();
            Body.InitializeParameters(piece.Body);

            _cubeObject.AddComponent<Mind>();
            Mind = _cubeObject.GetComponent<Mind>();
            Mind.InitializeParameters(piece.Body, Personality, otherPieces);

            Debug.Log("New agent part added: part " + name + ". " + piece.Body.Size + " size and " + Personality + " personality");
        }

        public Piece(string name, Configuration.Personality personality, Configuration.Size size, Dictionary<string, Piece> allPieces)
        {
            Personality = personality;
            Name = name;
            PieceComponentInitializer();

            List<Piece> otherPieces = new List<Piece>();
            foreach (KeyValuePair<string, Piece> keyValuePair in allPieces)
            {
                if (keyValuePair.Key == Name)
                {
                    continue;
                }

                otherPieces.Add(keyValuePair.Value);
            }

            _cubeObject.AddComponent<Body>();
            Body = _cubeObject.GetComponent<Body>();
            Body.InitializeParameters(size, personality);

            _cubeObject.AddComponent<Mind>();
            Mind = _cubeObject.GetComponent<Mind>();
            Mind.InitializeParameters(Body, Personality, otherPieces);

            Debug.Log("New agent part added: part " + name + ". " + size + " size and " + personality + " personality");
        }

        public void RemoveStoredAgentPiece(Piece piece)
        {
            Mind.RemoveStoreAgentPiece(piece);
        }

        public void StoreAgentPiece(Piece piece)
        {
            Mind.StoreAgentPiece(piece);
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