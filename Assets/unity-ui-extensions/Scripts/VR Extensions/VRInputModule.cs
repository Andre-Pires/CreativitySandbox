﻿/// Credit Ralph Barbagallo (www.flarb.com /www.ralphbarbagallo.com / @flarb)
/// Sourced from - http://forum.unity3d.com/threads/vr-cursor-possible-unity-4-6-gui-bug-or-is-it-me
/// Fix supplied by - http://forum.unity3d.com/threads/vr-cursor-possible-unity-4-6-gui-bug-or-is-it-me.296934/

using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.VR_Extensions
{
    [AddComponentMenu("Event/VR Input Module")]
    public class VRInputModule : BaseInputModule
    {
        public static GameObject targetObject;

        private static VRInputModule _singleton;

        private static bool mouseClicked;
        public static Vector3 cursorPosition;

        private int counter;

        protected override void Awake()
        {
            _singleton = this;
        }

        public override void Process()
        {
            if (targetObject == null)
            {
                mouseClicked = false;
            }
        }

        public static void PointerSubmit(GameObject obj)
        {
            targetObject = obj;
            mouseClicked = true;
            if (mouseClicked)
            {
                //BaseEventData data = GetBaseEventData(); //Original from Process().  Can't be called here so is replaced by the next line:
                var data = new BaseEventData(_singleton.eventSystem);
                data.selectedObject = targetObject;
                ExecuteEvents.Execute(targetObject, data, ExecuteEvents.submitHandler);
                print("clicked " + targetObject.name);
                mouseClicked = false;
            }
        }

        public static void PointerExit(GameObject obj)
        {
            print("PointerExit " + obj.name);
            var pEvent = new PointerEventData(_singleton.eventSystem);
            ExecuteEvents.Execute(obj, pEvent, ExecuteEvents.pointerExitHandler);
            ExecuteEvents.Execute(obj, pEvent, ExecuteEvents.deselectHandler); //This fixes the problem
        }

        public static void PointerEnter(GameObject obj)
        {
            print("PointerEnter " + obj.name);
            var pEvent = new PointerEventData(_singleton.eventSystem);
            pEvent.pointerEnter = obj;
            var rcr = new RaycastResult {worldPosition = cursorPosition};
            pEvent.pointerCurrentRaycast = rcr;
            ExecuteEvents.Execute(obj, pEvent, ExecuteEvents.pointerEnterHandler);
        }
    }
}