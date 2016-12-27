using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.IO;
using Assets.Scripts.Classes.UI;
using Assets.Scripts.Layout;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scripts.UI
{
    public class ColorScenario : MonoBehaviour
    {
        private List<Renderer> _scenarioPlanes;
        private GameObject _loadedScenario;
        private Configuration.Colors _currentSetColor;
        private Configuration.Colors _currentBackgroundColor;
        private GameObject _setColorPicker;
        private GameObject _skyboxColorPicker;
        private Dictionary<Configuration.Colors, int> _setColorPickerIndex;
        private Dictionary<Configuration.Colors, int> _skyboxColorPickerIndex;
        private List<GameObject> _setColorButtons;
        private bool _alreadyInitialized;

        // Use this for initialization
        public void Awake()
        {
            _setColorButtons = new List<GameObject>();
            _setColorPickerIndex = new Dictionary<Configuration.Colors, int>();
            _skyboxColorPickerIndex = new Dictionary<Configuration.Colors, int>();
            _setColorPicker = AppUIManager.Instance.SetColorPicker;
            _skyboxColorPicker = AppUIManager.Instance.SkyboxColorPicker;
            InitializeSetParameters();
            UpdateLoadedScenario();
            SetupColorPickers();
            //add listener to button to update the values shown
            AppUIManager.Instance.ColorMenuOpenButton.GetComponent<Button>().onClick.AddListener(UpdateToInUseColor);
        }


        // Update is called once per frame
        public void Update()
        {
            if (_loadedScenario == null)
            {
                UpdateLoadedScenario();
                UpdateColorPickers();
            }

            if (!_alreadyInitialized)
            {
                /*Debug.Log("set -> " + Utility.GetChild(_setColorPicker, "SetColorPicker").GetComponent<ScrollSnap>().AlreadyInitialized);
                Debug.Log("sky -> " + Utility.GetChild(_setColorPicker, "SkyboxColorPicker").GetComponent<ScrollSnap>().AlreadyInitialized);*/
                bool bothScrollSnapsWereInitialized =
                    _setColorPicker.GetComponent<ScrollSnap>().AlreadyInitialized &&
                    _skyboxColorPicker.GetComponent<ScrollSnap>().AlreadyInitialized;

                if (bothScrollSnapsWereInitialized)
                {
                    Debug.Log("updating color");
                    UpdateToInUseColor();
                    _alreadyInitialized = true;
                }
            }
        }

        // called on enable
        private void UpdateToInUseColor()
        {
            Utility.GetChild(_setColorPicker, "SetColorPicker").GetComponent<ScrollSnap>().ChangePage(_setColorPickerIndex[_currentSetColor]);
            Utility.GetChild(_skyboxColorPicker, "SkyboxColorPicker").GetComponent<ScrollSnap>().ChangePage(_skyboxColorPickerIndex[_currentBackgroundColor]);

            SessionLogger.Instance.WriteToLogFile("Updated the set's color.");
        }

        private void InitializeSetParameters()
        {
            int numberOfColors = Configuration.Instance.AvailableColors.Count;

            {
                Configuration.Colors randomColor = Configuration.Instance.ColorNames.Keys.ToList()[Random.Range(0, numberOfColors)];
                _currentSetColor = randomColor;
            }
            {
                _currentBackgroundColor = Configuration.Instance.ColorNames.Keys.ToList()[Random.Range(0, numberOfColors)];
                Color randomColor = Configuration.Instance.ColorNames[_currentBackgroundColor];
                Camera.main.backgroundColor = Color.Lerp(randomColor, Color.black, 0.2f);
            }
        }

        private void UpdateColorPickers()
        {
            foreach (var button in _setColorButtons)
            {
                var tempButton = button;
                button.GetComponent<Button>().onClick.RemoveAllListeners();
                button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    foreach (var plane in _scenarioPlanes)
                    {
                        plane.GetComponent<Renderer>().material.color = tempButton.GetComponent<Image>().color;
                    }

                    _currentSetColor = Configuration.Instance.ColorNames.FirstOrDefault(c => c.Value == tempButton.GetComponent<Image>().color).Key;
                });
            }
        }

        private void UpdateLoadedScenario()
        {
            _loadedScenario = GameObject.FindGameObjectWithTag("Scenario");
            _scenarioPlanes = _loadedScenario.GetComponentsInChildren<Renderer>().ToList();

            foreach (var plane in _scenarioPlanes)
            {
                plane.GetComponent<Renderer>().material.color = Configuration.Instance.ColorNames[_currentSetColor];
            }
        }

        public void SetupColorPickers()
        {
            {
                GameObject listObject = Utility.GetChild(_setColorPicker, "List");
                int pageIndex = 0;

                foreach (var color in Configuration.Instance.ColorNames)
                {
                    var item = Object.Instantiate(Resources.Load("Prefabs/UISettings/SettingsItem")) as GameObject;
                    var tempColor = Color.Lerp(color.Value, Color.black, 0.2f);
                    item.name = color.Key.ToString();

                    Configuration.Colors tempColorName = color.Key;
                    item.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        foreach (var plane in _scenarioPlanes)
                        {
                            plane.GetComponent<Renderer>().material.color = tempColor;
                        }
                        _currentSetColor = tempColorName;
                    });
                    item.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Scenario/ColorPickers/stain");
                    item.GetComponent<Image>().color = tempColor;
                    item.GetComponentInChildren<Text>().text = "";
                    item.GetComponent<RectTransform>().SetParent(listObject.transform, false);
                    _setColorButtons.Add(item);
                    _skyboxColorPickerIndex.Add(color.Key, pageIndex);
                    pageIndex++;
                }
            }

            {
                GameObject listObject = Utility.GetChild(_skyboxColorPicker, "List");
                int pageIndex = 0;

                foreach (var color in Configuration.Instance.ColorNames)
                {
                    var item = Object.Instantiate(Resources.Load("Prefabs/UISettings/SettingsItem")) as GameObject;
                    Color tempColor = Color.Lerp(color.Value, Color.black, 0.2f); ;
                    item.name = color.Key.ToString();

                    Configuration.Colors tempColorName = color.Key;
                    item.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        Camera.main.backgroundColor = tempColor;
                        _currentBackgroundColor = tempColorName;
                    });
                    item.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Scenario/ColorPickers/stain");
                    item.GetComponent<Image>().color = tempColor;
                    item.GetComponentInChildren<Text>().text = "";
                    item.GetComponent<RectTransform>().SetParent(listObject.transform, false);
                    _setColorPickerIndex.Add(color.Key, pageIndex);
                    pageIndex++;
                }
            }

            //A non-clickable backdrop was added to block raycasting
            GameObject closeBackdrop = AppUIManager.Instance.ColorPickerBackground;
            closeBackdrop.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        }
    }
}