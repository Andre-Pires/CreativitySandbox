using UnityEngine;

namespace Assets.Scripts.Scripts
{
    public class SetPicker : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void LoadChosenSet(string fullPath)
        {
            Destroy(GameObject.FindGameObjectWithTag("Scenario"));
            //place the picked set in our scene
            GameObject set = Instantiate(Resources.Load("Prefabs/" + fullPath)) as GameObject;
            set.transform.parent = GameObject.Find("Scene").transform;

        }
    }
}
