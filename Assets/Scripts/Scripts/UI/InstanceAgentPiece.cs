using System.Collections.Generic;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scripts.UI
{
    public class InstanceAgentPiece : MonoBehaviour
    {
        public delegate void OnSelectEvent(Configuration.Personality personality, Configuration.Size size);

        // to check size selection through mouse input
        private RaycastHit _hit;
        private Ray _ray;
        public GameObject PageTitle;

        public Configuration.Personality Personality;
        public List<GameObject> SizeSelectionButtons;
        public event OnSelectEvent OnSelect;

        // Construct 	
        public void Start()
        {
            for (var imageIndex = 0; imageIndex < SizeSelectionButtons.Count; imageIndex++)
            {
                //TODO add correct path when all assets are finished
                //SizeSelectionButtons[imageIndex].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Agent/" + Personality.ToString()
                //    + Configuration.Instance.SizeValues[imageIndex]);

                var buttonSize = (Configuration.Size) imageIndex;
                var sprite = Resources.Load<Sprite>("Images/Agent/" + buttonSize);
                var buttonImage = SizeSelectionButtons[imageIndex].GetComponent<Image>();
                buttonImage.sprite = sprite;

                //specific assets may be designed later but for now we tint with each personalities color
                var personalityColor = Configuration.Instance.PersonalityColors[Personality];
                buttonImage.color = new Color(personalityColor.r, personalityColor.g, personalityColor.b, 0.7f);

                var button = SizeSelectionButtons[imageIndex].GetComponent<Button>();
                //necessary otherwise all closures would get executed with the last value of the cicle
                var index = imageIndex;
                button.onClick.AddListener(() => OnSelect(Personality, buttonSize));
            }

            PageTitle.GetComponent<Text>().text = Constants.Instance.GetPersonalityString(Personality);
        }
    }
}