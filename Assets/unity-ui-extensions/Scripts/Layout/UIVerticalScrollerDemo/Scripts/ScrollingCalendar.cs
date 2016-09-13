/// <summary>
/// Brought you by Mrs. YakaYocha
/// https://www.youtube.com/channel/UCHp8LZ_0-iCvl-5pjHATsgw
/// 
/// Please donate: https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=RJ8D9FRFQF9VS
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Layout.UIVerticalScrollerDemo.Scripts
{
    public class ScrollingCalendar : MonoBehaviour

    {
        public Text dateText;
        public GameObject daysButtonPrefab;
        private GameObject[] daysButtons;
        public RectTransform daysCenter;
        public RectTransform daysScrollingPanel;

        private int daysSet;
        private UIVerticalScroller daysVerticalScroller;

        public InputField inputFieldDays;
        public InputField inputFieldMonths;
        public InputField inputFieldYears;

        public RectTransform monthCenter;
        public GameObject monthsButtonPrefab;

        private GameObject[] monthsButtons;
        public RectTransform monthsScrollingPanel;
        private int monthsSet;
        private UIVerticalScroller monthsVerticalScroller;

        public GameObject yearsButtonPrefab;
        private GameObject[] yearsButtons;
        public RectTransform yearsCenter;
        public RectTransform yearsScrollingPanel;
        private int yearsSet;

        private UIVerticalScroller yearsVerticalScroller;

        private void InitializeYears()
        {
            var currentYear = int.Parse(DateTime.Now.ToString("yyyy"));

            var arrayYears = new int[currentYear + 1 - 1900];

            yearsButtons = new GameObject[arrayYears.Length];

            for (var i = 0; i < arrayYears.Length; i++)
            {
                arrayYears[i] = 1900 + i;

                var clone =
                    (GameObject)
                        Instantiate(yearsButtonPrefab, new Vector3(0, i*80, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
                clone.transform.SetParent(yearsScrollingPanel);
                clone.transform.localScale = new Vector3(1, 1, 1);
                clone.GetComponentInChildren<Text>().text = "" + arrayYears[i];
                clone.name = "Year_" + arrayYears[i];
                clone.AddComponent<CanvasGroup>();
                yearsButtons[i] = clone;
            }
        }

        //Initialize Months
        private void InitializeMonths()
        {
            var months = new int[12];

            monthsButtons = new GameObject[months.Length];
            for (var i = 0; i < months.Length; i++)
            {
                var month = "";
                months[i] = i;

                var clone =
                    (GameObject)
                        Instantiate(monthsButtonPrefab, new Vector3(0, i*80, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
                clone.transform.SetParent(monthsScrollingPanel);
                clone.transform.localScale = new Vector3(1, 1, 1);

                switch (i)
                {
                    case 0:
                        month = "Jan";
                        break;
                    case 1:
                        month = "Feb";
                        break;
                    case 2:
                        month = "Mar";
                        break;
                    case 3:
                        month = "Apr";
                        break;
                    case 4:
                        month = "May";
                        break;
                    case 5:
                        month = "Jun";
                        break;
                    case 6:
                        month = "Jul";
                        break;
                    case 7:
                        month = "Aug";
                        break;
                    case 8:
                        month = "Sep";
                        break;
                    case 9:
                        month = "Oct";
                        break;
                    case 10:
                        month = "Nov";
                        break;
                    case 11:
                        month = "Dec";
                        break;
                }

                clone.GetComponentInChildren<Text>().text = month;
                clone.name = "Month_" + months[i];
                clone.AddComponent<CanvasGroup>();
                monthsButtons[i] = clone;
            }
        }

        private void InitializeDays()
        {
            var days = new int[31];
            daysButtons = new GameObject[days.Length];

            for (var i = 0; i < days.Length; i++)
            {
                days[i] = i + 1;
                var clone =
                    (GameObject)
                        Instantiate(daysButtonPrefab, new Vector3(0, i*80, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
                clone.transform.SetParent(daysScrollingPanel);
                clone.transform.localScale = new Vector3(1, 1, 1);
                clone.GetComponentInChildren<Text>().text = "" + days[i];
                clone.name = "Day_" + days[i];
                clone.AddComponent<CanvasGroup>();
                daysButtons[i] = clone;
            }
        }

        // Use this for initialization
        public void Awake()
        {
            InitializeYears();
            InitializeMonths();
            InitializeDays();

            //Yes Unity complains about this but it doesn't matter in this case.
            monthsVerticalScroller = new UIVerticalScroller(monthsScrollingPanel, monthsButtons, monthCenter);
            yearsVerticalScroller = new UIVerticalScroller(yearsScrollingPanel, yearsButtons, yearsCenter);
            daysVerticalScroller = new UIVerticalScroller(daysScrollingPanel, daysButtons, daysCenter);

            monthsVerticalScroller.Start();
            yearsVerticalScroller.Start();
            daysVerticalScroller.Start();
        }

        public void SetDate()
        {
            daysSet = int.Parse(inputFieldDays.text) - 1;
            monthsSet = int.Parse(inputFieldMonths.text) - 1;
            yearsSet = int.Parse(inputFieldYears.text) - 1900;

            daysVerticalScroller.SnapToElement(daysSet);
            monthsVerticalScroller.SnapToElement(monthsSet);
            yearsVerticalScroller.SnapToElement(yearsSet);
        }

        private void Update()
        {
            monthsVerticalScroller.Update();
            yearsVerticalScroller.Update();
            daysVerticalScroller.Update();

            var dayString = daysVerticalScroller.GetResults();
            var monthString = monthsVerticalScroller.GetResults();
            var yearsString = yearsVerticalScroller.GetResults();

            if (dayString.EndsWith("1") && dayString != "11")
                dayString = dayString + "st";
            else if (dayString.EndsWith("2") && dayString != "12")
                dayString = dayString + "nd";
            else if (dayString.EndsWith("3") && dayString != "13")
                dayString = dayString + "rd";
            else
                dayString = dayString + "th";

            dateText.text = monthString + " " + dayString + " " + yearsString;
        }

        public void DaysScrollUp()
        {
            daysVerticalScroller.ScrollUp();
        }

        public void DaysScrollDown()
        {
            daysVerticalScroller.ScrollDown();
        }

        public void MonthsScrollUp()
        {
            monthsVerticalScroller.ScrollUp();
        }

        public void MonthsScrollDown()
        {
            monthsVerticalScroller.ScrollDown();
        }

        public void YearsScrollUp()
        {
            yearsVerticalScroller.ScrollUp();
        }

        public void YearsScrollDown()
        {
            yearsVerticalScroller.ScrollDown();
        }
    }
}