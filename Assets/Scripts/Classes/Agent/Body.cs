﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent
{
    public class Body : MonoBehaviour
    {
        private Configuration.Size _size;
        private Transform _body;
        private const float InitialPlacementRadius = 26.0f;

        //blinking and color
        private readonly Color _negativeColor = Color.black;
        private readonly Color _positiveColor = Color.white;
        private Color _pieceColor = Color.white;
        private Configuration.BlinkingSpeed _currentBlinkSpeed = Configuration.BlinkingSpeed.VerySlow;
        private Configuration.BlinkingStatus _currentBlinkStatus = Configuration.BlinkingStatus.Stopped;

        //dragging fields
        public bool Dragging;
        private Transform _objectToDrag;
        private Vector3 _distance;

        private List<Collider> _collidersToIgnore;

        //double click - change color
        private bool _firstClick;
        private float _initialTime;
        private float _interval = 0.6f;

        //rotation
        private float _currentRotation;


        public void Init(Configuration.Size size, Transform body)
        {
            _size = size;
            _body = body;
       
            //using size's enum index to select correct multiplier
            _body.localScale = Vector3.one* Configuration.Instance.AvailableSizes[size];

            //place cube in a vacant position in the set
            Utility.PlaceNewGameObject(_body, Vector3.zero, InitialPlacementRadius);

            //initializing dragging variables
            _objectToDrag = body;
            //create a list with the colliders of the children and object
            _collidersToIgnore = new List<Collider>();
            _collidersToIgnore.Add(body.gameObject.GetComponent<Collider>());
            _collidersToIgnore.Add(Utility.GetChild(body.gameObject, "Button").GetComponent<Collider>());
        }

        public void Update()
        {
            
            Blink();


           HandleRotationInput();

            if (Input.touchCount < 3 || UnityEngine.Application.platform != RuntimePlatform.Android)
            {
                HandleDragging();
            }
        }

        public void LateUpdate()
        {
            CheckBlinkChange();
        }

        public void SetupBehavior(Color pieceColor, Configuration.BlinkingSpeed speed)
        {
            _pieceColor = pieceColor;
            _body.GetComponent<Renderer>().material.color = _pieceColor;
            _currentBlinkSpeed = speed;
        }

        private void Blink()
        {
            if (_currentBlinkStatus != Configuration.BlinkingStatus.Stopped)
            {
                Color colorToUse = _currentBlinkStatus == Configuration.BlinkingStatus.Positive ? _positiveColor : _negativeColor;
                float duration = Configuration.Instance.AvailableBlinkingSpeeds[_currentBlinkSpeed];
                float lerp = Mathf.PingPong(Time.time, duration) / duration;
                _body.GetComponent<Renderer>().material.color = Color.Lerp(_pieceColor, colorToUse, lerp);
            }
        }

        private void ToggleBlinking()
        {
            int enumLength = Enum.GetNames(typeof(Configuration.BlinkingStatus)).Length;
            int newStatus = ((int)_currentBlinkStatus + 1) %enumLength;
            _currentBlinkStatus = (Configuration.BlinkingStatus) newStatus;

            //always reset color to eliminate inconsistencies
            _body.GetComponent<Renderer>().material.color = _pieceColor;
        }

        private void CheckBlinkChange()
        {
            if (_firstClick)
            {
                if (Time.time - _initialTime > _interval)
                {
                    _firstClick = false;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if(Utility.Instance.CheckIfClicked(_body))
                    { 
                        _firstClick = false;
                        ToggleBlinking();
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (Utility.Instance.CheckIfClicked(_body))
                {
                    _firstClick = true;
                    _initialTime = Time.time;
                }
            }
        }

        private void HandleRotationInput()
        {
            float rotationSpeed;  //This will determine rotation speed
            float lerpSpeed;     //This will determine lerp speed

            #if UNITY_ANDROID
            if (Input.touchCount == 2)
            {
                int layer = 8;
                int layerMask = 1 << layer;

                Touch touchSlider;
                var touch1 = Input.GetTouch(0);
                var touch2 = Input.GetTouch(1);
              
                if (touch1.phase == TouchPhase.Stationary && Utility.Instance.CheckIfClicked(_body.transform, layerMask, touch1.position))
                {
                    touchSlider = touch2;
                }
                else if (touch2.phase == TouchPhase.Stationary && Utility.Instance.CheckIfClicked(_body.transform, layerMask, touch2.position))
                {
                    touchSlider = touch1;
                }
                else
                {
                    return;
                }

               
                rotationSpeed = 2.0f;
                lerpSpeed = 10.0f;
        
                _currentRotation += touchSlider.deltaPosition.y * rotationSpeed;
                _body.rotation = Quaternion.Slerp(_body.transform.rotation, Quaternion.Euler(0, _currentRotation, 0), lerpSpeed * Time.deltaTime);
            }
            #endif

            #if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                if (Utility.Instance.CheckIfClicked(_body.transform))
                {
                    lerpSpeed = 100.0f; 
                    rotationSpeed = 50.0f;
                    _currentRotation += Input.GetAxis("Mouse ScrollWheel") * rotationSpeed;
                    _body.rotation = Quaternion.Slerp(_body.transform.rotation, Quaternion.Euler(0, _currentRotation, 0), lerpSpeed * Time.deltaTime);
                }
            }
            #endif
        }

        private void HandleDragging()
        {
            //to avoid moving the pieces by accident
            if (UnityEngine.Application.platform == RuntimePlatform.Android && Input.touchCount > 1)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                if(Utility.Instance.CheckIfClicked(_objectToDrag))
                {
                    Dragging = true;
                    _distance = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                        Camera.main.WorldToScreenPoint(transform.position).z)) - transform.position;
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (Dragging)
                {
                    Vector3 distanceToScreen = Camera.main.WorldToScreenPoint(transform.position);
                    Vector3 posMove = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToScreen.z));

                    Vector3 futurePos = new Vector3(posMove.x - _distance.x, transform.position.y, posMove.z - _distance.z);

                    if (!IsColliding(futurePos))
                        _objectToDrag.position = futurePos;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Dragging = false;
            }
        }

        private bool IsColliding(Vector3 position)
        {
            Collider[] hitColliders = Physics.OverlapSphere(position, _objectToDrag.localScale.x / 2);
            int numberOfCollidersHit = hitColliders.Length;

            foreach (Collider collider in _collidersToIgnore)
            {
                if (hitColliders.Contains(collider))
                {
                    numberOfCollidersHit--;
                }
            }

            //always ignore the floor where the piece stands on by putting 1
            if (numberOfCollidersHit > 0)
            {
                Debug.Log("collided with something");
                return true;
            }
            else
            {
                return false;
            }

        }

        public void OnDrawGizmos()
        {
            /*Gizmos.color = new Color(1, 0, 0, 0.5F);
            Gizmos.DrawSphere(_body.position,_body.localScale.x/2);*/
        }
    }
}