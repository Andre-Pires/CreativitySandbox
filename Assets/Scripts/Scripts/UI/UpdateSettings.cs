using Assets.Scripts.Classes.Agent;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scripts.UI
{
    public class UpdateSettings : MonoBehaviour
    {
        public Piece Piece;

        // Use this for initialization
        private void Awake()
        {
            GameObject list = GameObject.Find("SizeOptions/Sizes/List");

            foreach (var size in Configuration.Instance.AvailableSizes)
            {
                var item = Instantiate(Resources.Load("Prefabs/UISettings/SettingsItem")) as GameObject;
                var tempSize = size;
                item.GetComponent<Button>().onClick.AddListener(() => Piece.UpdateSettings(tempSize));
                item.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Agent/" + size);
                item.GetComponentInChildren<Text>().text = "";

                item.GetComponent<RectTransform>().SetParent(list.transform, false);
            }

            list = GameObject.Find("ColorOptions/Colors/List");

            foreach (var color in Configuration.Instance.AvailableColors)
            {
                var item = Instantiate(Resources.Load("Prefabs/UISettings/SettingsItem")) as GameObject;
                var tempColor = color;
                item.GetComponent<Button>().onClick.AddListener(() => Piece.UpdateSettings(tempColor));
                item.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Agent/Medium");
                item.GetComponent<Image>().color = tempColor;
                item.GetComponentInChildren<Text>().text = "";

                item.GetComponent<RectTransform>().SetParent(list.transform, false);
            }

            list = GameObject.Find("BlinkTypeOptions/BlinkTypes/List");

            foreach (var color in Configuration.Instance.AvailableColors)
            {
                var item = Instantiate(Resources.Load("Prefabs/UISettings/SettingsItem")) as GameObject;
                var tempColor = color;
                item.GetComponent<Button>().onClick.AddListener(() => Piece.UpdateSettings(tempColor));
                item.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Buttons/BlinkColor");
                item.GetComponent<Image>().color = tempColor;
                item.GetComponentInChildren<Text>().text = "";

                item.GetComponent<RectTransform>().SetParent(list.transform, false);

                Debug.Log("cor: " + color);
            }

            list = GameObject.Find("BlinkSpeedOptions/BlinkSpeeds/List");

            foreach (var speed in Configuration.Instance.AvailableBlinkSpeeds)
            {
                var item = Instantiate(Resources.Load("Prefabs/UISettings/SettingsItem")) as GameObject;
                var tempSpeed = speed;
                item.GetComponent<Button>().onClick.AddListener(() => Piece.UpdateSettings(tempSpeed));
                item.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Buttons/BlinkSpeed");
                item.GetComponentInChildren<Text>().text = tempSpeed.ToString();

                item.GetComponent<RectTransform>().SetParent(list.transform, false);
            }
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}