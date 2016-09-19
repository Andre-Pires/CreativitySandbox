using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent
{
    public class Body : MonoBehaviour
    {
        private Transform _body;
        private const float InitialPlacementRadius = 26.0f;

        //blinking and color
        private Color _color;
        public Color Color
        {
            get { return _color; }
            set
            {
                _body.GetComponent<Renderer>().material.color = _color;
                _color = value;
            }
        }

        private Color _blinkColor;
        public Color BlinkColor
        {
            get { return _blinkColor; }
            set
            {
                _body.GetComponent<Renderer>().material.color = Color;
                _blinkColor = value;
            }
        }

        private Configuration.Size _size;
        public Configuration.Size Size
        {
            get { return _size; }
            set
            {
                _size = value;
                //using size's enum index to select correct multiplier
                _body.localScale = Vector3.one * Configuration.Instance.SizeValues[value];
                _body.localPosition = new Vector3(_body.position.x, _body.GetComponent<Renderer>().bounds.extents.y, _body.position.z);
            }
        }

        private Configuration.BlinkingSpeed _blinkSpeed;
        public Configuration.BlinkingSpeed BlinkSpeed
        {
            get { return _blinkSpeed; }
            set
            {
                if (value == Configuration.BlinkingSpeed.Stopped)
                {
                    _body.GetComponent<Renderer>().material.color = Color;
                }
                _blinkSpeed = value;
            }
        }

        //dragging fields
        public bool Dragging;
        private Transform _objectToDrag;
        private List<Collider> _collidersToIgnore;
        private Vector3 _distance;

        //rotation
        private float _currentRotation;

        public void InitializeParameters(Configuration.Size size, Transform body, Configuration.Personality personality)
        {
            _body = body;

            if (personality == Configuration.Personality.CustomPersonality)
            {
                int blinkSpeedsCount = Configuration.Instance.PersonalityBlinkingSpeeds.Count-1;
                int colorsCount = Configuration.Instance.AvailableColors.Count-1;

                BlinkSpeed = Configuration.Instance.AvailableBlinkSpeeds[Random.Range(0, blinkSpeedsCount)];
                Color = Configuration.Instance.AvailableColors[Random.Range(0, colorsCount)];
                BlinkColor = Configuration.Instance.AvailableColors[Random.Range(0, colorsCount)];
            }
            else
            {
                //NOTE: for now no association between the personalities and blink colors was made
                BlinkColor = Color.white;
                BlinkSpeed = Configuration.Instance.PersonalityBlinkingSpeeds[personality];
                Color = Configuration.Instance.PersonalityColors[personality];
            }

            _body.GetComponent<Renderer>().material.color = Color;

            //using size's enum index to select correct multiplier
            _body.localScale = Vector3.one*Configuration.Instance.SizeValues[size];
            _body.localPosition = new Vector3(_body.position.x, _body.GetComponent<Renderer>().bounds.extents.y, _body.position.z);
            Size = size;

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

            HandleDragging();
        }

        public void SetupBehavior(Color pieceColor, Configuration.BlinkingSpeed speed, Color blinkColor)
        {
            Color = pieceColor;
            _body.GetComponent<Renderer>().material.color = Color;
            BlinkSpeed = speed;
            BlinkColor = blinkColor;
        }

        private void Blink()
        {
            if (BlinkSpeed != Configuration.BlinkingSpeed.Stopped)
            {
                var colorToUse = BlinkColor;
                var duration = Configuration.Instance.BlinkingSpeedsValues[BlinkSpeed];
                var lerp = Mathf.PingPong(Time.time, duration)/duration;
                _body.GetComponent<Renderer>().material.color = Color.Lerp(Color, colorToUse, lerp);
            }
        }

        private void HandleRotationInput()
        {
            float rotationSpeed; //This will determine rotation speed
            float lerpSpeed; //This will determine lerp speed

            #if UNITY_ANDROID
            if (Input.touchCount == 2)
            {
                var layer = 8;
                var layerMask = 1 << layer;

                Touch touchSlider;
                var touch1 = Input.GetTouch(0);
                var touch2 = Input.GetTouch(1);

                if (touch1.phase == TouchPhase.Stationary &&
                    Utility.Instance.CheckIfClicked(_body.transform, layerMask, touch1.position))
                {
                    touchSlider = touch2;
                }
                else if (touch2.phase == TouchPhase.Stationary &&
                         Utility.Instance.CheckIfClicked(_body.transform, layerMask, touch2.position))
                {
                    touchSlider = touch1;
                }
                else
                {
                    return;
                }


                rotationSpeed = 2.0f;
                lerpSpeed = 10.0f;

                _currentRotation += touchSlider.deltaPosition.y*rotationSpeed;
                _body.rotation = Quaternion.Slerp(_body.transform.rotation, Quaternion.Euler(0, _currentRotation, 0),
                    lerpSpeed*Time.deltaTime);
            }
            #endif

            #if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                if (Utility.Instance.CheckIfClicked(_body.transform))
                {
                    lerpSpeed = 100.0f;
                    rotationSpeed = 50.0f;
                    _currentRotation += Input.GetAxis("Mouse ScrollWheel")*rotationSpeed;
                    _body.rotation = Quaternion.Slerp(_body.transform.rotation, Quaternion.Euler(0, _currentRotation, 0),
                        lerpSpeed*Time.deltaTime);
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
                if (Utility.Instance.CheckIfClicked(_objectToDrag))
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
                    var distanceToScreen = Camera.main.WorldToScreenPoint(transform.position);
                    var posMove =
                        Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                            distanceToScreen.z));

                    var futurePos = new Vector3(posMove.x - _distance.x, transform.position.y, posMove.z - _distance.z);

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
            var hitColliders = Physics.OverlapSphere(position, _objectToDrag.localScale.x/2);
            var numberOfCollidersHit = hitColliders.Length;

            foreach (var collider in _collidersToIgnore)
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
            return false;
        }

        public void OnDrawGizmos()
        {
            /*Gizmos.color = new Color(1, 0, 0, 0.5F);
            Gizmos.DrawSphere(Body.position,Body.localScale.x/2);*/
        }
    }
}