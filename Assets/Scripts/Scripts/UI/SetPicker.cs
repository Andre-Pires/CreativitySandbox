﻿using Assets.Scripts.Classes.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scripts.UI
{
    public class SetPicker : MonoBehaviour
    {
        public Scenario ScenarioToLoad;
        // Use this for initialization
        private void Start()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(() => LoadChosenSet(ScenarioToLoad));
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void LoadChosenSet(Scenario scenario)
        {
            //place the picked set in our scene
            var set = Instantiate(Resources.Load(Constants.Instance.ScenarioPath[scenario])) as GameObject;
            set.transform.parent = GameObject.Find("Scene").transform;

            Camera.main.gameObject.GetComponent<Skybox>().material = Constants.Instance.ScenarioSkybox[scenario];

            Destroy(GameObject.FindGameObjectWithTag("Scenario"));
        }
    }
}