using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class ClearVideoRecordings : MonoBehaviour
    {
        public delegate void OnSelectEvent();

        // Singleton 	
        private static ClearVideoRecordings _instance;

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