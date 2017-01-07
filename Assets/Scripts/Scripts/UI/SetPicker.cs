using System;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scripts.UI
{
    public class SetPicker : MonoBehaviour
    {
        public Scenario ScenarioToLoad;
        // Use this for initialization
        public void Awake()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(() => LoadChosenSet(ScenarioToLoad));

            try
            {
                if (PlayerPrefs.HasKey("currentScenario"))
                {
                    string storedScenario = PlayerPrefs.GetString("currentScenario");

                    //if this isn't the component responsible for that scenario, return
                    if (ScenarioToLoad.ToString() != storedScenario)
                    {
                        return;
                    }

                    int enumSize = Enum.GetNames(typeof(Scenario)).Length;

                    for (int j = 0; j < enumSize; j++)
                    {
                        if (storedScenario == ((Scenario)j).ToString())
                        {
                            LoadChosenSet((Scenario)j);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Scenario stored from last session couldn't be loaded: " + e.Message);
            }
           
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void LoadChosenSet(Scenario scenario)
        {
            //place the picked set in our scene
            var set = Instantiate(Resources.Load(Constants.Instance.ScenarioPath[scenario])) as GameObject;
            set.transform.SetParent(GameObject.Find("Scene").transform, false);
            set.name = scenario.ToString();

            Destroy(GameObject.FindGameObjectWithTag("Scenario"));

            PlayerPrefs.SetString("currentScenario", scenario.ToString());
            PlayerPrefs.Save();

            if (SessionLogger.Instance != null)
                SessionLogger.Instance.WriteToLogFile("Changed set to: " + scenario + ".");
        }
    }
}