using System;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.ComposedBehaviors
{
    public class DisgustBehavior : ComposedBehavior
    {
        private readonly Color _behaviorColor = Configuration.Instance.ColorNames[Configuration.Colors.Purple];

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
                                Configuration.Transitions.EaseInOut, 8, 2.0f, true);
                            break;
                        case Configuration.Behaviors.Resize:
                            (behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Small,
                                Configuration.Transitions.EaseInOut, 5, 2.0f, true);
                            break;
                        case Configuration.Behaviors.Rotate:
                            (behavior as RotationBehavior).PrepareBehavior(body, 25.0f, Configuration.RotationDirection.Alternating, 
                                Configuration.Transitions.EaseIn, 6, 2.0f);
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
                                Color newColor = Configuration.Instance.PersonalityColors[Configuration.Personality.Disgust];
                                (behavior as BlinkBehavior).PrepareBehavior(body, newColor,
                                    Configuration.Transitions.EaseInOut, 4, 1.7f, true);
                            }
                            else
                            {
                                (behavior as BlinkBehavior).PrepareBehavior(body, _behaviorColor,
                                    Configuration.Transitions.EaseInOut, 4, 1.7f);
                            }
                            break;
                        case Configuration.Behaviors.Resize:
                            if (body.Size == Configuration.Size.Small)
                            {
                                (behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Medium,
                                    Configuration.Transitions.EaseInOut, 3, 1.7f, true);
                            }
                            else
                            {
                                (behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Small,
                                    Configuration.Transitions.EaseInOut, 3, 1.7f);
                            }
                            break;
                        case Configuration.Behaviors.Rotate:
                            (behavior as RotationBehavior).PrepareBehavior(body, 25.0f, Configuration.RotationDirection.Alternating, 
                                Configuration.Transitions.Linear, 4, 1.7f);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

        }
    }
}