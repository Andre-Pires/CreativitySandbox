using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class PauseVideoRecording : MonoBehaviour
    {
        // Singleton 	
        private static PauseVideoRecording _instance;

        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

        // Construct 	
        private PauseVideoRecording()
        {
        }

        //  Instance 	
        public static PauseVideoRecording Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType(typeof(PauseVideoRecording)) as PauseVideoRecording;
                return _instance;
            }

        }

        // Handle our Ray and Hit
        void Update()
        {
        }

        public void OnClick()
        {
            // Notify of the event!
            OnSelect();
        }

    }
}
 