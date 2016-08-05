using UnityEngine;

namespace Assets.Scripts.Scripts.CameraControl
{
    public class ThirdPersonCamera : MonoBehaviour
    {

        private const float MIN_Y_ANGLE = 10.0f;
        private const float MAX_Y_ANGLE = 70.0f;
        private const float MIN_ZOOM = 5.0f;
        private const float MAX_ZOOM = 75.0f;

        public Transform BirdViewLookAt;
        public Transform CloseUpLookAt;
        private Camera _camera;
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

        private void Start()
        {
            _camera = Camera.main;

            //start looking at the center of the set
            BirdViewLookAt = GameObject.FindGameObjectWithTag("Scenario").transform;

            //register listener from camera mode switch
            UI.ChangeCameraMode.Instance.OnSelect += ChangeCameraMode;
        }

        private void Update()
        {
            if (BirdViewLookAt == null)
            {
                BirdViewLookAt = GameObject.FindGameObjectWithTag("Scenario").transform;
            }

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
                currentDistance += Input.GetAxis("Mouse ScrollWheel")*zoomSensitivity;
                currentDistance = Mathf.Clamp(currentDistance, MIN_ZOOM, MAX_ZOOM);

                if (Input.GetButtonUp("Fire1"))
                {
                    _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(_ray, out _hit, 100))
                    {
                        Debug.Log("New look at: " + _hit.transform.name);
                        CloseUpLookAt = _hit.transform;
                    }
                }
            }
        }

        private void LateUpdate()
        {
            Transform lookAtInUse;

            if (_birdEyeViewActive)
            {
                lookAtInUse = BirdViewLookAt;
            }
            else
            {
                lookAtInUse = CloseUpLookAt ?? BirdViewLookAt;
            }

            if (lookAtInUse != null)
            {
                Vector3 dir = new Vector3(0, 0, -currentDistance);
                Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
                _camera.transform.position = lookAtInUse.position + rotation * dir;
                _camera.transform.LookAt(lookAtInUse.position);
            }
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