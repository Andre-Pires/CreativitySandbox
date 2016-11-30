using System;
using System.Collections.Generic;
using Assets.Scripts.Classes.Agent;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Layout;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Classes.UI
{
    public class PiecePopupUI
    {
        private readonly Piece _piece;
        private readonly GameObject _settingsPopup;
        private readonly GameObject _pieceButton;

        //settings popup references
        private GameObject _pieceSizeList;
        private Dictionary<string, int> _pieceSizeIndex;
        private GameObject _pieceColorList;
        private Dictionary<string, int> _pieceColorIndex;
        private GameObject _pieceBlinkTypeList;
        private Dictionary<string, int> _pieceBlinkTypeIndex;
        private GameObject _pieceBlinkSpeedList;
        private Dictionary<string, int> _pieceBlinkSpeedIndex;
        private bool _alreadyInitialized;

        //double click - to show settings popup
        private bool _popupActive = true;
        private bool _firstClickPopup;
        private float _initialTimePopup;
        private readonly float _interval = 0.6f;


        public PiecePopupUI(Piece piece, GameObject button)
        {
            _piece = piece;
            _pieceSizeIndex = new Dictionary<string, int>();
            _pieceColorIndex = new Dictionary<string, int>();

            if (Configuration.Instance.BlinkingBehaviorActive)
            {
                _pieceBlinkTypeIndex = new Dictionary<string, int>();
                _pieceBlinkSpeedIndex = new Dictionary<string, int>();
                _settingsPopup = Object.Instantiate(Resources.Load("Prefabs/UISettings/SettingsPopup")) as GameObject;
            }
            else
            {
                _settingsPopup = Object.Instantiate(Resources.Load("Prefabs/UISettings/SettingsPopup(w_o blink)")) as GameObject;
            }
            SetupPopupPrefab(_settingsPopup);
            //getting notified if properties are changed to update indexes
            _piece.Body.NotifyUI += UpdateSettingsIndex;
            _piece.Body.NotifyUI += UpdatePieceIcon;

            //Updating agent button with popup settings
            _pieceButton = button;
        }

        private void SetupPopupPrefab(GameObject settingsPopup)
        {
            if (settingsPopup != null)
            {
                settingsPopup.transform.SetParent(AppUIManager.Instance.MainCanvas.transform, false);
                settingsPopup.name = _piece.Name + "_Popup";

                {
                    _pieceSizeList = GameObject.Find(settingsPopup.name + "/SizeOptions/Sizes");
                    GameObject listObject = Utility.GetChild(_pieceSizeList, "List");
                    int pageIndex = 0;

                    foreach (var size in Configuration.Instance.AvailableSizes)
                    {
                        var item = Object.Instantiate(Resources.Load("Prefabs/UISettings/SettingsItem")) as GameObject;
                        Configuration.Size tempSize = size;
                        item.name = size.ToString();

                        item.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            _piece.Body.Size = tempSize;
                            UpdatePieceIcon();
                        });
                        item.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Agent/" + size);
                        item.GetComponentInChildren<Text>().text = "";
                        item.GetComponent<RectTransform>().SetParent(listObject.transform, false);

                        //in order to present the user with the currently selected options
                        _pieceSizeIndex.Add(item.name, pageIndex);
                        pageIndex++;
                    }
                }

                {
                    _pieceColorList = GameObject.Find(settingsPopup.name + "/ColorOptions/Colors");
                    GameObject listObject = Utility.GetChild(_pieceColorList, "List");
                    int pageIndex = 0;

                    foreach (var color in Configuration.Instance.AvailableColors)
                    {
                        var item = Object.Instantiate(Resources.Load("Prefabs/UISettings/SettingsItem")) as GameObject;
                        var tempColor = color;
                        item.name = color.ToString();

                        item.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            _piece.Body.Color = tempColor;
                            UpdatePieceIcon();

                        });
                        item.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Agent/Small");
                        item.GetComponent<Image>().color = tempColor;
                        item.GetComponentInChildren<Text>().text = "";
                        item.GetComponent<RectTransform>().SetParent(listObject.transform, false);

                        //in order to present the user with the currently selected options
                        _pieceColorIndex.Add(item.name, pageIndex);
                        pageIndex++;
                    }
                }

                if (Configuration.Instance.BlinkingBehaviorActive)
                {
                    _pieceBlinkTypeList = GameObject.Find(settingsPopup.name + "/BlinkTypeOptions/BlinkTypes");
                    GameObject listObject = Utility.GetChild(_pieceBlinkTypeList, "List");
                    int pageIndex = 0;

                    foreach (var color in Configuration.Instance.AvailableColors)
                    {
                        var item = Object.Instantiate(Resources.Load("Prefabs/UISettings/SettingsItem")) as GameObject;
                        var tempColor = color;
                        item.name = color.ToString();


                        item.GetComponent<Button>().onClick.AddListener(() => _piece.Body.BlinkColor = tempColor);
                        item.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Buttons/BlinkColor");
                        item.GetComponent<Image>().color = tempColor;
                        item.GetComponentInChildren<Text>().text = "";
                        item.GetComponent<RectTransform>().SetParent(listObject.transform, false);

                        //in order to present the user with the currently selected options
                        _pieceBlinkTypeIndex.Add(item.name, pageIndex);
                        pageIndex++;
                    }
                }

                if (Configuration.Instance.BlinkingBehaviorActive)
                {
                    _pieceBlinkSpeedList = GameObject.Find(settingsPopup.name + "/BlinkSpeedOptions/BlinkSpeeds");
                    GameObject listObject = Utility.GetChild(_pieceBlinkSpeedList, "List");
                    int pageIndex = 0;

                    foreach (var speed in Configuration.Instance.AvailableBlinkSpeeds)
                    {
                        var item = Object.Instantiate(Resources.Load("Prefabs/UISettings/SettingsItem")) as GameObject;
                        var tempSpeed = speed;
                        item.name = speed.ToString();

                        item.GetComponent<Button>().onClick.AddListener(() => _piece.Body.BlinkSpeed = tempSpeed);
                        item.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Buttons/BlinkSpeed");
                        item.GetComponentInChildren<Text>().text = Constants.Instance.SpeedStrings[tempSpeed];

                        item.GetComponent<RectTransform>().SetParent(listObject.transform, false);

                        //in order to present the user with the currently selected options
                        _pieceBlinkSpeedIndex.Add(item.name, pageIndex);
                        pageIndex++;
                    }
                }
                


                // Since there's no way to raycast UI properly, a clickable backdrop was 
                // added to close the popup - a fixed resolution is sufficient for our application
                GameObject closeBackdrop = Utility.GetChild(settingsPopup, "Background");
                closeBackdrop.GetComponent<Button>().onClick.AddListener(ClosePopup);
                closeBackdrop.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
            }
            else
            {
                throw new NullReferenceException("The settings popup prefab wasn't properly loaded");
            }
        }

        private void ClosePopup()
        {
            _popupActive = false;
            _settingsPopup.SetActive(false);
        }

        public void Update()
        {
            HandleSettingsPopup();
        }

        private void HandleSettingsPopup()
        {
            if (_popupActive)
            {
                var popupSize = _settingsPopup.GetComponent<RectTransform>().rect.height;
                var screenPos = Camera.main.WorldToScreenPoint(_piece.Body.transform.position);
                screenPos.y += popupSize;
                _settingsPopup.transform.position = screenPos;

                if (!_alreadyInitialized)
                {
                    bool componentsInitialized = _pieceSizeList.GetComponent<ScrollSnap>().AlreadyInitialized;
                    componentsInitialized = componentsInitialized && _pieceColorList.GetComponent<ScrollSnap>().AlreadyInitialized;

                    if (Configuration.Instance.BlinkingBehaviorActive)
                    {
                        componentsInitialized = componentsInitialized &&
                                                _pieceBlinkTypeList.GetComponent<ScrollSnap>().AlreadyInitialized;
                        componentsInitialized = componentsInitialized &&
                                            _pieceBlinkSpeedList.GetComponent<ScrollSnap>().AlreadyInitialized;
                    }

                    if (componentsInitialized)
                    {
                        UpdateSettingsIndex();
                        _alreadyInitialized = true;
                    }
                }
                return;
            }

            // On double click show settings popup
            if (_firstClickPopup)
            {
                if (Time.time - _initialTimePopup > _interval)
                {
                    _firstClickPopup = false;
                }
                else if (Input.GetMouseButtonDown(0) && Utility.Instance.CheckIfClicked(_piece.Body.transform))
                {
                    _firstClickPopup = false;
                    _popupActive = true;
                    _settingsPopup.SetActive(true);
                    UpdateSettingsIndex();
                }
            }
            else if (Input.GetMouseButtonDown(0) && Utility.Instance.CheckIfClicked(_piece.Body.transform))
            {
                _firstClickPopup = true;
                _initialTimePopup = Time.time;
            }
        }

        public void OnDrawGizmos()
        {
            /*
            Gizmos.color = new Color(1, 0, 0, 0.5F);
            Gizmos.DrawCube(cubeObject.transform.position + Vector3.up * 10, new Vector3(1, 1, 1));
            */
        }

        public void OnGUI()
        {
        }

        public void DestroyPopupUI()
        {
            Object.Destroy(_settingsPopup);
        }

        public void UpdatePieceIcon()
        {
            _pieceButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Agent/" + _piece.Body.Size);
            _pieceButton.GetComponent<Image>().color = _piece.Body.Color;
        }

        public void UpdateSettingsIndex()
        { 
            _pieceSizeList.GetComponent<ScrollSnap>().ChangePage(_pieceSizeIndex[_piece.Body.Size.ToString()]);
            _pieceColorList.GetComponent<ScrollSnap>().ChangePage(_pieceColorIndex[_piece.Body.Color.ToString()]);

            if (Configuration.Instance.BlinkingBehaviorActive)
            {
                _pieceBlinkTypeList.GetComponent<ScrollSnap>().ChangePage(_pieceBlinkTypeIndex[_piece.Body.BlinkColor.ToString()]);
                _pieceBlinkSpeedList.GetComponent<ScrollSnap>().ChangePage(_pieceBlinkSpeedIndex[_piece.Body.BlinkSpeed.ToString()]);
            }
        }
    }
}