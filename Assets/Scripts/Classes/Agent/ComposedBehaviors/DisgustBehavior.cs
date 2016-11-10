using System;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.ComposedBehaviors
{
    public class DisgustBehavior : ComposedBehavior
    {
        private readonly Color _behaviorColor = Configuration.Instance.ColorNames[Configuration.Colors.Green];

        public DisgustBehavior(float standardMultiplier, float excitedMultiplier) : base(standardMultiplier, excitedMultiplier)
        {
            BehaviorType = Configuration.ComposedBehaviors.Disgust;
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
                                Configuration.Transitions.EaseInOut, 8, duration);
                            break;
                        case Configuration.Behaviors.Resize:
                            (behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Small,
                                Configuration.Transitions.EaseInOut, 10, duration);
                            break;
                        case Configuration.Behaviors.Rotate:
                            (behavior as RotationBehavior).PrepareBehavior(body, 25.0f, Configuration.RotationDirection.Alternating, 
                                Configuration.Transitions.EaseIn, 6, duration);
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
                                Configuration.Transitions.EaseInOut, 4, duration);
                            break;
                        case Configuration.Behaviors.Resize:
                            (behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Small,
                                Configuration.Transitions.EaseInOut, 4, duration);
                            break;
                        case Configuration.Behaviors.Rotate:
                            (behavior as RotationBehavior).PrepareBehavior(body, 25.0f, Configuration.RotationDirection.Alternating, 
                                Configuration.Transitions.Linear, 4, duration);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

        }
    }
}