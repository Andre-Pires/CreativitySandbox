using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Scripts
{
    public class DragCube : MonoBehaviour
    {
        private float _distanceToDragPlane;
        private bool _dragging;
        private Plane _dragPlane;
        private Vector3 _dragPlaneNormal = Vector3.up;
        private RaycastHit _hit;
        private readonly Transform _objectToDrag;
        private Ray _ray;
        private List<Collider> _collidersToIgnore;

        public DragCube()
        {
            _objectToDrag = gameObject.transform;
            //create a list with the colliders of the children and object
            _collidersToIgnore = new List<Collider>();
            _collidersToIgnore.Add(gameObject.GetComponent<Collider>());
            _collidersToIgnore.Add(GetChild(gameObject, "Button").GetComponent<Collider>());
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(_ray, out _hit, 100) && _hit.transform == _objectToDrag)
                {
                    _dragging = true;
                    _dragPlane = new Plane(_dragPlaneNormal, _objectToDrag.position);
                }
            }

            if (Input.GetMouseButton(0))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (_dragging)
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
                _dragging = false;
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
            Collider[] hitColliders = Physics.OverlapSphere(position, _objectToDrag.localScale.x/2);
            int numberOfCollidersHit = hitColliders.Length;

            foreach (Collider collider in _collidersToIgnore)
            {
                if (hitColliders.Contains(collider))
                {
                    numberOfCollidersHit--;
                }
            }

            //always ignore the floor where the piece stands on by putting 1
            if(numberOfCollidersHit > 0)
            { 
                Debug.Log("collided with something");
                return true;
            }
            else
            {
                return false;
            }


        }

        private GameObject GetChild(GameObject parent, string name)
        {
            Component[] transforms = parent.GetComponentsInChildren(typeof(Transform), true);

            foreach (Transform transform in transforms)
            {
                if (transform.gameObject.name == name)
                {
                    return transform.gameObject;
                }
            }

            return null;
        }
    }
}