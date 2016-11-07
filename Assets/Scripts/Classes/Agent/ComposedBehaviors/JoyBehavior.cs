using System;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.ComposedBehaviors
{
    public class JoyBehavior : ComposedBehavior
    {
        private readonly Color _behaviorColor = Configuration.Instance.ColorNames[Configuration.Colors.Yellow];

        public JoyBehavior(float standardMultiplier, float excitedMultiplier) : base(standardMultiplier, excitedMultiplier)
        {
            BehaviorType = Configuration.ComposedBehaviors.Joy;
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
                                Configuration.Transitions.EaseInOut, 4, duration);
                            break;
                        case Configuration.Behaviors.Resize:
                            (behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Medium,
                                Configuration.Transitions.EaseInOut, 3, duration);
                            break;
                        case Configuration.Behaviors.Rotate:
                            (behavior as RotationBehavior).PrepareBehavior(body, 360.0f, Configuration.RotationDirection.Random, 
                                Configuration.Transitions.EaseIn, 2, duration);
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
                                Configuration.Transitions.EaseInOut, 2, duration);
                            break;
                        case Configuration.Behaviors.Resize:
                            (behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Medium,
                                Configuration.Transitions.EaseInOut, 1, duration);
                            break;
                        case Configuration.Behaviors.Rotate:
                            (behavior as RotationBehavior).PrepareBehavior(body, 360.0f, Configuration.RotationDirection.Random, 
                                Configuration.Transitions.EaseIn, 1, duration);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

        }
    }
}