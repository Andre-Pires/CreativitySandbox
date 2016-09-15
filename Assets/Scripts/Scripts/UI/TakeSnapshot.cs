using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class TakeSnapshot : MonoBehaviour
    {
        public delegate void OnSelectEvent();
        public event OnSelectEvent OnSelect;

        // Singleton 	
        private static TakeSnapshot _instance;

        //due to the random order of execution in Unity's scripts, this assigment is required in the Awake function
        public void Awake()
        {
            _instance = FindObjectOfType(typeof(TakeSnapshot)) as TakeSnapshot;
        }

        // Construct 	
        private TakeSnapshot()
        {
        }

        //  Instance 	
        public static TakeSnapshot Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(TakeSnapshot)) as TakeSnapshot;
                }
                return _instance;
            }
        }

        public void OnClick()
        {
            // Notify of the event!
            OnSelect();
        }
    }
}