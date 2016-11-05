using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.SimpleBehaviors
{
    public class RotationBehavior : Behavior
    {
        public Configuration.Transitions RotateTransition;
        public float Orientation;
        public float FinalOrientation;

        public RotationBehavior(float multiplier, bool behaviorDriveActive = true) : base(multiplier, behaviorDriveActive)
        {
            BehaviorType = Configuration.Behaviors.Rotate;
        }

        //this function randomizes the Behavior
        public override void PrepareBehavior(Body body, float duration)
        {
            var transitionsCount = Configuration.Instance.AvailableTransitions.Count;

            //rotation Behavior
            Configuration.Transitions rotTransition =
                Configuration.Instance.AvailableTransitions[Random.Range(0, transitionsCount)];
            float rotationAmount = Random.Range(0.0f, 720.0f);

            Orientation = body.CurrentRotation;
            FinalOrientation = rotationAmount;
            RotateTransition = rotTransition;

            if (rotTransition == Configuration.Transitions.Instant)
            {
                BehaviorDuration = 0.0f;
            }
            else
            {
                BehaviorDuration = duration;
            }

        }

        //this function allows to customize the Behavior in the mind
        public void PrepareBehavior(Body body, float rotationAmount, Configuration.Transitions rotationTransition, float duration)
        {
            Orientation = body.CurrentRotation;
            FinalOrientation = rotationAmount;
            RotateTransition = rotationTransition;

            if (rotationTransition == Configuration.Transitions.Instant)
            {
                BehaviorDuration = 0.0f;
            }
            else
            {
                BehaviorDuration = duration;
            }
        }

        public override void ApplyBehavior(Body agentBody)
        {
            float rotationX = agentBody.transform.rotation.eulerAngles.x;
            float rotationZ = agentBody.transform.rotation.eulerAngles.z;

            switch (RotateTransition)
            {
                case Configuration.Transitions.Linear:
                    var lerp = (Time.time - StartTime)/BehaviorDuration;
                    agentBody.transform.rotation = Quaternion.Slerp(agentBody.transform.rotation,
                        Quaternion.Euler(0, FinalOrientation, 0), lerp);
                    break;
                case Configuration.Transitions.Instant:
                    agentBody.transform.eulerAngles = new Vector3(rotationX, FinalOrientation, rotationZ);
                    break;
                case Configuration.Transitions.EaseIn:
                {
                    Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseInExpo);
                    float currentRotation = easeFunction(Orientation, FinalOrientation - Orientation,
                        Time.time - StartTime, BehaviorDuration);

                    agentBody.transform.eulerAngles = new Vector3(rotationX, currentRotation, rotationZ);
                    break;
                }
                case Configuration.Transitions.EaseInOut:
                {
                    float totalTime = BehaviorDuration / 2;

                    if (Time.time - StartTime <= totalTime)
                    {
                        Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseInCubic);
                        float distance = FinalOrientation - Orientation;
                        float timeElapsed = Time.time - StartTime;
                        float currentRotation = easeFunction(Orientation, distance, timeElapsed, totalTime);
                        agentBody.transform.eulerAngles = new Vector3(rotationX, currentRotation, rotationZ);

                        //Debug.Log("easing in: " + easeFunction(Orientation, distance, timeElapsed, totalTime));
                    }
                    else
                    {
                        Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseOutCubic);
                        float distance = -(FinalOrientation - Orientation);
                        float timeElapsed = Time.time - StartTime - totalTime;
                        float currentRotation = easeFunction(FinalOrientation, distance, timeElapsed, totalTime);
                        agentBody.transform.eulerAngles = new Vector3(rotationX, currentRotation, rotationZ);

                        //Debug.Log("easing out: " + easeFunction(FinalOrientation, distance, timeElapsed, totalTime));
                    }
                    break;
                }
            }

            if (Time.time - StartTime > BehaviorDuration)
            {
                IsOver = true;
                FinalizeEffects(agentBody);
                //Debug.Log("Behavior ended");
            }

        }

        public override void FinalizeEffects(Body body)
        {
            body.CurrentRotation = FinalOrientation;
        }

    }
}