using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent
{
    public class Mind
    {
        private Configuration.Personality _personality;
        private Body _body;


        public Mind(Configuration.Personality personality, Body body)
        {
            _personality = personality;
            _body = body;
            SetupBehavior();
        }
        
        public void ChangeBehaviour(Body body)
        {
            //will allow outside events to affect emotional state
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            //Empty for now
        }

        //where color, sound, movement patterns are initially established
        private void SetupBehavior()
        {
            Color color = Configuration.Instance.PersonalityColors[_personality];
            Configuration.BlinkingSpeed blinkingSpeed = Configuration.Instance.PersonalityBlinkingSpeeds[_personality];
            _body.SetupBehavior(color, blinkingSpeed);
        }
    }
}