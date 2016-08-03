using System;
using UnityEngine;

namespace Assets.Scripts.Scripts.Elenesski_Generic_Move_Camera {

    public class GenericMoveCamera : MonoBehaviour {

        private Movement _forward;
        private Movement _panX;
        private Movement _rotateX;
        private Movement _panY;
        private Movement _rotateY;
        private float _resolution = 1f;

        [Header("Operational")]
        public bool Operational = true;

        [Header("Input Method")]
        public GenericMoveCameraInputs GetInputs = null;

        [Header("Camera")]
        public bool LevelCamera = true;
        public bool ForwardMovementLockEnabled = true;

        [Header("Movement Speed")]
        public float MovementSpeedMagnification = 1f;
        public float WheelMouseMagnification = 5f;
        public float ShiftKeyMagnification = 2f;
        public float ControlKeyMagnification = 0.25f;
        public float RotationMagnification = 1f;

        [Header("Pan Speed Modifications")]
        public float PanLeftRightSensitivity = 1f;
        public float PanUpDownSensitivity = 1f;

        [Header("Mouse Rotation Sensitivity")]
        public float MouseRotationSensitivity = 0.5f;

        [Header("Dampening")]
        public float ForwardDampenRate = 0.99f;
        public float PanningDampenRate = 0.95f;
        public float RotateDampenRate = 0.99f;

        private Vector3 _hiddenTarget = Vector3.zero;

        //TODO might be unecessary
        public Vector3 LookAtTarget
        {
            get
            {
                return _hiddenTarget;
            }

            set
            {
                _hiddenTarget = value;

                if (value != null)
                {
                    GetInputs.IsLockedToTarget = true;
                }
                else
                {
                    GetInputs.IsLockedToTarget = false;
                }
            }
        }

        private RaycastHit _hit;
        private Ray _ray;


        [Header("Look At")]
        public float MinimumZoom = 20f;
        public float MaximumZoom = 80f;

        [Header("Movement Limits - X")]
        public bool LockX = false;
        public bool UseXRange;
        public float XRangeMin;
        public float XRangeMax;

        [Header("Movement Limits - Y")]
        public bool LockY = false;
        public bool UseYRange;
        public float YRangeMin;
        public float YRangeMax;

        [Header("Movement Limits - Z")]
        public bool LockZ = false;
        public bool UseZRange;
        public float ZRangeMin;
        public float ZRangeMax;

        // Rotation when in Awake(), to prevent weird rotations later
        private Vector3 _defaultRotation;

        private class Movement {
            private readonly Action<float> _action;
            private readonly Func<float> _dampenRate;
            private float _velocity;
            private float _dampen;

            public Movement(Action<float> aAction, Func<float> aDampenRate) {
                _action = aAction;
                _dampenRate = aDampenRate;
                _velocity = 0f;
                _dampen = 0;
            }

            public void ChangeVelocity(float aAmount) {
                _velocity += aAmount;
                _dampen = _dampenRate();
            }

            public void SetVelocity(float aAmount) {
                _velocity = aAmount;
                _dampen = _dampenRate();
            }

            public void Update(bool aDampen = true) {
                if (_dampen > 0)
                    if (_velocity >= -0.001f && _velocity <= 0.001f) {
                        _dampen = 0;
                        _velocity = 0;
                    } else {
                        if (aDampen)
                            _velocity *= _dampen;

                        _action(_velocity);
                    }
            }
        }

        public void SetResolution(float aResolution) {
            _resolution = aResolution;
        }

        public void Awake() {

            if ( GetInputs == null )
                GetInputs = gameObject.AddComponent<GenericMoveCameraInputs>();

            _defaultRotation = gameObject.transform.localRotation.eulerAngles;

            GetInputs.Initialize();
        }

        public void Start()
        {

            LookAtTarget = GameObject.FindGameObjectWithTag("Scenario").transform.position;

            if (LookAtTarget == null) {
                _forward = new Movement(aAmount => gameObject.transform.Translate(Vector3.forward*aAmount), () => ForwardDampenRate);
            } else {
                _forward = new Movement(aAmount => gameObject.GetComponent<UnityEngine.Camera>().fieldOfView += aAmount, () => ForwardDampenRate);
            }

            _panX = new Movement(aAmount => gameObject.transform.Translate(Vector3.left*aAmount), () => PanningDampenRate);
            _panY = new Movement(aAmount => gameObject.transform.Translate(Vector3.up*aAmount), () => PanningDampenRate);

            _rotateX = new Movement(aAmount => gameObject.transform.Rotate(Vector3.up*aAmount), () => RotateDampenRate);
            _rotateY = new Movement(aAmount => gameObject.transform.Rotate(Vector3.left*aAmount), () => RotateDampenRate);

        }

