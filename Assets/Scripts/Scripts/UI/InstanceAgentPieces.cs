using Assets.Scripts.Classes.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scripts.UI
{
    public class InstanceAgentPieces : MonoBehaviour
    {
        // Construct 	
        public void Start()
        {
            foreach (Configuration.Personality personality in Configuration.Instance.AvailablePersonalities)
            {
                if (personality == Configuration.Personality.CustomPersonality)
                {
                    continue;
                }

                GameObject personalityItem = Instantiate(Resources.Load("Prefabs/UISettings/PersonalityItem")) as GameObject;
                var button = personalityItem.GetComponent<Button>();
                var sprite = Resources.Load<Sprite>("Images/Agent/" + Configuration.Instance.PersonalitySizes[personality]);
                personalityItem.GetComponent<Image>().sprite = sprite;
                personalityItem.GetComponent<Image>().color = Configuration.Instance.PersonalityColors[personality];

                var tempPersonality = personality;
                button.onClick.AddListener(() => transform.GetComponent<CreateAgentPiece>().OnTrigger(tempPersonality));
                personalityItem.GetComponentInChildren<Text>().text = Constants.Instance.PersonalitiesStrings[personality];
                
                personalityItem.transform.SetParent(this.transform, false);
            }

        }
         
    }
}