using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class TakeSnapshot : MonoBehaviour
    {
        public delegate void OnSelectEvent();

        // Singleton 	
        private static TakeSnapshot _instance;

        // Construct 	
        private TakeSnapshot()
        {
        }

        //  Instance 	
        public static TakeSnapshot Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType(typeof(TakeSnapshot)) as TakeSnapshot;
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