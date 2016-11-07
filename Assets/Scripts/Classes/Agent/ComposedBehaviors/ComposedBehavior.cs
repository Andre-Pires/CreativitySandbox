using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Interface;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.ComposedBehaviors
{
    public abstract class ComposedBehavior : IComposedBehavior
    {
        public Configuration.ComposedBehaviors BehaviorType;

        //excited behaviors
        public float ExcitedBehaviorDrive { get; private set; }
        protected float ExcitedDriveMultiplier;
        protected float ExcitedDriveStep = 0.75f;
        protected List<Behavior> ExcitedBehaviors;

        //standard behaviors
        public float StandardBehaviorDrive { get; private set; }
        protected float StandardDriveMultiplier;
        protected float StandardDriveStep = 3.0f;
        protected List<Behavior> StandardBehaviors;

        //generic variables
        protected Configuration.ActiveBehaviors ActiveBehavior = Configuration.ActiveBehaviors.StandardBehavior;
        protected float BehaviorDuration;
        protected float StartTime;
        public bool IsOver = true;

        protected ComposedBehavior(float standardMultiplier, float excitedMultiplier)
        {
            StandardBehaviorDrive = Random.Range(0.0f, 35.0f);
            StandardDriveMultiplier = standardMultiplier;
            ExcitedDriveMultiplier = excitedMultiplier;

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

        public void StartBehavior()
        {
            StartTime = Time.time;
            IsOver = false;

            //The rarer excited behaviors clear the standard drive but the standard behaviors do not affect the excited behaviors 
            if (ActiveBehavior == Configuration.ActiveBehaviors.ExcitedBehavior)
            {
                StandardBehaviorDrive = 0;
                ExcitedBehaviorDrive = 0;

                foreach (Behavior behavior in ExcitedBehaviors)
                {
                    behavior.StartBehavior();
                }

                Debug.Log("Starting excited " + BehaviorType);
            }
            else
            {
                StandardBehaviorDrive = 0;

                foreach (Behavior behavior in StandardBehaviors)
                {
                    behavior.StartBehavior();
                }

                Debug.Log("Starting standard " + BehaviorType);
            }
        }

        protected void UpdateBehaviorDriver()
        {
            if (StandardBehaviorDrive <= 100)
            {
                StandardBehaviorDrive += StandardDriveStep * StandardDriveMultiplier;
            }

            if (ExcitedBehaviorDrive <= 100)
            {
                ExcitedBehaviorDrive += ExcitedDriveStep * ExcitedDriveMultiplier;
            }

            //Debug.Log("inercia state " + InerciaDriver);

            new Thread(() =>
            {
                Thread.Sleep(1000);
                UpdateBehaviorDriver();
            }).Start();
        }

        public void ReceiveStimuli(Configuration.ProxemicDistance stimuliDistance, ComposedBehavior stimulatingBehavior)
        {
            //filtering Behavior to simulate needs at this point; only affecting inactive behaviors
            if (stimulatingBehavior.BehaviorType == BehaviorType && IsOver)
            {
                switch (stimuliDistance)
                {
                    case Configuration.ProxemicDistance.Social:
                        ExcitedBehaviorDrive += 30;
                        break;
                    case Configuration.ProxemicDistance.Personal:
                        ExcitedBehaviorDrive += 45;
                        break;
                    case Configuration.ProxemicDistance.Intimate:
                        ExcitedBehaviorDrive += 60;
                        break;
                }
            }
            else if (IsOver)
            {
                switch (stimuliDistance)
                {
                    case Configuration.ProxemicDistance.Social:
                        StandardBehaviorDrive += 25;
                        break;
                    case Configuration.ProxemicDistance.Personal:
                        StandardBehaviorDrive += 35;
                        break;
                    case Configuration.ProxemicDistance.Intimate:
                        StandardBehaviorDrive += 45;
                        break;
                }
            }

            if (StandardBehaviorDrive > 100)
            {
                StandardBehaviorDrive = 100;
            }

            if (ExcitedBehaviorDrive > 100)
            {
                ExcitedBehaviorDrive = 100;
            }

            //Debug.Log("Check stimuli: piece being called : " + transform.name + ", Behavior stimulus sent: " + stimulatingBehavior.BehaviorType);
            //Debug.Log("inercia after stimulus " + Behavior.StandardBehaviorDrive + ", ComplexBehavior: " + ComposedBehavior.BehaviorType);
        }

        public void ApplyBehavior(Body agentBody)
        {
            IsOver = true;

            if (ActiveBehavior == Configuration.ActiveBehaviors.ExcitedBehavior)
            {
                foreach (Behavior behavior in ExcitedBehaviors)
                {
                    behavior.ApplyBehavior(agentBody);

                    if (IsOver == true && !behavior.IsOver)
                    {
                        IsOver = false;
                    }
                }
            }
            else
            {
                foreach (Behavior behavior in StandardBehaviors)
                {
                    behavior.ApplyBehavior(agentBody);

                    if (IsOver == true && !behavior.IsOver)
                    {
                        IsOver = false;
                    }
                }
            }
            
        }

        public abstract void PrepareBehavior(Body body, Configuration.ActiveBehaviors behaviorToPrepare, float duration);
    }
}