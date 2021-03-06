﻿using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets.Scripts.Utilities
{
    /// <summary>
    ///     Simple toggle -- something that has an 'on' and 'off' states: checkbox, toggle button, radio button, etc.
    /// </summary>
    [AddComponentMenu("UI/Extensions/Extensions Toggle", 31)]
    [RequireComponent(typeof(RectTransform))]
    public class ExtensionsToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
    {
        public enum ToggleTransition
        {
            None,
            Fade
        }

        /// <summary>
        ///     Graphic the toggle should be working with.
        /// </summary>
        public Graphic graphic;

        // group that this toggle can belong to
        [SerializeField] private ExtensionsToggleGroup m_Group;

        // Whether the toggle is on
        [FormerlySerializedAs("m_IsActive")] [Tooltip("Is the toggle currently on or off?")] [SerializeField] private
            bool m_IsOn;

        /// <summary>
        ///     Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        /// </summary>
        public ToggleEvent onValueChanged = new ToggleEvent();

        /// <summary>
        ///     Transition type.
        /// </summary>
        public ToggleTransition toggleTransition = ToggleTransition.Fade;

        protected ExtensionsToggle()
        {
        }

        public ExtensionsToggleGroup group
        {
            get { return m_Group; }
            set
            {
                m_Group = value;
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                {
                    SetToggleGroup(m_Group, true);
                    PlayEffect(true);
                }
            }
        }

        /// <summary>
        ///     Whether the toggle is currently active.
        /// </summary>
        public bool isOn
        {
            get { return m_IsOn; }
            set { Set(value); }
        }

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(m_IsOn);
#endif
        }

        public virtual void LayoutComplete()
        {
        }

        public virtual void GraphicUpdateComplete()
        {
        }

        /// <summary>
        ///     React to clicks.
        /// </summary>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            InternalToggle();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            InternalToggle();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Set(m_IsOn, false);
            PlayEffect(toggleTransition == ToggleTransition.None);

            var prefabType = PrefabUtility.GetPrefabType(this);
            if (prefabType != PrefabType.Prefab && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif // if UNITY_EDITOR

        protected override void OnEnable()
        {
            base.OnEnable();
            SetToggleGroup(m_Group, false);
            PlayEffect(true);
        }

        protected override void OnDisable()
        {
            SetToggleGroup(null, false);
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Check if isOn has been changed by the animation.
            // Unfortunately there is no way to check if we donï¿½t have a graphic.
            if (graphic != null)
            {
                var oldValue = !Mathf.Approximately(graphic.canvasRenderer.GetColor().a, 0);
                if (m_IsOn != oldValue)
                {
                    m_IsOn = oldValue;
                    Set(!oldValue);
                }
            }

            base.OnDidApplyAnimationProperties();
        }

        private void SetToggleGroup(ExtensionsToggleGroup newGroup, bool setMemberValue)
        {
            var oldGroup = m_Group;

            // Sometimes IsActive returns false in OnDisable so don't check for it.
            // Rather remove the toggle too often than too little.
            if (m_Group != null)
                m_Group.UnregisterToggle(this);

            // At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
            // That's why we use the setMemberValue parameter.
            if (setMemberValue)
                m_Group = newGroup;

            // Only register to the new group if this Toggle is active.
            if (m_Group != null && IsActive())
                m_Group.RegisterToggle(this);

            // If we are in a new group, and this toggle is on, notify group.
            // Note: Don't refer to m_Group here as it's not guaranteed to have been set.
            if (newGroup != null && newGroup != oldGroup && isOn && IsActive())
                m_Group.NotifyToggleOn(this);
        }

        private void Set(bool value)
        {
            Set(value, true);
        }

        private void Set(bool value, bool sendCallback)
        {
            if (m_IsOn == value)
                return;

            // if we are in a group and set to true, do group logic
            m_IsOn = value;
            if (m_Group != null && IsActive())
            {
                if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.allowSwitchOff))
                {
                    m_IsOn = true;
                    m_Group.NotifyToggleOn(this);
                }
            }

            // Always send event when toggle is clicked, even if value didn't change
            // due to already active toggle in a toggle group being clicked.
            // Controls like Dropdown rely on this.
            // It's up to the user to ignore a selection being set to the same value it already was, if desired.
            PlayEffect(toggleTransition == ToggleTransition.None);
            if (sendCallback)
                onValueChanged.Invoke(m_IsOn);
        }

        /// <summary>
        ///     Play the appropriate effect.
        /// </summary>
        private void PlayEffect(bool instant)
        {
            if (graphic == null)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                graphic.canvasRenderer.SetAlpha(m_IsOn ? 1f : 0f);
            else
#endif
                graphic.CrossFadeAlpha(m_IsOn ? 1f : 0f, instant ? 0f : 0.1f, true);
        }

        /// <summary>
        ///     Assume the correct visual state.
        /// </summary>
        protected override void Start()
        {
            PlayEffect(true);
        }

        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable())
                return;

            isOn = !isOn;
        }

        [Serializable]
        public class ToggleEvent : UnityEvent<bool>
        {
        }
    }
}