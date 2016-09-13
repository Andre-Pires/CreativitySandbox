using System;
using Assets.Scripts.Classes.Helpers;

namespace Assets.Scripts.Classes.Agent
{
    public class Mind
    {
        private readonly Body _body;
        private readonly Configuration.Personality _personality;


        public Mind(Configuration.Personality personality, Body body)
        {
            _personality = personality;
            _body = body;
            SetupBehavior();
        }

        public void ChangeBehaviour(Body body)
        {
            //will allow outside events to affect emotional state
            throw new NotImplementedException();
        }

        public void Update()
        {
            //Empty for now
        }

        //where color, sound, movement patterns are initially established
        private void SetupBehavior()
        {
            var color = Configuration.Instance.PersonalityColors[_personality];
            var blinkingSpeed = Configuration.Instance.PersonalityBlinkingSpeeds[_personality];
            _body.SetupBehavior(color, blinkingSpeed);
        }
    }
}