using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class ChangeCameraMode : MonoBehaviour
    {
        public delegate void OnSelectEvent();

        // Singleton 	
        private static ChangeCameraMode _instance;

        // Construct 	
        private ChangeCameraMode()
        {
        }

        //  Instance 	
        public static ChangeCameraMode Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType(typeof(ChangeCameraMode)) as ChangeCameraMode;
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