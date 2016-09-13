using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class CreateAgentPiece : MonoBehaviour
    {
        public delegate void OnSelectEvent();

        // Singleton 	
        private static CreateAgentPiece _instance;

        // Construct 	
        private CreateAgentPiece()
        {
        }

        // Instance 	
        public static CreateAgentPiece Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType(typeof(CreateAgentPiece)) as CreateAgentPiece;
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