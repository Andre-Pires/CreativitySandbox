using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class CreateAgentPiece : MonoBehaviour
    {
        public delegate void OnSelectEvent(Configuration.Personality piecePersonality, string pieceName);
        public event OnSelectEvent OnSelect;

        // Singleton 	
        private static CreateAgentPiece _instance;

        public void Awake()
        {
            if (_instance == null)
                _instance = FindObjectOfType(typeof(CreateAgentPiece)) as CreateAgentPiece;
        }

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

        // Handle our Ray and Hit
        private void Update()
        {
        }

        public void OnTrigger(Configuration.Personality personality, string pieceName)
        {
            // Notify of the event!
            if (OnSelect != null)
            {
                OnSelect(personality, pieceName);
            }
            else
            {
                Debug.Log("Event listener was null");
            }
        }
    }
}