using System;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.ComposedBehaviors
{
    public class JoyBehavior : ComposedBehavior
    {
        private readonly Color _behaviorColor = Configuration.Instance.ColorNames[Configuration.Colors.Green];

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
                                Configuration.Transitions.EaseInOut, 4, 1.5f, true);
                            break;
                        case Configuration.Behaviors.Resize:
                            if (body.Size == Configuration.Size.Large)
                            {
                                (behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Medium,
                                    Configuration.Transitions.EaseInOut, 3, 1.5f);
                            }
                            else
                            {
                                (behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Large,
                                    Configuration.Transitions.EaseInOut, 3, 1.5f,true);
                            }
                            break;
                        case Configuration.Behaviors.Rotate:
                            (behavior as RotationBehavior).PrepareBehavior(body, 360.0f, Configuration.RotationDirection.Random, 
                                Configuration.Transitions.EaseIn, 2, 1.5f);
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
                            if (body.Color == _behaviorColor)
                            {
                                Color newColor = Configuration.Instance.PersonalityColors[Configuration.Personality.Joy];
                                (behavior as BlinkBehavior).PrepareBehavior(body, newColor,
                                Configuration.Transitions.EaseInOut, 2, 2.0f, true);
                            }
                            else
                            {
                                (behavior as BlinkBehavior).PrepareBehavior(body, _behaviorColor,
                                Configuration.Transitions.EaseInOut, 2, 2.0f);
                            }

                            break;
                        case Configuration.Behaviors.Resize:
                            if (body.Size == Configuration.Size.Large)
                            {
                                (behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Medium,
                                    Configuration.Transitions.EaseInOut, 2, 2.0f,true);
                            }
                            else
                            {
                                (behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Large,
                                    Configuration.Transitions.EaseInOut, 2, 2.0f);
                            }
                            break;
                        case Configuration.Behaviors.Rotate:
                            (behavior as RotationBehavior).PrepareBehavior(body, 360.0f, Configuration.RotationDirection.Random, 
                                Configuration.Transitions.EaseIn, 1, 2.0f);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

        }
    }
}