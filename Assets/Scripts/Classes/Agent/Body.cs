using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Classes.Agent.ComposedBehaviors;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.IO;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent
{
    public class Body : MonoBehaviour
    {
        public bool BodyHalted;
        public Transform Transform;
        public Transform Mesh;
        private Configuration.ApplicationMode _pieceMode;
        private bool _alreadyInitialized;
        private const float ScenarioPlacementRadius = 26.0f;
        private const float FloatComparisonEpsilon = 0.1f;

        public delegate void OnPropertyChange();
        public event OnPropertyChange NotifyUI;

        public delegate void OnBehaviorActivation();
        public event OnBehaviorActivation NotifyAgentMind;

        //blinking and color
        private Color _color;
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                Mesh.GetComponent<Renderer>().material.color = _color;

                if (NotifyUI != null) NotifyUI();
            }
        }

        private Color _blinkColor;
        public Color BlinkColor
        {
            get { return _blinkColor; }
            set
            {
                Mesh.GetComponent<Renderer>().material.color = Color;
                _blinkColor = value;

                if (NotifyUI != null) NotifyUI();
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
                Transform.localScale = Vector3.one * Configuration.Instance.SizeValues[value];
                Transform.localPosition = new Vector3(Transform.localPosition.x, 0 /*Mesh.GetComponent<Renderer>().bounds.extents.y*/, Transform.localPosition.z);

                if (NotifyUI != null) NotifyUI();
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
                    Mesh.GetComponent<Renderer>().material.color = Color;
                }
                _blinkSpeed = value;

                if (NotifyUI != null) NotifyUI();
            }
        }

        public Dictionary<Configuration.ComposedBehaviors, ComposedBehavior> AgentBehaviors;

        //dragging fields
        public bool DraggingStatus;
        private Transform _objectToDrag;
        private List<Collider> _collidersToIgnore;
        private Vector3 _distance;
        private Vector3 _dragStartPosition;

        //rotation
        public float CurrentRotation;

        //autonomous Behavior
        public bool DisplayingBehavior;


        public void InitializeParameters(Configuration.Size size, Configuration.Personality personality, Configuration.ApplicationMode pieceMode)
        {
            Transform = transform;
            Mesh = Utility.GetChild(gameObject, "AgentMesh").transform;
            _pieceMode = pieceMode;
            CurrentRotation = Transform.localEulerAngles.y;

            if (personality == Configuration.Personality.CustomPersonality)
            {
                int colorsCount = Configuration.Instance.AvailableColors.Count-1;
                Color = Configuration.Instance.AvailableColors[Random.Range(0, colorsCount)];

                if (Configuration.Instance.BlinkingBehaviorActive)
                {
                    int blinkSpeedsCount = Configuration.Instance.PersonalityBlinkingSpeeds.Count-1;
                    BlinkSpeed = Configuration.Instance.AvailableBlinkSpeeds[Random.Range(0, blinkSpeedsCount)];
                    BlinkColor = Configuration.Instance.AvailableColors[Random.Range(0, colorsCount)];
                }
            }
            else
            {
                Color = Configuration.Instance.PersonalityColors[personality];

                //NOTE: for now no association between the personalities and blink colors was made
                if (Configuration.Instance.BlinkingBehaviorActive)
                {
                    BlinkColor = Color.white;
                    BlinkSpeed = Configuration.Instance.PersonalityBlinkingSpeeds[personality];
                }
            }

            Mesh.GetComponent<Renderer>().material.color = Color;

            //using size's enum index to select correct multiplier
            Size = size;

            //place cube in a vacant position in the set
            Utility.PlaceNewGameObject(Transform, Vector3.zero, ScenarioPlacementRadius, Mesh.GetComponent<Renderer>());

            SetupDrag();

            _alreadyInitialized = true;
        }

        //Body cloner
        public void InitializeParameters(Body body, Configuration.ApplicationMode pieceMode)
        {
            Transform = transform;
            Mesh = Utility.GetChild(gameObject, "AgentMesh").transform;
            CurrentRotation = Transform.localEulerAngles.y;

            _pieceMode = pieceMode;
            Color = body.Color;

            if (Configuration.Instance.BlinkingBehaviorActive)
            {
                BlinkSpeed = body.BlinkSpeed;
                BlinkColor = body.BlinkColor;
            }
           
            //using size's enum index to select correct multiplier
            Size = body.Size;

            //place cube in a vacant position in the set
            Utility.PlaceNewGameObject(Mesh, Vector3.zero, ScenarioPlacementRadius, Mesh.GetComponent<Renderer>());

            SetupDrag();
        }

        private void SetupDrag()
        {
            //initializing dragging variables
            _objectToDrag = Transform;
            //create a list with the colliders of the children and object
            _collidersToIgnore = new List<Collider>();
            _collidersToIgnore.Add(Mesh.GetComponent<Collider>());

            if (Configuration.Instance.SoundRecordingActive)
            {
                _collidersToIgnore.Add(Utility.GetChild(Transform.gameObject, "Button").GetComponent<Collider>());
            }
        }

        void OnEnable()
        {
            if (_alreadyInitialized)
            {
                Collider[] hitColliders = Physics.OverlapSphere(Transform.localPosition,
                   Mesh.GetComponent<Renderer>().bounds.extents.magnitude);
                int collidersHit = hitColliders.Length;

                foreach (var collider in hitColliders)
                {
                    if (_collidersToIgnore.Contains(collider))
                    {
                        collidersHit--;
                    }
                }

                if (collidersHit > 0)
                {
                    //Debug.Log("clear");
                    //place cube in a vacant position in the set
                    Utility.PlaceNewGameObject(Transform, Vector3.zero, ScenarioPlacementRadius, Mesh.GetComponent<Renderer>());
                }
            }
        }

        void OnDisable()
        {
            //only has to reset animator if it an autonomous agent piece
            if (_pieceMode == Configuration.ApplicationMode.AutonomousAgent)
            {
                Mesh.GetComponent<Animator>().enabled = false;

                Mesh.transform.localPosition = new Vector3(Mesh.transform.localPosition.x, 0, Mesh.transform.localPosition.z);
                Mesh.rotation = new Quaternion(0,0,0,0);

                for (int i = 0; i < Mesh.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; i++)
                {
                    Mesh.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(i, 0);
                }

                Mesh.GetComponent<Animator>().enabled = true;
            }
        }

        public void Update()
        {
            if (BodyHalted)
            {
                return;
            }

            bool executingBehavior = false;

            if (_pieceMode == Configuration.ApplicationMode.AutonomousAgent && AgentBehaviors != null)
            {
                foreach (ComposedBehavior behavior in AgentBehaviors.Values)
                {
                    if (!behavior.IsOver)
                    {
                        behavior.ApplyBehavior(this);
                        executingBehavior = true;
                    }
                }
            }

            if (!executingBehavior)
            {
                HandleBlinking();
                HandleRotation();
            }
            HandleDragging();
        }


        private void HandleBlinking()
        {
            if (Configuration.Instance.BlinkingBehaviorActive && BlinkSpeed != Configuration.BlinkingSpeed.Stopped)
            {
                var colorToUse = BlinkColor;
                var duration = Configuration.Instance.BlinkingSpeedsValues[BlinkSpeed];
                var lerp = Mathf.PingPong(Time.time, duration)/duration;
                Mesh.GetComponent<Renderer>().material.color = Color.Lerp(Color, colorToUse, lerp);
            }
        }

        private void HandleRotation()
        {
            float rotationSpeed; //This will determine rotation speed
            float lerpSpeed; //This will determine lerp speed

            #if UNITY_ANDROID
            if (Input.touchCount == 2)
            {
                float oldRotation = CurrentRotation;
                var layer = 8;
                var layerMask = 1 << layer;

                Touch touchSlider;
                var touch1 = Input.GetTouch(0);
                var touch2 = Input.GetTouch(1);

                if (touch1.phase == TouchPhase.Stationary &&
                    Utility.Instance.CheckIfClicked(Mesh, layerMask, touch1.position))
                {
                    touchSlider = touch2;
                }
                else if (touch2.phase == TouchPhase.Stationary &&
                         Utility.Instance.CheckIfClicked(Mesh, layerMask, touch2.position))
                {
                    touchSlider = touch1;
                }
                else
                {
                    return;
                }

                rotationSpeed = 2.0f;
                lerpSpeed = 10.0f;

                CurrentRotation += touchSlider.deltaPosition.y*rotationSpeed;
                Transform.rotation = Quaternion.Slerp(Transform.rotation, Quaternion.Euler(0, CurrentRotation, 0),
                    lerpSpeed*Time.deltaTime);

                float minimumRotation = 2.0f;
                if (Mathf.Abs(oldRotation - CurrentRotation) > FloatComparisonEpsilon + minimumRotation)
                    SessionLogger.Instance.WriteToLogFile("Agent " + Transform.name + " was rotated.");
            }
            #endif

            #if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                if (Utility.Instance.CheckIfClicked(Mesh))
                {
                    float oldRotation = CurrentRotation;
                    lerpSpeed = 100.0f;
                    rotationSpeed = 50.0f;
                    CurrentRotation += Input.GetAxis("Mouse ScrollWheel")*rotationSpeed;
                    Transform.rotation = Quaternion.Slerp(Transform.rotation, Quaternion.Euler(0, CurrentRotation, 0),
                        lerpSpeed*Time.deltaTime);

                    float minimumRotation = 2.0f;
                    if (Mathf.Abs(oldRotation - CurrentRotation) > FloatComparisonEpsilon + minimumRotation)
                        SessionLogger.Instance.WriteToLogFile("Agent " + Transform.name + " was rotated.");
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
                if (Utility.Instance.CheckIfClicked(Mesh))
                {
                    DraggingStatus = true;
                    _dragStartPosition = transform.position;
                    _distance = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                        Camera.main.WorldToScreenPoint(_dragStartPosition).z)) - _dragStartPosition;

                    SessionLogger.Instance.WriteToLogFile("Agent " + Transform.name + " was dragged.");
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (DraggingStatus)
                {
                    var distanceToScreen = Camera.main.WorldToScreenPoint(transform.position);
                    var posMove =
                        Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                            distanceToScreen.z));


                    var futurePos = new Vector3(posMove.x - _distance.x, transform.position.y, posMove.z - _distance.z);

                    if (!IsColliding(futurePos))
                    {
                        // to avoid drags out of the scenario
                        if ((futurePos.x < ScenarioPlacementRadius && futurePos.x > -ScenarioPlacementRadius) &&
                            (futurePos.z < ScenarioPlacementRadius && futurePos.z > -ScenarioPlacementRadius))
                        {
                            _objectToDrag.position = futurePos;
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && DraggingStatus)
            {
                DraggingStatus = false;

                Vector3 distanceCovered = _dragStartPosition -transform.position;

                //ignoring clicking the piece
                if (distanceCovered.magnitude > 0.5f)
                {
                    if (NotifyAgentMind != null) NotifyAgentMind();
                }

            }
        }

        public bool IsColliding(Vector3 position)
        {
            return IsColliding(position, transform.localRotation);
        }

        public bool IsColliding(Vector3 position, Quaternion rotation)
        {
            var hitColliders = Physics.OverlapBox(position, Transform.localScale / 2, rotation);
            var numberOfCollidersHit = hitColliders.Length;

            foreach (var collider in _collidersToIgnore)
            {
                if (hitColliders.Contains(collider))
                {
                    numberOfCollidersHit--;
                }
            }

            if (numberOfCollidersHit > 0)
            {
                //Debug.Log("collided with something");
                return true;
            }
            return false;
        }

        public bool IsColliding(out Vector3 hitNormal)
        {
            var hitColliders = Physics.OverlapBox(transform.position, _objectToDrag.localScale / 2, transform.localRotation).ToList();
            var numberOfCollidersHit = hitColliders.Count;

            foreach (var collider in _collidersToIgnore)
            {
                if (hitColliders.Contains(collider))
                {
                    numberOfCollidersHit--;
                    hitColliders.Remove(collider);
                }
            }

            if (numberOfCollidersHit > 0)
            {
                Vector3 hitPosition = Vector3.zero;
                foreach (Collider hitCollider in hitColliders)
                {
                    hitPosition += hitCollider.ClosestPointOnBounds(transform.position);
                }
                //Debug.Log("collided with something");
                hitNormal = transform.position - (hitPosition/hitColliders.Count);
                return true;
            }

            hitNormal = Vector3.zero;
            return false;
        }

        public void OnDrawGizmos()
        {
            /*Gizmos.color = new Color(1, 0, 0, 0.5F);
            Gizmos.DrawSphere(transform.position,transform.GetComponent<Renderer>().bounds.extents.magnitude);*/

        }
    }
}