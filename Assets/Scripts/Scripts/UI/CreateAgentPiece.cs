using System.Collections.Generic;
using Assets.Scripts.Classes;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Application = Assets.Scripts.Classes.Application;

namespace Assets.Scripts.Scripts.UI
{
    public class CreateAgentPiece : MonoBehaviour
    {
        public delegate void OnSelectEvent(Configuration.Personality personality, Configuration.Size size);
        public event OnSelectEvent OnSelect;

        public Configuration.Personality Personality;
        public List<GameObject> SizeSelectionButtons;
        public GameObject PageTitle;

        // to check size selection through mouse input
        private RaycastHit _hit;
        private Ray _ray;

        // Construct 	
        public void Start()
        {
            for (int imageIndex = 0; imageIndex < SizeSelectionButtons.Count; imageIndex++)
            {
                //TODO add correct path when all assets are finished
                //SizeSelectionButtons[imageIndex].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Agent/" + Personality.ToString()
                //    + Configuration.Instance.AvailableSizes[imageIndex]);

                Configuration.Size buttonSize = (Configuration.Size) imageIndex;
                Sprite sprite = Resources.Load<Sprite>("Images/Agent/" + buttonSize);
                Image buttonImage = SizeSelectionButtons[imageIndex].GetComponent<Image>();
                buttonImage.sprite = sprite;

                //specific assets may be designed later but for now we tint with each personalities color
                Color personalityColor = Configuration.Instance.PersonalityColors[Personality];
                buttonImage.color = new Color(personalityColor.r, personalityColor.g, personalityColor.b, 0.7f);

                Button button = SizeSelectionButtons[imageIndex].GetComponent<Button>();
                //necessary otherwise all closures would get executed with the last value of the cicle
                var index = imageIndex;
                button.onClick.AddListener(() => OnSelect(Personality, buttonSize));
            }

            PageTitle.GetComponent<Text>().text = Constants.Instance.GetPersonalityString(Personality);
        }
    }
}
 