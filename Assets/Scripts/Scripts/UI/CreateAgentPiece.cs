using System.Collections.Generic;
using Assets.Scripts.Classes;
using UnityEngine;
using UnityEngine.UI;
using Application = Assets.Scripts.Classes.Application;

namespace Assets.Scripts.Scripts.UI
{
    public class CreateAgentPiece : MonoBehaviour
    {
        public delegate void OnSelectEvent(Configuration.Personality personality);
        public event OnSelectEvent OnSelect;

        public Configuration.Personality Personality;
        public List<GameObject> SizeSelectionButtons;

        // Construct 	
        public void Start()
        {
            for (int imageIndex = 0; imageIndex < SizeSelectionButtons.Count; imageIndex++)
            {
                //TODO add correct path when all assets are finished
                //SizeSelectionButtons[imageIndex].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Agent/" + Personality.ToString()
                //    + Configuration.Instance.AvailableSizes[imageIndex]);

                Sprite sprite = Resources.Load<Sprite>("Images/Agent/" + Configuration.Instance.AvailableSizes[imageIndex]);
                SizeSelectionButtons[imageIndex].GetComponent<Image>().sprite = sprite;
            }
        }


        // Handle our Ray and Hit
        void Update()
        {
        }

        public void OnClick()
        {
            // Notify of the event!
            OnSelect(Personality);
        }

    }
}
 