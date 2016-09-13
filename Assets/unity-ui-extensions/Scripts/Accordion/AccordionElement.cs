///Credit ChoMPHi
///Sourced from - http://forum.unity3d.com/threads/accordion-type-layout.271818/

using System;
using Assets.Scripts.Accordion.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Accordion
{
    [RequireComponent(typeof(RectTransform), typeof(LayoutElement))]
    [AddComponentMenu("UI/Extensions/Accordion/Accordion Element")]
    public class AccordionElement : Toggle
    {
        [NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

        private Accordion m_Accordion;
        private LayoutElement m_LayoutElement;

        [SerializeField] private readonly float m_MinHeight = 18f;
        private RectTransform m_RectTransform;

        protected AccordionElement()
        {
            if (m_FloatTweenRunner == null)
                m_FloatTweenRunner = new TweenRunner<FloatTween>();

            m_FloatTweenRunner.Init(this);
        }

        protected override void Awake()
        {
            base.Awake();
            transition = Transition.None;
            toggleTransition = ToggleTransition.None;
            m_Accordion = gameObject.GetComponentInParent<Accordion>();
            m_RectTransform = transform as RectTransform;
            m_LayoutElement = gameObject.GetComponent<LayoutElement>();
            onValueChanged.AddListener(OnValueChanged);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (group == null)
            {
                var tg = GetComponentInParent<ToggleGroup>();

                if (tg != null)
                {
                    group = tg;
                }
            }

            var le = gameObject.GetComponent<LayoutElement>();

            if (le != null)
            {
                if (isOn)
                {
                    le.preferredHeight = -1f;
                }
                else
                {
                    le.preferredHeight = m_MinHeight;
                }
            }
        }
#endif

        public void OnValueChanged(bool state)
        {
            if (m_LayoutElement == null)
                return;

            var transition = m_Accordion != null ? m_Accordion.transition : Accordion.Transition.Instant;

            if (transition == Accordion.Transition.Instant)
            {
                if (state)
                {
                    m_LayoutElement.preferredHeight = -1f;
                }
                else
                {
                    m_LayoutElement.preferredHeight = m_MinHeight;
                }
            }
            else if (transition == Accordion.Transition.Tween)
            {
                if (state)
                {
                    StartTween(m_MinHeight, GetExpandedHeight());
                }
                else
                {
                    StartTween(m_RectTransform.rect.height, m_MinHeight);
                }
            }
        }

        protected float GetExpandedHeight()
        {
            if (m_LayoutElement == null)
                return m_MinHeight;

            var originalPrefH = m_LayoutElement.preferredHeight;
            m_LayoutElement.preferredHeight = -1f;
            var h = LayoutUtility.GetPreferredHeight(m_RectTransform);
            m_LayoutElement.preferredHeight = originalPrefH;

            return h;
        }

        protected void StartTween(float startFloat, float targetFloat)
        {
            var duration = m_Accordion != null ? m_Accordion.transitionDuration : 0.3f;

            var info = new FloatTween
            {
                duration = duration,
                startFloat = startFloat,
                targetFloat = targetFloat
            };
            info.AddOnChangedCallback(SetHeight);
            info.ignoreTimeScale = true;
            m_FloatTweenRunner.StartTween(info);
        }

        protected void SetHeight(float height)
        {
            if (m_LayoutElement == null)
                return;

            m_LayoutElement.preferredHeight = height;
        }
    }
}