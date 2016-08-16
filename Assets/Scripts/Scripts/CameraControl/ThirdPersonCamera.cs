using System.Diagnostics;
using Assets.Scripts.Classes.Agent;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Scripts.CameraControl
{
    public class ThirdPersonCamera : MonoBehaviour
    {

        private const float MIN_Y_ANGLE = 10.0f;
        private const float MAX_Y_ANGLE = 70.0f;
        private const float MIN_ZOOM = 5.0f;
        private const float MAX_ZOOM = 75.0f;

        private Camera _camera;
        public Transform BirdViewLookAt;
        public Transform CloseUpLookAt;
        private Transform _lookAtInUse;

        private bool _birdEyeViewActive = true;
        private const float BIRD_EYE_VIEW_DIST = 50.0f;

        private float currentDistance = 50.0f;
        private float zoomSensitivity = 50.0f;
        private float currentX = 0.0f;
        private float currentY = 40.0f;
        private float sensitivityX = 4.0f;
        private float sensitivityY = 1.0f;

        private RaycastHit _hit;
        private Ray _ray;

        //touch specific fields
        private Touch _touch;
        private float touchDist = 0;
        private float _touchSensitivityX = 60.0f;
        private float _touchSensitivityY = 50.0f;
        private float _touchZoomSensitivity = 35.0f;

        //the two fingers must be close to each other to activate orbiting the camera
        private float _touchOrbitDist = 400.0f;

        private void Start()
        {
            _camera = Camera.main;

            //start looking at the center of the set
            BirdViewLookAt = GameObject.FindGameObjectWithTag("Scenario").transform;
            _lookAtInUse = BirdViewLookAt;

            //register listener from camera mode switch
            UI.ChangeCameraMode.Instance.OnSelect += ChangeCameraMode;
        }

        private void Update()
        {
            if (BirdViewLookAt == null)
            {
                BirdViewLookAt = GameObject.FindGameObjectWithTag("Scenario").transform;
            }

            #if UNITY_ANDROID
                HandleTouchInput();
            #endif
            
            #if (UNITY_STANDALONE || UNITY_EDITOR)
                HandleMouseInput();
            #endif

            if (!_birdEyeViewActive)
            {
                CheckLookAtChange();
            }
        }

        [Conditional("UNITY_ANDROID")]
        private void HandleTouchInput()
        {
            //rotating the camera
            if (Input.touchCount > 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                var touch1 = Input.GetTouch(0);

                var touch2 = Input.GetTouch(1);

                float dist = Vector2.Distance(touch1.position, touch2.position);


                if (dist < _touchOrbitDist)
                {
                    _touch = touch1;

                    currentX += _touch.deltaPosition.x * _touchSensitivityX * 0.02f;
                    currentY -= _touch.deltaPosition.y * _touchSensitivityY * 0.02f;
                    currentY = Mathf.Clamp(currentY, MIN_Y_ANGLE, MAX_Y_ANGLE);
                }
            }

            //only when allowing changing lookat and zooming
            if (!_birdEyeViewActive)
            {
                //zoom camera
                if (Input.touchCount > 1 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved))
                {
                    var touch1 = Input.GetTouch(0);

                    var touch2 = Input.GetTouch(1);

                    float dist = Vector2.Distance(touch1.position, touch2.position);

                    if (dist > touchDist)
                    {
                        currentDistance -= Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition) * zoomSensitivity / 10;
                    }
                    else
                    {
                        currentDistance += Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition) * zoomSensitivity / 10;
                    }

                    currentDistance = Mathf.Clamp(currentDistance, MIN_ZOOM, MAX_ZOOM);
                    touchDist = dist;

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
            if (!_birdEyeViewActive)
            {
                currentDistance += Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
                currentDistance = Mathf.Clamp(currentDistance, MIN_ZOOM, MAX_ZOOM);
            }
        }

        private void CheckLookAtChange()
        {
            if (Input.GetButtonUp("Fire1"))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int layer = 8;
                int layerMask = 1 << layer;

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

            
            if (_birdEyeViewActive)
            {
                _lookAtInUse = BirdViewLookAt;
            }
            else
            {
                // ReSharper disable once ConvertConditionalTernaryToNullCoalescing
                _lookAtInUse = CloseUpLookAt == null ? BirdViewLookAt : CloseUpLookAt;
            }

            if (_lookAtInUse == null)
            {
                Debug.Log("Look at is null");
                return;
            }

            //if the lookat isn't static check if it's moving and don't move if it is
            if (!_birdEyeViewActive && _lookAtInUse.tag == "Cube")
            {
                if (_lookAtInUse.gameObject.GetComponent<Body>().Dragging)
                {
                    return;
                }
            }

            
            float translationSpeed = 3.0f;  //This will determine translation speed
            float rotationSpeed = 50.0f;  //This will determine rotation speed
            float lookAtSpeed = 6.0f;  //This will determine lookAt speed

            Vector3 dir = new Vector3(0, 0, -currentDistance);
            Quaternion rotation = Quaternion.Slerp(_camera.transform.rotation, Quaternion.Euler(currentY, currentX, 0), rotationSpeed * Time.deltaTime);
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, _lookAtInUse.position + rotation * dir, translationSpeed * Time.deltaTime);

            Vector3 direction = _lookAtInUse.position - _camera.transform.position;
            _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), lookAtSpeed * Time.deltaTime);
        }

        public void ChangeCameraMode()
        {
            _birdEyeViewActive = !_birdEyeViewActive;

            if (_birdEyeViewActive)
            {
                currentDistance = BIRD_EYE_VIEW_DIST;
            }
        }

    }
}