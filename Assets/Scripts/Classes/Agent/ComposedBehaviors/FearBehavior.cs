using System;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.ComposedBehaviors
{
    public class FearBehavior : ComposedBehavior
    {
        private readonly Color _behaviorColor = Configuration.Instance.ColorNames[Configuration.Colors.DarkPurple];

        public FearBehavior(float standardMultiplier, float excitedMultiplier, Animator animator = null) : base(standardMultiplier, excitedMultiplier, animator)
        {
            BehaviorType = Configuration.ComposedBehaviors.Fear;
            StandardAnimation = new AnimationBehavior(Animator, "fearStandard", "TriggerFearStandard", "SpeedFearStandard");
            ExcitedAnimation = new AnimationBehavior(Animator, "fearExcited", "TriggerFearExcited", "SpeedFearExcited");
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
                                Configuration.Transitions.EaseInOut, 3, 1.5f);
                            break;
                        case Configuration.Behaviors.Resize:
                            /*(behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Small,
                                Configuration.Transitions.EaseInOut, 1, 2.0f, true);*/
                            break;
                        case Configuration.Behaviors.Rotate:
                            /*(behavior as RotationBehavior).PrepareBehavior(body, 5.0f, Configuration.RotationDirection.Alternating, 
                                Configuration.Transitions.EaseIn, 30, 1.5f);*/
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
                            Configuration.Transitions.EaseInOut, 3, 2.0f);
                            break;
                        case Configuration.Behaviors.Resize:
                            /*(behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Small, 
                            Configuration.Transitions.EaseInOut, 1, 2.0f);*/
                            break;
                        case Configuration.Behaviors.Rotate:
                            /*(behavior as RotationBehavior).PrepareBehavior(body, 5.0f, Configuration.RotationDirection.Alternating, 
                                Configuration.Transitions.EaseIn, 25, 2.0f);*/
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

        }
    }
}