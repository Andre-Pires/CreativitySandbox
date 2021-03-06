﻿using System;
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
        public readonly Configuration.ApplicationMode PieceMode;

        public bool IsPieceVisible
        {
            private set
            {
                //no behavior
            }

            get { return _cubeObject.activeSelf; }
        }

        //Piece cloner
        public Piece(string name, Piece piece, List<Piece> autonomousPieces = null)
        {
            Personality = piece.Personality;
            Name = name;
            PieceComponentInitializer();
            PieceMode = piece.PieceMode;

            _cubeObject.AddComponent<Body>();
            Body = _cubeObject.GetComponent<Body>();
            Body.InitializeParameters(piece.Body, PieceMode);

            if (PieceMode == Configuration.ApplicationMode.AutonomousAgent)
            {
                List<Piece> otherPieces = autonomousPieces;

                _cubeObject.AddComponent<Mind>();
                Mind = _cubeObject.GetComponent<Mind>();
                Mind.InitializeParameters(piece.Body, Personality, otherPieces);
            }
            else
            {
                _cubeObject.GetComponentInChildren<Animator>().enabled = false;
            }

            Debug.Log("New agent part added: part " + name + ". " + piece.Body.Size + " size and " + Personality + " personality");
            SessionLogger.Instance.WriteToLogFile("Created  piece's Mind and Body: " + name);
        }

        public Piece(string name, Configuration.Personality personality, Configuration.Size size, Configuration.ApplicationMode pieceMode, List<Piece> autonomousPieces = null)
        {
            Personality = personality;
            Name = name;
            PieceMode = pieceMode;
            PieceComponentInitializer();

            _cubeObject.AddComponent<Body>();
            Body = _cubeObject.GetComponent<Body>();
            Body.InitializeParameters(size, personality, pieceMode);

            if (PieceMode == Configuration.ApplicationMode.AutonomousAgent)
            {
                List<Piece> otherPieces = autonomousPieces;

                _cubeObject.AddComponent<Mind>();
                Mind = _cubeObject.GetComponent<Mind>();
                Mind.InitializeParameters(Body, Personality, otherPieces);
            }
            else
            {
                _cubeObject.GetComponentInChildren<Animator>().enabled = false;
            }

            Debug.Log("New agent part added: part " + name + ". " + size + " size and " + personality + " personality");
            SessionLogger.Instance.WriteToLogFile("Created  piece's Mind and Body: " + name);
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
            _cubeObject = Object.Instantiate(Resources.Load("Prefabs/Agent/Agent")) as GameObject;

            if (_cubeObject != null)
            {
                _cubeObject.name = Name;
                _cubeObject.tag = "Cube";
                _cubeObject.transform.SetParent(_root.transform, false);
                Utility.GetChild(_cubeObject.gameObject, "AgentMesh").tag = "Cube";
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

            //halt agent modules when piece is invisible
            if (PieceMode == Configuration.ApplicationMode.AutonomousAgent)
            {
                Mind.MindHalted = !Mind.MindHalted;
            }
            Body.BodyHalted = !Body.BodyHalted;
        }

        public void DestroyPiece()
        {
            Object.Destroy(_cubeObject);
        }
    }
}