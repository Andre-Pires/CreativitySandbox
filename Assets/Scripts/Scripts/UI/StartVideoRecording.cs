using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class StartVideoRecording : MonoBehaviour
    {
        public delegate void OnSelectEvent();

        // Singleton 	
        private static StartVideoRecording _instance;

        // Construct 	
        private StartVideoRecording()
        {
        }

        //  Instance 	
        public static StartVideoRecording Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType(typeof(StartVideoRecording)) as StartVideoRecording;
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