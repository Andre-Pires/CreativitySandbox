using System;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.ComposedBehaviors
{
    public class DisgustBehavior : ComposedBehavior
    {
        private readonly Color _behaviorColor = Configuration.Instance.ColorNames[Configuration.Colors.DarkGreen];

        public DisgustBehavior(float standardMultiplier, float excitedMultiplier, Animator animator = null) : base(standardMultiplier, excitedMultiplier, animator)
        {
            BehaviorType = Configuration.ComposedBehaviors.Disgust;
            StandardAnimation = new AnimationBehavior(Animator, "disgust", "TriggerDisgustStandard", "SpeedDisgustStandard");
            ExcitedAnimation = new AnimationBehavior(Animator, "disgust", "TriggerDisgustExcited", "SpeedDisgustExcited");
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
                                Configuration.Transitions.EaseInOut, 3, BehaviorDuration);
                            break;
                        case Configuration.Behaviors.Resize:
                            /*(behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Small,
                                Configuration.Transitions.EaseInOut, 5, 2.0f, true);*/
                            break;
                        case Configuration.Behaviors.Rotate:
                            /*(behavior as RotationBehavior).PrepareBehavior(body, 90.0f, Configuration.RotationDirection.Right, 
                                Configuration.Transitions.EaseIn, 1, 4.0f);*/
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
                                Configuration.Transitions.EaseInOut, 2, BehaviorDuration);
                            break;
                        case Configuration.Behaviors.Resize:
                            /*(behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Small,
                                Configuration.Transitions.EaseInOut, 3, 4.0f);*/
                            break;
                        case Configuration.Behaviors.Rotate:
                            /*(behavior as RotationBehavior).PrepareBehavior(body, 90.0f, Configuration.RotationDirection.Right, 
                                Configuration.Transitions.EaseIn, 1, 3.0f);*/
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

        }
    }
}