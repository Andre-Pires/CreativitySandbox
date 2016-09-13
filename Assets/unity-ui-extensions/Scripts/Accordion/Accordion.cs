///Credit ChoMPHi
///Sourced from - http://forum.unity3d.com/threads/accordion-type-layout.271818/


using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Accordion
{
    [RequireComponent(typeof(VerticalLayoutGroup), typeof(ContentSizeFitter), typeof(ToggleGroup))]
    [AddComponentMenu("UI/Extensions/Accordion/Accordion Group")]
    public class Accordion : MonoBehaviour
    {
        public enum Transition
        {
            Instant,
            Tween
        }

        [SerializeField] private Transition m_Transition = Transition.Instant;
        [SerializeField] private float m_TransitionDuration = 0.3f;

        /// <summary>
        ///     Gets or sets the transition.
        /// </summary>
        /// <value>The transition.</value>
        public Transition transition
        {
            get { return m_Transition; }
            set { m_Transition = value; }
        }

        /// <summary>
        ///     Gets or sets the duration of the transition.
        /// </summary>
        /// <value>The duration of the transition.</value>
        public float transitionDuration
        {
            get { return m_TransitionDuration; }
            set { m_TransitionDuration = value; }
        }
    }
}