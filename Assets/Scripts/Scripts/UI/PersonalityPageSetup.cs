using System.Collections.Generic;
using Assets.Scripts.Classes;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Layout;
using Assets.Scripts.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts
{

    public class PersonalityPageSetup : MonoBehaviour
    {
        public HorizontalScrollSnap SliderScrollScript;

      
        // Use this for initialization
        public void Start ()
        {
            List<Configuration.Personality> availablePersonalities = Configuration.Instance.AvailablePersonalities;

            foreach (Configuration.Personality personality in availablePersonalities)
            {
                GameObject tempPage = Object.Instantiate(Resources.Load("Prefabs/PersonalitySetupPage")) as GameObject;
                tempPage.GetComponent<CreateAgentPiece>().Personality = personality;
                SliderScrollScript.AddChild(tempPage);
            }
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}