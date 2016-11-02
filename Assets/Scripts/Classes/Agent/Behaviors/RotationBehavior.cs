using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.Behaviors
{
    public class RotationBehavior : Behavior
    {
        public Configuration.Transitions RotateTransition;
        public float Orientation;
        public float FinalOrientation;

        public RotationBehavior(float multiplier) : base(multiplier)
        {
            BehaviorType = Configuration.Behaviors.Rotate;
        }

        //this function randomizes the behavior
        public void PrepareBehavior(float currentRotation, float duration)
        {
            var transitionsCount = Configuration.Instance.AvailableTransitions.Count;

            //rotation behavior
            Configuration.Transitions rotTransition =
                Configuration.Instance.AvailableTransitions[Random.Range(0, transitionsCount)];
            float rotationAmount = Random.Range(0.0f, 720.0f);

            Orientation = currentRotation;
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

        //this function allows to customize the behavior in the mind
        public void PrepareBehavior(float currentRotation, float rotationAmount, Configuration.Transitions rotationTransition, float duration)
        {
            Orientation = currentRotation;
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

            if (RotateTransition == Configuration.Transitions.Linear)
            {
                var lerp = (Time.time - StartTime)/BehaviorDuration;
                agentBody.transform.rotation = Quaternion.Slerp(agentBody.transform.rotation, Quaternion.Euler(0, FinalOrientation, 0),  lerp);
            }
            else if (RotateTransition == Configuration.Transitions.Instant)
            {
                agentBody.transform.eulerAngles = new Vector3(rotationX, FinalOrientation, rotationZ);
            }
            else if (RotateTransition == Configuration.Transitions.EaseIn)
            {
                Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseInExpo);
                float currentRotation = easeFunction(Orientation, FinalOrientation - Orientation,Time.time - StartTime, BehaviorDuration);

                agentBody.transform.eulerAngles = new Vector3(rotationX, currentRotation, rotationZ);
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