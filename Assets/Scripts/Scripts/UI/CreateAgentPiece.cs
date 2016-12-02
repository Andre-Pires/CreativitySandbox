using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class CreateAgentPiece : MonoBehaviour
    {
        public delegate void OnAutonomousCreationEvent(Configuration.Personality piecePersonality, string pieceName);
        public event OnAutonomousCreationEvent OnAutonomousPieceCreation;

        public delegate void OnManualCreationEvent(Configuration.Personality piecePersonality, Configuration.Size size, string pieceName);
        public event OnManualCreationEvent OnManualPieceCreation;

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

        public void OnAutonomousTrigger(Configuration.Personality personality, string pieceName)
        {
            // Notify of the event!
            if (OnAutonomousPieceCreation != null)
            {
                OnAutonomousPieceCreation(personality, pieceName);
            }
            else
            {
                Debug.Log("Event listener was null");
            }
        }

        public void OnManualTrigger(Configuration.Personality personality, Configuration.Size size, string pieceName)
        {
            // Notify of the event!
            if (OnManualPieceCreation != null)
            {
                OnManualPieceCreation(personality, size, pieceName);
            }
            else
            {
                Debug.Log("Event listener was null");
            }
        }


    }
}