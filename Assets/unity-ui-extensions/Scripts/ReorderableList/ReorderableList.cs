﻿/// Credit Ziboo
/// Sourced from - http://forum.unity3d.com/threads/free-reorderable-list.364600/

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.ReorderableList
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    [AddComponentMenu("UI/Extensions/Re-orderable list")]
    public class ReorderableList : MonoBehaviour
    {
        private RectTransform _content;
        private ReorderableListContent _listContent;

        [Tooltip("Should the draggable components be removed or cloned?")] public bool CloneDraggedObject = false;

        [Tooltip("Child container with re-orderable items in a layout group")] public LayoutGroup ContentLayout;

        [Tooltip("Parent area to draw the dragged element on top of containers. Defaults to the root Canvas")] public
            RectTransform DraggableArea;

        [Tooltip("Can items be dragged from the container?")] public bool IsDraggable = true;

        [Tooltip("Can new draggable items be dropped in to the container?")] public bool IsDropable = true;


        [Header("UI Re-orderable Events")] public ReorderableListHandler OnElementDropped = new ReorderableListHandler();

        public ReorderableListHandler OnElementGrabbed = new ReorderableListHandler();
        public ReorderableListHandler OnElementRemoved = new ReorderableListHandler();

        public RectTransform Content
        {
            get
            {
                if (_content == null)
                {
                    _content = ContentLayout.GetComponent<RectTransform>();
                }
                return _content;
            }
        }

        private Canvas GetCanvas()
        {
            var t = transform;
            Canvas canvas = null;


            var lvlLimit = 100;
            var lvl = 0;

            while (canvas == null && lvl < lvlLimit)
            {
                canvas = t.gameObject.GetComponent<Canvas>();
                if (canvas == null)
                {
                    t = t.parent;
                }

                lvl++;
            }
            return canvas;
        }

        private void Awake()
        {
            if (ContentLayout == null)
            {
                Debug.LogError("You need to have a child LayoutGroup content set for the list: " + name, gameObject);
                return;
            }
            if (DraggableArea == null)
            {
                DraggableArea = transform.root.GetComponentInChildren<Canvas>().GetComponent<RectTransform>();
            }
            if (IsDropable && !GetComponent<Graphic>())
            {
                Debug.LogError(
                    "You need to have a Graphic control (such as an Image) for the list [" + name + "] to be droppable",
                    gameObject);
                return;
            }
            if (GetCanvas().renderMode != RenderMode.ScreenSpaceOverlay)
            {
                Debug.LogError("The ReOrderable List is only supported on a Screenspace-Overlay Canvas at the moment");
            }

            _listContent = ContentLayout.gameObject.AddComponent<ReorderableListContent>();
            _listContent.Init(this);
        }

        #region Nested type: ReorderableListEventStruct

        [Serializable]
        public struct ReorderableListEventStruct
        {
            public GameObject DroppedObject;
            public int FromIndex;
            public ReorderableList FromList;
            public bool IsAClone;
            public GameObject SourceObject;
            public int ToIndex;
            public ReorderableList ToList;
        }

        #endregion

        #region Nested type: ReorderableListHandler

        [Serializable]
        public class ReorderableListHandler : UnityEvent<ReorderableListEventStruct>
        {
        }

        public void TestReOrderableListTarget(ReorderableListEventStruct item)
        {
            Debug.Log("Event Received");
            Debug.Log("Hello World, is my item a clone? [" + item.IsAClone + "]");
        }

        #endregion
    }
}