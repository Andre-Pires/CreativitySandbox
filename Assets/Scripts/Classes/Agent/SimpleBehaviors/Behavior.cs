using System.Threading;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Interface;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.SimpleBehaviors
{
    public abstract class Behavior : IBehavior
    {
        public Configuration.Behaviors BehaviorType;
        public bool IsOver = true;
        public float BehaviorDrive = Random.Range(0.0f, 35.0f);
        protected float StartTime;
        protected float BehaviorDuration;
        protected float DriveMultiplier;
        protected const float DriveStep = 3.0f;

        protected Behavior(float multiplier, bool behaviorDriveActive = true)
        {
            DriveMultiplier = multiplier;

            if (behaviorDriveActive)
            {
                UpdateBehaviorDriver();
            }
        }

        public void StartBehavior()
        {
            StartTime = Time.time;
            IsOver = false;
        }

        protected void UpdateBehaviorDriver()
        {
            if (BehaviorDrive <= 100)
            {
                BehaviorDrive += DriveStep * DriveMultiplier;
            }

            //Debug.Log("inercia state " + InerciaDriver);

            new Thread(() =>
            {
                Thread.Sleep(1000);
                UpdateBehaviorDriver();
            }).Start();
        }

        public abstract void PrepareBehavior(Body body, float duration);

        public abstract void ApplyBehavior(Body agentBody);

        public abstract void FinalizeEffects(Body body);
    }
}