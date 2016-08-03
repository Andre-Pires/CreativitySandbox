using UnityEngine;

namespace Assets.Scripts.Scripts.CameraControl
{
    public class ThirdPersonCamera : MonoBehaviour
    {

        private const float MIN_Y_ANGLE = 10.0f;
        private const float MAX_Y_ANGLE = 70.0f;

        public Transform LookAtTarget;
        private UnityEngine.Camera _camera;

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
            LookAtTarget = GameObject.FindGameObjectWithTag("Scenario").transform;
        }

        private void Update()
        {
            //negate X in order to move in same direction as mouse
            if (Input.GetButton("Fire2"))
            {
                currentX -= Input.GetAxis("Mouse X");
                currentY += Input.GetAxis("Mouse Y");
                currentY = Mathf.Clamp(currentY, MIN_Y_ANGLE, MAX_Y_ANGLE);

                currentDistance += Input.GetAxis("Mouse ScrollWheel")*zoomSensitivity;
            }

            if (Input.GetButtonUp("Fire1"))
            {
                //LookAtTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(_ray, out _hit, 100))
                {
                    Debug.Log("trocou para " + _hit.transform.name);
                    LookAtTarget = _hit.transform;
                }
            }
        }

        private void LateUpdate()
        {
            Vector3 dir = new Vector3(0,0,-currentDistance);
            Quaternion rotation = Quaternion.Euler(currentY,currentX,0);
            _camera.transform.position = LookAtTarget.position + rotation*dir;
            _camera.transform.LookAt(LookAtTarget.position);
        }
    }
}