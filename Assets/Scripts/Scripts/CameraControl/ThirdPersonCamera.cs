using System;
using System.Diagnostics;
using Assets.Scripts.Classes.Agent;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.UI;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Scripts.CameraControl
{
    public enum ActiveCameraMode { BirdEye, FollowCharacter, ColorPicker}

    public class ThirdPersonCamera : MonoBehaviour
    {
        private const float MIN_Y_ANGLE = 10.0f;
        private const float MAX_Y_ANGLE = 70.0f;
        private const float MIN_ZOOM = 5.0f;
        private const float MAX_ZOOM = 75.0f;
        private const float BIRD_EYE_VIEW_DIST = 50.0f;

        private ActiveCameraMode _currentCameraMode = ActiveCameraMode.BirdEye;
        private ActiveCameraMode _lastCameraMode;
        private Transform _lastCameraTransformation;

        private Camera _camera;

        private RaycastHit _hit;
        private Transform _lookAtInUse;
        private Ray _ray;

        //touch specific fields
        private Touch _touch;
        private float _touchDist;

        //the two fingers must be close to each other to activate orbiting the camera
        private readonly float _touchOrbitDist = 400.0f;
        private readonly float _touchSensitivityX = 60.0f;
        private readonly float _touchSensitivityY = 50.0f;
        private readonly float _touchZoomSensitivity = 25.0f;
        public Transform BirdViewLookAt;
        public Transform CloseUpLookAt;
        public Transform ColorLookAt;

        private float currentDistance = 50.0f;
        private float currentX;
        private float currentY = 40.0f;
        private float sensitivityX = 4.0f;
        private float sensitivityY = 1.0f;
        private readonly float zoomSensitivity = 50.0f;
        
        private void Start()
        {
            _camera = Camera.main;

            
            //start looking at the center of the set
            BirdViewLookAt = GameObject.FindGameObjectWithTag("Scenario").transform;
            _lookAtInUse = BirdViewLookAt;

            if (Configuration.Instance.CameraMovementActive)
            {
                //register listener from camera mode switch
                UI.ChangeCameraMode.Instance.OnSelect += ToggleCameraMode;
            }

            GameObject.Find("OpenScenarioColors").GetComponent<Button>().onClick.AddListener(ToggleColorPickerCameraMode);
            AppUIManager.Instance.ColorMenuCloseButton.GetComponent<Button>().onClick.AddListener(ToggleColorPickerCameraMode);
            ColorLookAt = GameObject.Find("ColorLookAt").GetComponent<Transform>();
        }

        private void Update()
        {
            if (!Configuration.Instance.CameraMovementActive)
            {
                return;
            }

            if (BirdViewLookAt == null)
            {
                BirdViewLookAt = GameObject.FindGameObjectWithTag("Scenario").transform;
            }

            if (_currentCameraMode != ActiveCameraMode.ColorPicker)
            {
            #if UNITY_ANDROID
            HandleTouchInput();
            #endif

            #if (UNITY_STANDALONE || UNITY_EDITOR)
            HandleMouseInput();
            #endif
            }

            if (_currentCameraMode == ActiveCameraMode.FollowCharacter)
            {
                CheckLookAtChange();
            }
        }

        [Conditional("UNITY_ANDROID")]
        private void HandleTouchInput()
        {
            //rotating the camera
            if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved &&
                Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                var touch1 = Input.GetTouch(0);

                var touch2 = Input.GetTouch(1);

                var dist = Vector2.Distance(touch1.position, touch2.position);


                if (dist < _touchOrbitDist)
                {
                    _touch = touch1;

                    currentX += _touch.deltaPosition.x*_touchSensitivityX*0.02f;
                    currentY -= _touch.deltaPosition.y*_touchSensitivityY*0.02f;
                    currentY = Mathf.Clamp(currentY, MIN_Y_ANGLE, MAX_Y_ANGLE);
                }
            }

            //only when allowing changing lookat and zooming
            if (_currentCameraMode == ActiveCameraMode.FollowCharacter)
            {
                //zoom camera
                if (Input.touchCount == 2 &&
                    (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved))
                {
                    var touch1 = Input.GetTouch(0);

                    var touch2 = Input.GetTouch(1);

                    var dist = Vector2.Distance(touch1.position, touch2.position);

                    if (dist > _touchDist)
                    {
                        currentDistance -= Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition)*
                                           _touchZoomSensitivity/10;
                    }
                    else
                    {
                        currentDistance += Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition)*
                                           _touchZoomSensitivity/10;
                    }

                    currentDistance = Mathf.Clamp(currentDistance, MIN_ZOOM, MAX_ZOOM);
                    _touchDist = dist;
                }
            }
        }

        [Conditional("UNITY_STANDALONE"), Conditional("UNITY_EDITOR")]
        private void HandleMouseInput()
        {
            //negate X in order to move in same direction as mouse
            if (Input.GetButton("Fire2"))
            {
                currentX -= Input.GetAxis("Mouse X");
                currentY += Input.GetAxis("Mouse Y");
                currentY = Mathf.Clamp(currentY, MIN_Y_ANGLE, MAX_Y_ANGLE);
            }

            //only when allowing changing lookat and zooming
            if (_currentCameraMode == ActiveCameraMode.FollowCharacter)
            {
                currentDistance += Input.GetAxis("Mouse ScrollWheel")*zoomSensitivity;
                currentDistance = Mathf.Clamp(currentDistance, MIN_ZOOM, MAX_ZOOM);
            }
        }

        private void CheckLookAtChange()
        {
            if (Input.GetButtonUp("Fire1"))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var layer = 8;
                var layerMask = 1 << layer;

                //if we selected a cube change the look at (layer only considers cubes)
                if (Physics.Raycast(_ray, out _hit, 100, layerMask))
                {
                    Debug.Log("New look at: " + _hit.transform.name);
                    CloseUpLookAt = _hit.transform;
                }
            }
        }

        private void LateUpdate()
        {
            switch (_currentCameraMode)
            {
                case ActiveCameraMode.ColorPicker:
                    _lookAtInUse = ColorLookAt;
                    break;
                case ActiveCameraMode.BirdEye:
                    _lookAtInUse = BirdViewLookAt;
                    break;
                default:
                    _lookAtInUse = CloseUpLookAt == null ? BirdViewLookAt : CloseUpLookAt;
                    break;
            }

            if (_lookAtInUse == null)
            {
                //Debug.Log("Look at is null");
                return;
            }

            //if the lookat isn't static check if it's moving and don't move if it is
            if (_currentCameraMode == ActiveCameraMode.FollowCharacter && _lookAtInUse.tag == "Cube")
            {
                if (_lookAtInUse.gameObject.GetComponent<Body>().DraggingStatus)
                {
                    return;
                }
            }


            var translationSpeed = 3.0f; //This will determine translation speed
            var rotationSpeed = 50.0f; //This will determine rotation speed
            var lookAtSpeed = 6.0f; //This will determine lookAt speed

            var dir = new Vector3(0, 0, -currentDistance);
            var rotation = Quaternion.identity;

            if (_currentCameraMode == ActiveCameraMode.ColorPicker)
            {
                rotation = Quaternion.Slerp(_camera.transform.rotation, Quaternion.identity, 
                rotationSpeed * Time.deltaTime);
            }
            else
            {
                rotation = Quaternion.Slerp(_camera.transform.rotation, Quaternion.Euler(currentY, currentX, 0),
                rotationSpeed*Time.deltaTime);
            }

            _camera.transform.position = Vector3.Lerp(_camera.transform.position, _lookAtInUse.position + rotation*dir,
                translationSpeed*Time.deltaTime);

            var direction = _lookAtInUse.position - _camera.transform.position;
            _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation,
                Quaternion.LookRotation(direction, Vector3.up), lookAtSpeed*Time.deltaTime);
        }

        public void ToggleCameraMode()
        {
            switch (_currentCameraMode)
            {
                case ActiveCameraMode.BirdEye:
                    _currentCameraMode = ActiveCameraMode.FollowCharacter;
                    break;
                case ActiveCameraMode.FollowCharacter:
                    _currentCameraMode = ActiveCameraMode.BirdEye;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_currentCameraMode == ActiveCameraMode.BirdEye)
            {
                currentDistance = BIRD_EYE_VIEW_DIST;
            }
        }

        public void ToggleColorPickerCameraMode()
        {
            switch (_currentCameraMode)
            {
                case ActiveCameraMode.FollowCharacter:
                case ActiveCameraMode.BirdEye:
                    _lastCameraMode = _currentCameraMode;
                    _lastCameraTransformation = Camera.main.transform;
                    _currentCameraMode = ActiveCameraMode.ColorPicker;
                    Debug.Log("Color camera");
                    break;
                case ActiveCameraMode.ColorPicker:
                    _currentCameraMode = _lastCameraMode;
                    Camera.main.transform.position = _lastCameraTransformation.position;
                    Camera.main.transform.rotation = _lastCameraTransformation.rotation;
                    Debug.Log("Standard camera");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}