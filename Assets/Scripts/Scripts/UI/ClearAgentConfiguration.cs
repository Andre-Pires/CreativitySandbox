using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class ClearAgentConfiguration : MonoBehaviour
    {
        // Singleton 	
        private static ClearAgentConfiguration _instance;

        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

        // Construct 	
        private ClearAgentConfiguration()
        {
        }

        //  Instance 	
        public static ClearAgentConfiguration Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType(typeof(ClearAgentConfiguration)) as ClearAgentConfiguration;
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
 