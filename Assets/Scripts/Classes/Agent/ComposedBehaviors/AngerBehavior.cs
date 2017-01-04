using System;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.ComposedBehaviors
{
    public class AngerBehavior : ComposedBehavior
    {
        private readonly Color _behaviorColor = Configuration.Instance.ColorNames[Configuration.Colors.DarkRed];

        public AngerBehavior(float standardMultiplier, float excitedMultiplier, Animator animator = null) : base(standardMultiplier, excitedMultiplier, animator)
        {
            BehaviorType = Configuration.ComposedBehaviors.Anger;
            StandardAnimation = new AnimationBehavior(Animator, "angryStandard", "TriggerAngryStandard", "SpeedAngryStandard");
            ExcitedAnimation = new AnimationBehavior(Animator, "angryExcited", "TriggerAngryExcited", "SpeedAngryExcited");
        }

        public override void PrepareBehavior(Body body, Configuration.ActiveBehaviors behaviorToPrepare, float duration)
        {
            BehaviorDuration = duration;
            ActiveBehavior = behaviorToPrepare;

            if (ActiveBehavior == Configuration.ActiveBehaviors.ExcitedBehavior)
            {
                foreach (Behavior behavior in ExcitedBehaviors)
                {
                    switch (behavior.BehaviorType)
                    {
                        case Configuration.Behaviors.Blink:
                            (behavior as BlinkBehavior).PrepareBehavior(body, _behaviorColor,
                                Configuration.Transitions.EaseInOut, 3, 1.8f, 0.15f);
                            break;
                        case Configuration.Behaviors.Resize:
                            /*(behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Large,
                                Configuration.Transitions.EaseInOut, 2, 1.0f);*/
                            break;
                        case Configuration.Behaviors.Rotate:
                            /*(behavior as RotationBehavior).PrepareBehavior(body, 45.0f, Configuration.RotationDirection.Alternating, 
                                Configuration.Transitions.EaseIn, 12, 2.5f);*/
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                foreach (Behavior behavior in StandardBehaviors)
                {
                    switch (behavior.BehaviorType)
                    {
                        case Configuration.Behaviors.Blink:

                            (behavior as BlinkBehavior).PrepareBehavior(body, _behaviorColor,
                            Configuration.Transitions.EaseInOut, 2, 2.0f);
                            break;

                        case Configuration.Behaviors.Resize:
                            
                           /* (behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Large, 
                            Configuration.Transitions.EaseInOut, 1, 1.0f);*/
                           
                            break;
                        case Configuration.Behaviors.Rotate:
                            /*(behavior as RotationBehavior).PrepareBehavior(body, 45.0f, Configuration.RotationDirection.Alternating, 
                                Configuration.Transitions.EaseOut, 8, 2.0f);*/
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

        }
    }
}