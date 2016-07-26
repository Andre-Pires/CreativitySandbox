using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class StartVideoRecording : MonoBehaviour
    {
        // Singleton 	
        private static StartVideoRecording _instance;

        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

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
                    _instance = GameObject.FindObjectOfType(typeof(StartVideoRecording)) as StartVideoRecording;
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
 