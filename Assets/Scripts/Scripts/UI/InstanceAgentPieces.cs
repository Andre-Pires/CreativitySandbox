using Assets.Scripts.Classes.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scripts.UI
{
    public class InstanceAgentPieces : MonoBehaviour
    {
        public GameObject ButtonList;
        public Button CloseButtonList;
        public GameObject NamingFieldsList;
        public GameObject ScreenOverlays;
        public GameObject MainCanvas;

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
                personalityItem.GetComponentInChildren<Text>().text = "";

                GameObject pieceNameInput = Instantiate(Resources.Load("Prefabs/UISettings/WritePieceName")) as GameObject;
                pieceNameInput.transform.SetParent(NamingFieldsList.transform, false);

                GameObject pieceThumbnail = Utility.GetChild(pieceNameInput, "Thumbnail");
                pieceThumbnail.GetComponentInChildren<Image>().sprite = sprite;
                pieceThumbnail.GetComponentInChildren<Image>().color = Configuration.Instance.PersonalityColors[personality];

                button.onClick.AddListener(() =>
                {
                    pieceNameInput.SetActive(true);
                    ScreenOverlays.SetActive(true);
                    MainCanvas.SetActive(false);
                    CloseButtonList.onClick.Invoke();
                });

                var tempPersonality = personality;
                pieceNameInput.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    string pieceName = pieceNameInput.GetComponentInChildren<InputField>().text;

                    if (pieceName != "")
                    {
                        transform.GetComponent<CreateAgentPiece>().OnTrigger(tempPersonality, pieceName);
                        pieceNameInput.SetActive(false);
                        ScreenOverlays.SetActive(false);
                        MainCanvas.SetActive(true);
                        pieceNameInput.GetComponentInChildren<InputField>().text = "";
                    }
                });

                personalityItem.transform.SetParent(ButtonList.transform, false);
            }

        }
         
    }
}