using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class SaveVideoRecordings : MonoBehaviour
    {
        public delegate void OnSelectEvent();

        // Singleton 	
        private static SaveVideoRecordings _instance;

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