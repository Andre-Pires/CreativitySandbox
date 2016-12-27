using System;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.ComposedBehaviors
{
    public class SadnessBehavior : ComposedBehavior
    {
        private readonly Color _behaviorColor = Configuration.Instance.ColorNames[Configuration.Colors.DarkBlue];

        public SadnessBehavior(float standardMultiplier, float excitedMultiplier, Animator animator = null) : base(standardMultiplier, excitedMultiplier, animator)
        {
            BehaviorType = Configuration.ComposedBehaviors.Sadness;
        }

        public override void PrepareBehavior(Body body, Configuration.ActiveBehaviors behaviorToPrepare, float duration)
        {
            if (Animator != null)
            {
                Animator.SetTrigger("TriggerSad");
            }

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
                                Configuration.Transitions.EaseInOut, 2, 6.0f);
                            break;
                        case Configuration.Behaviors.Resize:
                            /*(behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Small,
                                Configuration.Transitions.EaseInOut, 1, 6.0f);*/
                            break;
                        case Configuration.Behaviors.Rotate:
                            /*(behavior as RotationBehavior).PrepareBehavior(body, 180.0f, Configuration.RotationDirection.Alternating, 
                                Configuration.Transitions.EaseOut, 1, 4, 2);*/
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
                            Configuration.Transitions.EaseInOut, 1, 4.0f);
                            break;

                        case Configuration.Behaviors.Resize:
                            /*(behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Small, 
                            Configuration.Transitions.EaseInOut, 1, 4.0f);*/
                            break;

                        case Configuration.Behaviors.Rotate:
                            /*(behavior as RotationBehavior).PrepareBehavior(body, 90.0f, Configuration.RotationDirection.Alternating, 
                                Configuration.Transitions.EaseOut, 1, 3, 1);*/
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

        }
    }
}