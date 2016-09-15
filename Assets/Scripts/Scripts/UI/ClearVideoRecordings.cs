using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class ClearVideoRecordings : MonoBehaviour
    {
        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

        // Singleton 	
        private static ClearVideoRecordings _instance;

        //due to the random order of execution in Unity's scripts, this assigment is required in the Awake function
        public void Awake()
        {
            _instance = FindObjectOfType(typeof(ClearVideoRecordings)) as ClearVideoRecordings;
        }

        // Construct 	
        private ClearVideoRecordings()
        {
        }

        //  Instance 	
        public static ClearVideoRecordings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType(typeof(ClearVideoRecordings)) as ClearVideoRecordings;
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