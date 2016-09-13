using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Layout;
using UnityEngine;

namespace Assets.Scripts.Scripts.UI
{
    public class PersonalityPageSetup : MonoBehaviour
    {
        public HorizontalScrollSnap SliderScrollScript;


        // Use this for initialization
        public void Start()
        {
            var availablePersonalities = Configuration.Instance.AvailablePersonalities;

            foreach (var personality in availablePersonalities)
            {
                var tempPage = Instantiate(Resources.Load("Prefabs/UISettings/PersonalitySetupPage")) as GameObject;
                tempPage.GetComponent<InstanceAgentPiece>().Personality = personality;
                SliderScrollScript.AddChild(tempPage);
            }
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}