using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class ManagePieceInstances : MonoBehaviour {

        public delegate void OnSelectEvent(string pieceName);
        public event OnSelectEvent OnSelect;
        public string PieceName;

        // Construct 	
        private ManagePieceInstances()
        {
        }

        public void OnClick()
        {
            // Notify of the event!
            OnSelect(PieceName);
        }
    }
}
