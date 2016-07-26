using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class TakeSnapshot : MonoBehaviour
    {
        // Singleton 	
        private static TakeSnapshot _instance;

        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

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
                    _instance = GameObject.FindObjectOfType(typeof(TakeSnapshot)) as TakeSnapshot;
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
 