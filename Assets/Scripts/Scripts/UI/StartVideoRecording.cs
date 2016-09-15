using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class StartVideoRecording : MonoBehaviour
    {
        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

        // Singleton 	
        private static StartVideoRecording _instance;

        //due to the random order of execution in Unity's scripts, this assigment is required in the Awake function
        public void Awake()
        {
            _instance = FindObjectOfType(typeof(StartVideoRecording)) as StartVideoRecording;
        }

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


        public void OnClick()
        {
            // Notify of the event!
            OnSelect();
        }
    }
}