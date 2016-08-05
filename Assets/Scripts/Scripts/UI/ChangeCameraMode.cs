using UnityEngine;
using System.Collections;
using Assets.Scripts.Scripts.UI;

namespace Assets.Scripts.Scripts.UI
{
    public class ChangeCameraMode : MonoBehaviour
    {
        // Singleton 	
        private static ChangeCameraMode _instance;

        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

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
                    _instance = GameObject.FindObjectOfType(typeof(ChangeCameraMode)) as ChangeCameraMode;
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
