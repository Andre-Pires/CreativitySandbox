using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.SimpleBehaviors
{
    public class BlinkBehavior : Behavior
    {
        public Configuration.Transitions BlinkTransition;
        public Color Color;
        public Color BlinkColor;
        public float AnimationEndPause = 0;
        public BlinkBehavior(float multiplier, bool behaviorDriveActive = true) : base(multiplier, behaviorDriveActive)
        {
            BehaviorType = Configuration.Behaviors.Blink;
        }

        //this function randomizes the Behavior
        public override void PrepareBehavior(Body body, int repetitions, float duration)
        {
            AnimationEndPause = 0;
            KeepBehaviorSetting = false;
            var transitionsCount = Configuration.Instance.AvailableTransitions.Count;
            var colorsCount = Configuration.Instance.AvailableColors.Count;
            //color Behavior
            Configuration.Transitions colorTransition =
                Configuration.Instance.AvailableTransitions[Random.Range(0, transitionsCount)];
            Color finalColor;

            //ensuring that the transition is to a different value
            while (true)
            {
                finalColor = Configuration.Instance.AvailableColors[Random.Range(0, colorsCount)];

                if (finalColor != body.Color)
                {
                    break;
                }
            }

            Color = body.Color;
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

            MaxBehaviorRepetitions = repetitions;
            CurrentBehaviorRepetition = 1;
            AnimationIntervalTime = BehaviorDuration / MaxBehaviorRepetitions;
        }

        public void PrepareBehavior(Body body, Color finalColor, Configuration.Transitions transition, int repetitions,
            float duration, bool keepBehaviorSetting = false)
        {
            KeepBehaviorSetting = keepBehaviorSetting;
            Color = body.Color;
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

            MaxBehaviorRepetitions = repetitions;
            CurrentBehaviorRepetition = 1;
            AnimationIntervalTime = BehaviorDuration / MaxBehaviorRepetitions;
        }

        //this function allows to customize the Behavior in the mind
        public void PrepareBehavior(Body body, Color finalColor, Configuration.Transitions transition, int repetitions,
            float duration, float animationEndPause, bool keepBehaviorSetting = false)
        {
            
            AnimationEndPause = animationEndPause;
            PrepareBehavior(body, finalColor, transition, repetitions, duration, keepBehaviorSetting);
        }

        

        public override void ApplyBehavior(Body agentBody)
        {
            Renderer renderer = agentBody.Mesh.GetComponent<Renderer>();

            switch (BlinkTransition)
            {
                case Configuration.Transitions.Linear:
                    var lerp = (Time.time - StartTime)/AnimationIntervalTime;
                    renderer.material.color = Color.Lerp(Color, BlinkColor, lerp);
                    break;
                case Configuration.Transitions.Instant:
                    renderer.material.color = BlinkColor;
                    break;
                case Configuration.Transitions.EaseIn:
                {
                    Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseInSine);
                        renderer.material.color = Color.Lerp(Color, BlinkColor,
                        easeFunction(0, 1, Time.time - StartTime, AnimationIntervalTime));
                    break;
                }
                case Configuration.Transitions.EaseOut:
                {
                    Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseOutSine);
                    renderer.material.color = Color.Lerp(Color, BlinkColor, easeFunction(0, 1, Time.time - StartTime, AnimationIntervalTime));
                    break;
                }
                case Configuration.Transitions.EaseInOut:
                {
                    float totalTime = AnimationIntervalTime / 2;

                    if (Time.time - StartTime <= totalTime)
                    {
                        Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseInSine);
                        float timeElapsed = Time.time - StartTime;
                        renderer.material.color = Color.Lerp(Color, BlinkColor, easeFunction(0, 1, timeElapsed, totalTime));

                        //Debug.Log("easing in: " + easeFunction(0, 1, timeElapsed, totalTime));
                    }
                    //when the animation is over we pause before changing color
                    else if (Time.time - StartTime >= totalTime + AnimationEndPause)
                        {
                        Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseOutSine);
                        float timeElapsed = (Time.time - StartTime - AnimationEndPause)- totalTime;
                        renderer.material.color = Color.Lerp(Color, BlinkColor, 1-easeFunction(0, 1, timeElapsed, totalTime));

                        //Debug.Log("easing out: " + (1 - easeFunction(0, 1, timeElapsed, totalTime)));
                    }
                    break;
                }
            }

            //when the animation is over we pause before changing color
            if ((Time.time - StartTime) > AnimationIntervalTime + AnimationEndPause)
            {
                if (CurrentBehaviorRepetition == MaxBehaviorRepetitions) 
                {
                    IsOver = true;
                    FinalizeEffects(agentBody);
                    //Debug.Log("Behavior ended");
                    return;
                }
                CurrentBehaviorRepetition++;
                StartTime = Time.time;
            }
        }

        public override void FinalizeEffects(Body body)
        {
            if (KeepBehaviorSetting)
            {
                body.Color = BlinkColor;
            }
            else
            {
                body.Color = Color;
            }
        }

    }
}