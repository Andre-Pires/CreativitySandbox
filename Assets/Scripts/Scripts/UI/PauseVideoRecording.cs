using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class PauseVideoRecording : MonoBehaviour
    {
        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

        // Singleton 	
        private static PauseVideoRecording _instance;

        //due to the random order of execution in Unity's scripts, this assigment is required in the Awake function
        public void Awake()
        {
            _instance = FindObjectOfType(typeof(PauseVideoRecording)) as PauseVideoRecording;
        }

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

        public void OnClick()
        {
            // Notify of the event!
            OnSelect();
        }
    }
}