        public void Update() {

            if (!Operational)
                return;

            //TODO I don't know if this is the best way to this - rethink it
            if (Input.GetButtonUp("Fire1"))
            {

                LookAtTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Debug.Log(" devia trocar ");
                /*
                                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                                if (Physics.Raycast(_ray, out _hit, 100))
                                {
                                    Debug.Log("trocou para " + _hit.transform.name);
                                    LookAtTarget = _hit.transform.gameObject;
                                }*/
            }

            GetInputs.QueryInputSystem();

            Vector3 startPosition = gameObject.transform.position;

            if (GetInputs.ResetMovement) {
                ResetMovement();
            } else {

                float mag = (GetInputs.IsSlowModifier ? ControlKeyMagnification : 1f)*(GetInputs.IsFastModifier ? ShiftKeyMagnification : 1f);

                if (GetInputs.IsPanLeft) {
                    _panX.ChangeVelocity(0.01f*mag*_resolution*PanLeftRightSensitivity);
                } else if (GetInputs.IsPanRight) {
                    _panX.ChangeVelocity(-0.01f*mag*_resolution*PanLeftRightSensitivity);
                }

                if ( _panX != null )
                    _panX.Update();

                if (GetInputs.IsMoveForward ) {
                    _forward.ChangeVelocity(0.010f*mag*_resolution*MovementSpeedMagnification);
                } else if (GetInputs.IsMoveBackward ) {
                    _forward.ChangeVelocity(-0.010f*mag*_resolution*MovementSpeedMagnification);
                }

                if (GetInputs.IsMoveForwardAlt) {
                    _forward.ChangeVelocity(0.005f*mag*_resolution*MovementSpeedMagnification*WheelMouseMagnification);
                } else if (GetInputs.IsMoveBackwardAlt) {
                    _forward.ChangeVelocity(-0.005f*mag*_resolution*MovementSpeedMagnification*WheelMouseMagnification);
                }

                if (GetInputs.IsPanUp) {
                    _panY.ChangeVelocity(0.005f*mag*_resolution*PanUpDownSensitivity);
                } else if (GetInputs.IsPanDown) {
                    _panY.ChangeVelocity(-0.005f*mag*_resolution*PanUpDownSensitivity);
                }

                bool forwardLock = GetInputs.IsLockForwardMovement && ForwardMovementLockEnabled;
                _forward.Update(!forwardLock);

                _panY.Update();

                // Pan
                if (GetInputs.IsRotateAction) {

                    float x = (Input.mousePosition.x - GetInputs.RotateActionStart.x)/Screen.width*MouseRotationSensitivity;
                    float y = (Input.mousePosition.y - GetInputs.RotateActionStart.y)/Screen.height*MouseRotationSensitivity;

                    _rotateX.SetVelocity(x*mag*RotationMagnification*_resolution);
                    _rotateY.SetVelocity(y*mag*RotationMagnification*_resolution);

                }

                _rotateX.Update();
                _rotateY.Update();
            }


            // Lock at object
            if (LookAtTarget != null ) {
                transform.LookAt(LookAtTarget);
                if (gameObject.GetComponent<UnityEngine.Camera>().fieldOfView < MinimumZoom) {
                    ResetMovement();
                    gameObject.GetComponent<UnityEngine.Camera>().fieldOfView = MinimumZoom;
                } else if (gameObject.GetComponent<UnityEngine.Camera>().fieldOfView > MaximumZoom) {
                    ResetMovement();
                    gameObject.GetComponent<UnityEngine.Camera>().fieldOfView = MaximumZoom;
                }
            }

            // Set ranges
            Vector3 endPosition = transform.position;

            if (LockX)
                endPosition.x = startPosition.x;
            if (LockY)
                endPosition.y = startPosition.y;
            if (LockZ)
                endPosition.z = startPosition.z;

            if (UseXRange && gameObject.transform.position.x < XRangeMin) endPosition.x = XRangeMin;
            if (UseXRange && gameObject.transform.position.x > XRangeMax) endPosition.x = XRangeMax;

            if (UseYRange && gameObject.transform.position.y < YRangeMin) endPosition.y = YRangeMin;
            if (UseYRange && gameObject.transform.position.y > YRangeMax) endPosition.y = YRangeMax;

            if (UseZRange && gameObject.transform.position.z < ZRangeMin) endPosition.z = ZRangeMin;
            if (UseZRange && gameObject.transform.position.z > ZRangeMax) endPosition.z = ZRangeMax;

            transform.position = endPosition;

            // Level Camera
            if (LevelCamera)
                LevelTheCamera();

        }

        public void ResetMovement() {
            _panX.SetVelocity(0);
            _panY.SetVelocity(0);
            _forward.SetVelocity(0);
            _rotateX.SetVelocity(0);
            _rotateY.SetVelocity(0);

            _panX.Update();
            _panY.Update();
            _forward.Update();
            _rotateX.Update();
            _rotateY.Update();
        }

        public void OnCollisionEnter(Collision collision) {
            ResetMovement();
        }

        public void PanY( float aMagnitude ) {
            _panY.ChangeVelocity(0.005f*aMagnitude*_resolution*PanUpDownSensitivity);
        }

        public void PanX(float aMagnitude) {
            _panX.ChangeVelocity(-0.01f*aMagnitude*_resolution*PanLeftRightSensitivity);
        }

        public void ForwardBack( float aMagnitude ) {
            _forward.ChangeVelocity(-0.005f*aMagnitude*_resolution*MovementSpeedMagnification);
        }

        public void LevelTheCamera() {
            transform.rotation = Quaternion.LookRotation(transform.forward.normalized, Vector3.up);
        }

    }

}