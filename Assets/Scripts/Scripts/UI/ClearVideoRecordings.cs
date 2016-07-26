using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class ClearVideoRecordings : MonoBehaviour
    {
        // Singleton 	
        private static ClearVideoRecordings _instance;

        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

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
                    _instance = GameObject.FindObjectOfType(typeof(ClearVideoRecordings)) as ClearVideoRecordings;
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
 