using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.Behaviors
{
    public class BlinkBehavior : Behavior
    {
        public Configuration.Transitions BlinkTransition;
        public Color Color;
        public Color BlinkColor;

        public BlinkBehavior(float multiplier) : base(multiplier)
        {
            BehaviorType = Configuration.Behaviors.Blink;
        }

        //this function randomizes the behavior
        public void PrepareBehavior(Color bodyColor, float duration) 
        {

            var transitionsCount = Configuration.Instance.AvailableTransitions.Count;
            var colorsCount = Configuration.Instance.AvailableColors.Count;
            //color behavior
            Configuration.Transitions colorTransition =
                Configuration.Instance.AvailableTransitions[Random.Range(0, transitionsCount)];
            Color finalColor;

            //ensuring that the transition is to a different value
            while (true)
            {
                finalColor = Configuration.Instance.AvailableColors[Random.Range(0, colorsCount)];

                if (finalColor != bodyColor)
                {
                    break;
                }
            }

            Color = bodyColor;
            BlinkColor = finalColor;
            BlinkTransition = colorTransition;

            if (colorTransition == Configuration.Transitions.Instant)
            {
                BehaviorDuration = 0.0f;
            }
            else
            {
                BehaviorDuration = duration;
            }
        }

        //this function allows to customize the behavior in the mind
        public void PrepareBehavior(Color bodyColor, Color finalColor, Configuration.Transitions transition, float duration)
        {

            Color = bodyColor;
            BlinkColor = finalColor;
            BlinkTransition = transition;

            if (transition == Configuration.Transitions.Instant)
            {
                BehaviorDuration = 0.0f;
            }
            else
            {
                BehaviorDuration = duration;
            }
        }

        public override void ApplyBehavior(Body agentBody)
        {
            
            if (BlinkTransition == Configuration.Transitions.Linear)
            {
                var lerp = (Time.time - StartTime)/BehaviorDuration;
                agentBody.GetComponent<Renderer>().material.color = Color.Lerp(Color, BlinkColor, lerp);
            }
            else if (BlinkTransition == Configuration.Transitions.Instant)
            {
                agentBody.GetComponent<Renderer>().material.color = BlinkColor;
            }
            else if ( BlinkTransition == Configuration.Transitions.EaseIn)
            {
                Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseInExpo);
                agentBody.GetComponent<Renderer>().material.color = Color.Lerp(Color, BlinkColor, easeFunction(0,1,Time.time - StartTime, BehaviorDuration));
            }

            if (Time.time - StartTime > BehaviorDuration)
            {
                IsOver = true;
                FinalizeEffects(agentBody);
                Debug.Log("Behavior ended");
            }

        }

        public override void FinalizeEffects(Body body)
        {
            body.Color = BlinkColor;
        }

    }
}