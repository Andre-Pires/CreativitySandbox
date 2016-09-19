using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class ClearAgentConfiguration : MonoBehaviour
    {
        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

        // Singleton 	
        private static ClearAgentConfiguration _instance;

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
                    _instance = FindObjectOfType(typeof(ClearAgentConfiguration)) as ClearAgentConfiguration;
                return _instance;
            }
        }

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