using Assets.Scripts.Classes.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Application = Assets.Scripts.Classes.Application;

namespace Assets.Scripts.Scripts.UI
{
    public class InstanceAgentPieces : MonoBehaviour
    {
        public GameObject ButtonList;
        public Button CloseButtonList;
        public GameObject NamingFieldsList;
        public GameObject SizeInputList;
        public GameObject ScreenOverlays;
        public GameObject MainCanvas;

        //piece staging area
        private Configuration.Size _sizeStagedPiece;
        private Configuration.Personality _personalityStagedPiece;

        //TODO - Remove this and put it in the AppUIManager
        public GameObject Root;

        // Construct 	
        public void Start()
        {
            foreach (Configuration.Personality personality in Configuration.Instance.AvailablePersonalities)
            {
                if (personality == Configuration.Personality.CustomPersonality)
                {
                    continue;
                }

                //button that pick the personality/color of the piece
                GameObject personalityItem =
                    Instantiate(Resources.Load("Prefabs/UISettings/PersonalityItem")) as GameObject;
                var button = personalityItem.GetComponent<Button>();
                var sprite =
                    Resources.Load<Sprite>("Images/Agent/" + Configuration.Instance.PersonalitySizes[personality]);
                personalityItem.GetComponent<Image>().sprite = sprite;
                personalityItem.GetComponent<Image>().color = Configuration.Instance.PersonalityColors[personality];
                personalityItem.GetComponentInChildren<Text>().text = "";

                //screen to input the piece's name
                GameObject pieceNameInput = Instantiate(Resources.Load("Prefabs/UISettings/WritePieceName")) as GameObject;
                pieceNameInput.transform.SetParent(NamingFieldsList.transform, false);
                pieceNameInput.name = personality.ToString();

                GameObject pieceThumbnail = Utility.GetChild(pieceNameInput, "Thumbnail");
                pieceThumbnail.GetComponentInChildren<Image>().sprite = sprite;
                pieceThumbnail.GetComponentInChildren<Image>().color = Configuration.Instance.PersonalityColors[personality];

                //screen to pick the piece's size
                GameObject pieceSizeInput = Instantiate(Resources.Load("Prefabs/UISettings/SizeSelection")) as GameObject;
                pieceSizeInput.name = personality.ToString();
                pieceSizeInput.transform.SetParent(SizeInputList.transform, false);
                GameObject piecesSizesList = Utility.GetChild(pieceSizeInput, "List");

                //TODO: test if the application is running in autonomous mode or manual and create the piece accordingly
                var tempPersonality = personality;
                button.onClick.AddListener(() =>
                {
                    if (Root.GetComponent<Application>().ActiveMode == Configuration.ApplicationMode.ManuallyActivatedAgent)
                    {
                        pieceSizeInput.SetActive(true);
                    }
                    else
                    {
                        pieceNameInput.SetActive(true);
                    }

                    ScreenOverlays.SetActive(true);
                    MainCanvas.SetActive(false);
                    CloseButtonList.onClick.Invoke();
                    _personalityStagedPiece = tempPersonality;
                });

                foreach (Configuration.Size size in Configuration.Instance.AvailableSizes)
                {
                    //size selection
                    {
                        var item = Object.Instantiate(Resources.Load("Prefabs/UISettings/SettingsItem")) as GameObject;
                        var tempSize = size;
                        item.name = size.ToString();

                        item.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            _sizeStagedPiece = tempSize;
                            Debug.Log("size " + _sizeStagedPiece);
                        });

                        Utility.GetChild(pieceSizeInput, "YesButton").GetComponent<Button>().onClick.AddListener(() =>
                        {
                            pieceNameInput.SetActive(true);
                            pieceSizeInput.SetActive(false);
                        });

                        item.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Agent/" + size);
                        item.GetComponent<Image>().color = Configuration.Instance.PersonalityColors[personality];
                        item.GetComponentInChildren<Text>().text = "";
                        item.GetComponent<RectTransform>().SetParent(piecesSizesList.transform, false);
                    }
                }

                
                pieceNameInput.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    string pieceName = pieceNameInput.GetComponentInChildren<InputField>().text;

                    if (pieceName != "")
                    {
                        MainCanvas.SetActive(true);
                        ScreenOverlays.SetActive(false);
                        pieceNameInput.SetActive(false);
                        pieceNameInput.GetComponentInChildren<InputField>().text = "";

                        if (Root.GetComponent<Application>().ActiveMode == Configuration.ApplicationMode.ManuallyActivatedAgent)
                        {
                            transform.GetComponent<CreateAgentPiece>().OnManualTrigger(_personalityStagedPiece, _sizeStagedPiece, pieceName);
                        }
                        else
                        {
                            transform.GetComponent<CreateAgentPiece>().OnAutonomousTrigger(_personalityStagedPiece, pieceName);
                        }
                    }
                });

                personalityItem.transform.SetParent(ButtonList.transform, false);
            }

            
        }
    }
}