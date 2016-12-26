using System;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;
using UnityEngine.Experimental.Director;

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
            //TODO - just testing
            //extract this to initialization
            if (BehaviorType == Configuration.ComposedBehaviors.Joy)
            {
                Animator animator = body.Mesh.GetComponent<Animator>();
                animator.SetTrigger("TriggerHappy");
                
                /*AnimationClip clip = Resources.Load<AnimationClip>("Prefabs/Agent/happy");
                Animation animation = body.Mesh.GetComponent<Animation>();
                animation.AddClip(clip, BehaviorType.ToString());
                animation.Play(BehaviorType.ToString());*/
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
                                Configuration.Transitions.EaseInOut, 1, 1.5f);
                            break;
                        case Configuration.Behaviors.Resize:
                            /*(behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Large,
                                Configuration.Transitions.EaseInOut, 1, 1.5f);*/
                            break;
                        case Configuration.Behaviors.Rotate:
                            /*(behavior as RotationBehavior).PrepareBehavior(body, 720.0f, Configuration.RotationDirection.Random, 
                                Configuration.Transitions.EaseIn, 1, 1.5f);*/
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
                            Configuration.Transitions.EaseInOut, 1, 2.0f);
                            break;
                        case Configuration.Behaviors.Resize:
                            /*(behavior as ResizeBehavior).PrepareBehavior(body, Configuration.Size.Large,
                                Configuration.Transitions.EaseInOut, 1, 2.0f);*/
                            break;
                        case Configuration.Behaviors.Rotate:
                           /* (behavior as RotationBehavior).PrepareBehavior(body, 540.0f, Configuration.RotationDirection.Random, 
                                Configuration.Transitions.EaseIn, 1, 2.0f);*/
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

        }
    }
}