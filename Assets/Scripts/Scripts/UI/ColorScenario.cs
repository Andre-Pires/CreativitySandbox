using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Classes.Helpers;
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
        private Color _currentSetColor = Color.white;
        private GameObject _setColorPicker;
        private GameObject _skyboxColorPicker;
        private Dictionary<string, int> _setColorPickerIndex;
        private Dictionary<string, int> _skyboxColorPickerIndex;
        private List<GameObject> _setColorButtons;
        private bool _alreadyInitialized;

        // Use this for initialization
        private void Start()
        {
            _setColorButtons = new List<GameObject>();
            _setColorPickerIndex = new Dictionary<string, int>();
            _skyboxColorPickerIndex = new Dictionary<string, int>();
            _setColorPicker = AppUIManager.Instance.SetColorPicker;
            _skyboxColorPicker = AppUIManager.Instance.SkyboxColorPicker;
            InitializeSetParameters();
            UpdateLoadedScenario();
            SetupColorPickers();
            //add listener to button to update the values shown
            Utility.GetChild(gameObject, "OpenScenarioColors").GetComponent<Button>().onClick.AddListener(UpdateToInUseColor);
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
                
                bool bothScrollSnapsWereInitialized =
                    Utility.GetChild(_setColorPicker, "SetColorPicker").GetComponent<ScrollSnap>().AlreadyInitialized &&
                    Utility.GetChild(_skyboxColorPicker, "SkyboxColorPicker").GetComponent<ScrollSnap>().AlreadyInitialized;

                if (bothScrollSnapsWereInitialized)
                {
                    UpdateToInUseColor();
                    _alreadyInitialized = true;
                }
            }
        }

        // called on enable
        private void UpdateToInUseColor()
        {
            string scenarioColor = _scenarioPlanes[0].GetComponent<Renderer>().material.color.ToString();
            string skyboxColor = Camera.main.backgroundColor.ToString();
            Utility.GetChild(_setColorPicker, "SetColorPicker").GetComponent<ScrollSnap>().ChangePage(_setColorPickerIndex[scenarioColor]);
            Utility.GetChild(_skyboxColorPicker, "SkyboxColorPicker").GetComponent<ScrollSnap>().ChangePage(_skyboxColorPickerIndex[skyboxColor]);
        }

        private void InitializeSetParameters()
        {
            int numberOfColors = Configuration.Instance.AvailableColors.Count;

            {
                Color randomColor = Configuration.Instance.AvailableColors[Random.Range(0, numberOfColors)];
                _currentSetColor =  randomColor;
            }
            {
                Color randomColor = Configuration.Instance.AvailableColors[Random.Range(0, numberOfColors)];
                Camera.main.backgroundColor = randomColor;
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

                    _currentSetColor = tempButton.GetComponent<Image>().color;
                });
            }
        }

        private void UpdateLoadedScenario()
        {
            _loadedScenario = GameObject.FindGameObjectWithTag("Scenario");
            _scenarioPlanes = _loadedScenario.GetComponentsInChildren<Renderer>().ToList();

            foreach (var plane in _scenarioPlanes)
            {
                plane.GetComponent<Renderer>().material.color = _currentSetColor;
            }
        }

        public void SetupColorPickers()
        {
            {
                GameObject listObject = Utility.GetChild(_setColorPicker, "List");
                int pageIndex = 0;

                foreach (var color in Configuration.Instance.AvailableColors)
                {
                    var item = Object.Instantiate(Resources.Load("Prefabs/UISettings/SettingsItem")) as GameObject;
                    var tempColor = color;
                    item.name = color.ToString();

                    item.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        foreach (var plane in _scenarioPlanes)
                        {
                            plane.GetComponent<Renderer>().material.color = tempColor;
                        }
                        _currentSetColor = tempColor;
                    });
                    item.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Scenario/ColorPickers/stain");
                    item.GetComponent<Image>().color = tempColor;
                    item.GetComponentInChildren<Text>().text = "";
                    item.GetComponent<RectTransform>().SetParent(listObject.transform, false);
                    _setColorButtons.Add(item);
                    _skyboxColorPickerIndex.Add(item.name, pageIndex);
                    pageIndex++;
                }
            }

            {
                GameObject listObject = Utility.GetChild(_skyboxColorPicker, "List");
                int pageIndex = 0;

                foreach (var color in Configuration.Instance.AvailableColors)
                {
                    var item = Object.Instantiate(Resources.Load("Prefabs/UISettings/SettingsItem")) as GameObject;
                    var tempColor = color;
                    item.name = color.ToString();

                    item.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        Camera.main.backgroundColor = tempColor;
                    });
                    item.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Scenario/ColorPickers/stain");
                    item.GetComponent<Image>().color = tempColor;
                    item.GetComponentInChildren<Text>().text = "";
                    item.GetComponent<RectTransform>().SetParent(listObject.transform, false);
                    _setColorPickerIndex.Add(item.name, pageIndex);
                    pageIndex++;
                }
            }

            //A non-clickable backdrop was added to block raycasting
            GameObject closeBackdrop = Utility.GetChild(gameObject, "Background");
            closeBackdrop.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        }
    }
}