using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class PieceInstancingUI : MonoBehaviour {

        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

        // Construct 	
        private PieceInstancingUI()
        {
        }

        public void OnClick()
        {
            // Notify of the event!
            OnSelect();
        }
    }
}
