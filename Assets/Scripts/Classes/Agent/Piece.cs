using System;
using System.Collections.Generic;
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
        private readonly GameObject _root;
        private readonly GameObject _cubeObject;
        public readonly Body Body;
        private SoundRecorder _soundRecorder;
        public Configuration.Personality Personality { get; private set; }

        //Piece cloner
        public Piece(string name, Piece piece)
        {
            Personality = piece.Personality;
            Name = name;

            Debug.Log("New agent part added: part " + name + ". " + piece.Body.Size + " size and " + Personality + " personality");

            _root = GameObject.Find("Scene");
            _cubeObject = Object.Instantiate(Resources.Load("Prefabs/Agent/Cube")) as GameObject;
            SetupPiecePrefab(_cubeObject);

            _soundRecorder = new SoundRecorder(Name, _cubeObject, _root);

            _cubeObject.AddComponent<Body>();
            Body = _cubeObject.GetComponent<Body>();
            Body.InitializeParameters(_cubeObject.transform, piece.Body);
        }

        public Piece(string name, Configuration.Personality personality, Configuration.Size size)
        {
            Debug.Log("New agent part added: part " + name + ". " + size + " size and " + personality + " personality");

            Name = name;

            _root = GameObject.Find("Scene");
            _cubeObject = Object.Instantiate(Resources.Load("Prefabs/Agent/Cube")) as GameObject;
            SetupPiecePrefab(_cubeObject);

            _soundRecorder = new SoundRecorder(Name, _cubeObject, _root);

            _cubeObject.AddComponent<Body>();
            Body = _cubeObject.GetComponent<Body>();
            Body.InitializeParameters(size, _cubeObject.transform, personality);
        }


        private void SetupPiecePrefab(GameObject cubePrefab)
        {
            if (cubePrefab != null)
            {
                cubePrefab.name = Name;
                cubePrefab.tag = "Cube";
                cubePrefab.transform.parent = _root.transform;
                
            }
            else
            {
                throw new NullReferenceException("The cube's prefab wasn't properly loaded");
            }
        }


        public void Update()
        {
            Body.Update();
            _soundRecorder.Update();
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