using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Interface;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Classes.Agent.ComposedBehaviors
{
    public abstract class ComposedBehavior : Behavior
    {
        public new Configuration.ComposedBehaviors BehaviorType;

        protected List<Behavior> StandardBehaviors;
        protected List<Behavior> ExcitedBehaviors;

        protected ComposedBehavior(float multiplier) : base(multiplier)
        {
            StandardBehaviors = new List<Behavior>();
            StandardBehaviors.Add(new BlinkBehavior(Random.Range(1.0f, 2.5f), false));
            StandardBehaviors.Add(new ResizeBehavior(Random.Range(1.0f, 2.5f), false));
            StandardBehaviors.Add(new RotationBehavior(Random.Range(1.0f, 2.5f), false));

            ExcitedBehaviors = new List<Behavior>();
            ExcitedBehaviors.Add(new BlinkBehavior(Random.Range(1.0f, 2.5f), false));
            ExcitedBehaviors.Add(new ResizeBehavior(Random.Range(1.0f, 2.5f), false));
            ExcitedBehaviors.Add(new RotationBehavior(Random.Range(1.0f, 2.5f), false));

            UpdateBehaviorDriver();
        }
    }
}