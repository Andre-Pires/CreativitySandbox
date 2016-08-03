using UnityEngine;

namespace Assets.Scripts.Scripts.Elenesski_Generic_Move_Camera {

    public class GenericMoveCameraInputs : MonoBehaviour {

        public bool IsSlowModifier;         // Slows the movement down by a factor
        public bool IsFastModifier;         // Speeds the movement up by a factor
        public bool IsRotateAction;         // Indicates that the camera is rotating.
        public Vector2 RotateActionStart;   // The X,Y position where the right mouse was clicked
        public Vector2 PanActionStart;      // The X,Y position where the right mouse was clicked -- ONLY when locked
        public bool IsLockForwardMovement;  // Turns of forward dampening while on
        public bool ResetMovement;          // Stops all movement
        public bool IsPanLeft;              // Tells the system to pan left
        public bool IsPanRight;             // Tells the system to pan right
        public bool IsPanUp;                // Tells the system to pan up
        public bool IsPanDown;              // Tells the system to pan down
        public bool IsMoveForward;          // Moves the camera forward
        public bool IsMoveBackward;         // Moves the camera backward
        public bool IsMoveForwardAlt;       // Moves the camera forward (alternate)
        public bool IsMoveBackwardAlt;      // Moves the camera backward (alternate)

        private bool _hiddenLockedToTarget;
        public bool IsLockedToTarget
        {
            get { return _hiddenLockedToTarget; }
            set
            {
                _hiddenLockedToTarget = value;
            }
        }

        
        public virtual void Initialize() {
            RotateActionStart = new Vector2();
            PanActionStart = new Vector2();
        }

        public virtual void QueryInputSystem() {

            IsSlowModifier = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
            IsFastModifier = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
            IsRotateAction = Input.GetButton("Fire2");

            // Get mouse starting point when the button was clicked.
            if ( Input.GetButtonDown("Fire2") && !IsLockedToTarget ) {
                RotateActionStart.x = Input.mousePosition.x;
                RotateActionStart.y = Input.mousePosition.y;
            }
            IsLockForwardMovement = Input.GetButton("Fire3");
            ResetMovement = Input.GetKey(KeyCode.Space);

            if (!IsLockedToTarget)
            {
                IsPanLeft = Input.GetKey(KeyCode.A);
                IsPanRight = Input.GetKey(KeyCode.D);
                IsPanUp = Input.GetKey(KeyCode.Q);
                IsPanDown = Input.GetKey(KeyCode.Z);

                IsMoveForward = Input.GetKey(KeyCode.W);
                IsMoveBackward = Input.GetKey(KeyCode.S);

                IsMoveForwardAlt = Input.GetAxis("Mouse ScrollWheel") > 0;
                IsMoveBackwardAlt = Input.GetAxis("Mouse ScrollWheel") < 0;

            }
            else if (IsLockedToTarget)
            {
                if (Input.GetButton("Fire2"))
                {
                    //move left and right
                    if (PanActionStart.x < Input.mousePosition.x)
                    {
                        IsPanRight = true;
                        IsPanLeft = false;
                    }
                    else if (PanActionStart.x > Input.mousePosition.x)
                    {
                        IsPanLeft = true;
                        IsPanRight = false;
                    }

                    // move up and down
                    if (PanActionStart.y < Input.mousePosition.y)
                    {
                        IsPanUp = true;
                        IsPanDown = false;
                    }
                    else if (PanActionStart.y > Input.mousePosition.y)
                    {
                        IsPanDown = true;
                        IsPanUp = false;
                    }
                    PanActionStart.x = Input.mousePosition.x;
                    PanActionStart.y = Input.mousePosition.y;
                    IsFastModifier = true;
                }
                else
                {
                    IsPanLeft = false;
                    IsPanRight = false;
                    IsPanUp = false;
                    IsPanDown = false;
                }

                IsMoveForward = Input.GetAxis("Mouse ScrollWheel") < 0;
                IsMoveBackward = Input.GetAxis("Mouse ScrollWheel") > 0;
            }



        }

    }
}