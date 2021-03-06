﻿/// Credit Mrs. YakaYocha 
/// Sourced from - https://www.youtube.com/channel/UCHp8LZ_0-iCvl-5pjHATsgw
/// Please donate: https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=RJ8D9FRFQF9VS

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Layout
{
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("Layout/Extensions/Vertical Scroller")]
    public class UIVerticalScroller : MonoBehaviour
    {
        [Tooltip("Elements to populate inside the scroller")] public GameObject[] _arrayOfElements;

        [Tooltip("Center display area (position of zoomed content)")] public RectTransform _center;

        [Tooltip("Scrollable area (content of desired ScrollRect)")] public RectTransform _scrollingPanel;

        [Tooltip("Event fired when a specific item is clicked, exposes index number of item. (optional)")] public
            UnityEvent<int> ButtonClicked;

        //private int elementHalfLength;
        private float deltaY;
        private float[] distance;


        private float[] distReposition;
        private int elementLength;
        //private int elementsDistance;
        private int minElementsNum;
        private string result;
        public bool DeactivateOtherButtons;

        [Tooltip("Button to go to the previous page. (optional)")] public GameObject ScrollDownButton;

        [Tooltip("Button to go to the next page. (optional)")] public GameObject ScrollUpButton;

        [Tooltip("Select the item to be in center on start. (optional)")] public int StartingIndex = -1;

        public UIVerticalScroller()
        {
        }

        public UIVerticalScroller(RectTransform scrollingPanel, GameObject[] arrayOfElements, RectTransform center)
        {
            _scrollingPanel = scrollingPanel;
            _arrayOfElements = arrayOfElements;
            _center = center;
        }


        public void Awake()
        {
            var scrollRect = GetComponent<ScrollRect>();
            if (!_scrollingPanel)
            {
                _scrollingPanel = scrollRect.content;
            }
            if (!_center)
            {
                Debug.LogError("Please define the RectTransform for the Center viewport of the scrollable area");
            }
            if (_arrayOfElements == null || _arrayOfElements.Length == 0)
            {
                var childCount = scrollRect.content.childCount;
                if (childCount > 0)
                {
                    _arrayOfElements = new GameObject[childCount];
                    for (var i = 0; i < childCount; i++)
                    {
                        _arrayOfElements[i] = scrollRect.content.GetChild(i).gameObject;
                    }
                }
            }
        }

        public void Start()
        {
            if (_arrayOfElements.Length < 1)
            {
                Debug.Log("No child content found, exiting..");
                return;
            }

            elementLength = _arrayOfElements.Length;
            distance = new float[elementLength];
            distReposition = new float[elementLength];

            //get distance between buttons
            //elementsDistance = (int)Mathf.Abs(_arrayOfElements[1].GetComponent<RectTransform>().anchoredPosition.y - _arrayOfElements[0].GetComponent<RectTransform>().anchoredPosition.y);
            deltaY = _arrayOfElements[0].GetComponent<RectTransform>().rect.height*elementLength/3*2;
            var startPosition = new Vector2(_scrollingPanel.anchoredPosition.x, -deltaY);
            _scrollingPanel.anchoredPosition = startPosition;

            for (var i = 0; i < _arrayOfElements.Length; i++)
            {
                AddListener(_arrayOfElements[i], i);
            }

            if (ScrollUpButton)
                ScrollUpButton.GetComponent<Button>().onClick.AddListener(() => { ScrollUp(); });

            if (ScrollDownButton)
                ScrollDownButton.GetComponent<Button>().onClick.AddListener(() => { ScrollDown(); });

            if (StartingIndex > -1)
            {
                StartingIndex = StartingIndex > _arrayOfElements.Length ? _arrayOfElements.Length - 1 : StartingIndex;
                SnapToElement(StartingIndex);
            }
        }

        private void AddListener(GameObject button, int index)
        {
            button.GetComponent<Button>().onClick.AddListener(() => DoSomething(index));
        }

        private void DoSomething(int index)
        {
            if (ButtonClicked != null)
            {
                ButtonClicked.Invoke(index);
            }
        }

        public void Update()
        {
            if (_arrayOfElements.Length < 1)
            {
                return;
            }

            for (var i = 0; i < elementLength; i++)
            {
                distReposition[i] = _center.GetComponent<RectTransform>().position.y -
                                    _arrayOfElements[i].GetComponent<RectTransform>().position.y;
                distance[i] = Mathf.Abs(distReposition[i]);

                //Magnifying effect
                var scale = Mathf.Max(0.7f, 1/(1 + distance[i]/200));
                _arrayOfElements[i].GetComponent<RectTransform>().transform.localScale = new Vector3(scale, scale, 1f);
            }
            var minDistance = Mathf.Min(distance);

            for (var i = 0; i < elementLength; i++)
            {
                if (DeactivateOtherButtons)
                {
                    _arrayOfElements[i].GetComponent<CanvasGroup>().interactable = false;
                }

                if (minDistance == distance[i])
                {
                    minElementsNum = i;

                    if (DeactivateOtherButtons)
                    {
                        _arrayOfElements[i].GetComponent<CanvasGroup>().interactable = true;
                    }
                    result = _arrayOfElements[i].GetComponentInChildren<Text>().text;
                }
            }

            ScrollingElements(-_arrayOfElements[minElementsNum].GetComponent<RectTransform>().anchoredPosition.y);
        }

        private void ScrollingElements(float position)
        {
            var newY = Mathf.Lerp(_scrollingPanel.anchoredPosition.y, position, Time.deltaTime*1f);
            var newPosition = new Vector2(_scrollingPanel.anchoredPosition.x, newY);
            _scrollingPanel.anchoredPosition = newPosition;
        }

        public string GetResults()
        {
            return result;
        }

        public void SnapToElement(int element)
        {
            var deltaElementPositionY = _arrayOfElements[0].GetComponent<RectTransform>().rect.height*element;
            var newPosition = new Vector2(_scrollingPanel.anchoredPosition.x, -deltaElementPositionY);
            _scrollingPanel.anchoredPosition = newPosition;
        }

        public void ScrollUp()
        {
            var deltaUp = _arrayOfElements[0].GetComponent<RectTransform>().rect.height/1.2f;
            var newPositionUp = new Vector2(_scrollingPanel.anchoredPosition.x,
                _scrollingPanel.anchoredPosition.y - deltaUp);
            _scrollingPanel.anchoredPosition = Vector2.Lerp(_scrollingPanel.anchoredPosition, newPositionUp, 1);
        }

        public void ScrollDown()
        {
            var deltaDown = _arrayOfElements[0].GetComponent<RectTransform>().rect.height/1.2f;
            var newPositionDown = new Vector2(_scrollingPanel.anchoredPosition.x,
                _scrollingPanel.anchoredPosition.y + deltaDown);
            _scrollingPanel.anchoredPosition = newPositionDown;
        }
    }
}