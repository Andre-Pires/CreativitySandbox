﻿/// Credit Ralph Barbagallo (www.flarb.com /www.ralphbarbagallo.com / @flarb)
/// Sourced from - http://forum.unity3d.com/threads/vr-cursor-possible-unity-4-6-gui-bug-or-is-it-me

using UnityEngine;

namespace Assets.Scripts.VR_Extensions
{
    [AddComponentMenu("UI/Extensions/VR Cursor")]
    public class VRCursor : MonoBehaviour
    {
        private Collider currentCollider;
        public float xSens;
        public float ySens;

        // Update is called once per frame
        private void Update()
        {
            Vector3 thisPosition;

            thisPosition.x = Input.mousePosition.x*xSens;
            thisPosition.y = Input.mousePosition.y*ySens - 1;
            thisPosition.z = transform.position.z;

            transform.position = thisPosition;

            VRInputModule.cursorPosition = transform.position;

            if (Input.GetMouseButtonDown(0) && currentCollider)
            {
                VRInputModule.PointerSubmit(currentCollider.gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //print("OnTriggerEnter other " + other.gameObject);
            VRInputModule.PointerEnter(other.gameObject);
            currentCollider = other;
        }

        private void OnTriggerExit(Collider other)
        {
            //print("OnTriggerExit other " + other.gameObject);
            VRInputModule.PointerExit(other.gameObject);
            currentCollider = null;
        }
    }
}