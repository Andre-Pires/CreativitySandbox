using System.Threading;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Interface;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.Behaviors
{
    public abstract class Behavior : IBehavior
    {
        public Configuration.Behaviors BehaviorType;
        public bool IsOver = true;
        protected float StartTime;
        protected float BehaviorDuration;
        public float BehaviorDrive = Random.Range(0.0f, 35.0f);
        private float _driveMultiplier;
        private const float DriveStep = 3.0f;

        protected Behavior(float multiplier)
        {
            _driveMultiplier = multiplier;
            UpdateBehaviorDriver();
        }

        public void StartBehavior()
        {
            StartTime = Time.time;
            IsOver = false;
        }

        private void UpdateBehaviorDriver()
        {
            if (BehaviorDrive <= 100)
            {
                BehaviorDrive += DriveStep * _driveMultiplier;
            }

            //Debug.Log("inercia state " + InerciaDriver);

            new Thread(() =>
            {
                Thread.Sleep(1000);
                UpdateBehaviorDriver();
            }).Start();
        }

        public abstract void ApplyBehavior(Body agentBody);

        public abstract void FinalizeEffects(Body body);
    }
}