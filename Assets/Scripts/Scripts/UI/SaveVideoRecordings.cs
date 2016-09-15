using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class SaveVideoRecordings : MonoBehaviour
    {
        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

        // Singleton 	
        private static SaveVideoRecordings _instance;

        //due to the random order of execution in Unity's scripts, this assigment is required in the Awake function
        public void Awake()
        {
            _instance = FindObjectOfType(typeof(SaveVideoRecordings)) as SaveVideoRecordings;
        }

        // Construct 	
        private SaveVideoRecordings()
        {
        }

        //  Instance 	
        public static SaveVideoRecordings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType(typeof(SaveVideoRecordings)) as SaveVideoRecordings;
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