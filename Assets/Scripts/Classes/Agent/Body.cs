using System;
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
        private Color _negativeColor = Color.black;
        private Color _positiveColor = Color.white;
        private Color _pieceColor = Color.white;
        private Configuration.BlinkingSpeed _currentBlinkSpeed = Configuration.BlinkingSpeed.VerySlow;
        private Configuration.BlinkingStatus _currentBlinkStatus = Configuration.BlinkingStatus.Stopped;

        //dragging fields
        private float _distanceToDragPlane;
        public bool Dragging;
        private Plane _dragPlane;
        private Vector3 _dragPlaneNormal = Vector3.up;
        private Transform _objectToDrag;
        private Ray _ray;


        private List<Collider> _collidersToIgnore;

        //double click - change color
        private bool _singleClick;
        private float _initialTime;
        private float _interval = 1.0f;

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
            HandleDragging();
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
            //layer that contains only the cube itself
            int layer = 8;
            int layerMask = 1 << layer;

            if (_singleClick)
            {
                if (Time.time - _initialTime > _interval)
                {
                    _singleClick = false;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    if(Utility.Instance.CheckIfClicked(_body, layerMask));
                    { 
                        _singleClick = false;
                        ToggleBlinking();
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (Utility.Instance.CheckIfClicked(_body, layerMask))
                {
                    _singleClick = true;
                    _initialTime = Time.time;
                }
            }
        }

        private void HandleDragging()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(Utility.Instance.CheckIfClicked(_objectToDrag))
                {
                    Dragging = true;
                    _dragPlane = new Plane(_dragPlaneNormal, _objectToDrag.position);
                }
            }

            if (Input.GetMouseButton(0))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Dragging)
                {
                    if (_dragPlane.Raycast(_ray, out _distanceToDragPlane))
                    {
                        Vector3 futurePos = _ray.GetPoint(_distanceToDragPlane);

                        if (!IsColliding(futurePos))
                            _objectToDrag.position = futurePos;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Dragging = false;
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                _dragPlaneNormal = Vector3.forward;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                _dragPlaneNormal = Vector3.right;
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                _dragPlaneNormal = Vector3.up;
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