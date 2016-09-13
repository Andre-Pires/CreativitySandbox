using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class PauseVideoRecording : MonoBehaviour
    {
        public delegate void OnSelectEvent();

        // Singleton 	
        private static PauseVideoRecording _instance;

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
                    _instance = FindObjectOfType(typeof(PauseVideoRecording)) as PauseVideoRecording;
                return _instance;
            }
        }

        public event OnSelectEvent OnSelect;

        // Handle our Ray and Hit
        private void Update()
        {
        }

        public void OnClick()
        {
            // Notify of the event!
            OnSelect();
        }
    }
}