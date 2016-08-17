using UnityEngine;
using System.Collections;
using Assets.Scripts.Scripts.UI;

namespace Assets.Scripts.Scripts.UI
{
    public class SaveVideoRecordings : MonoBehaviour
    {
        // Singleton 	
        private static SaveVideoRecordings _instance;

        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

        // Construct 	
        private SaveVideoRecordings()
        {
        }

        //  Instance 	
        public static SaveVideoRecordings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType(typeof(SaveVideoRecordings)) as SaveVideoRecordings;
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
