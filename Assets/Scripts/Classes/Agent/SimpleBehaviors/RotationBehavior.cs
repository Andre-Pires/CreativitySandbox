using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.SimpleBehaviors
{
    public class RotationBehavior : Behavior
    {
        public Configuration.Transitions RotateTransition;
        public float Orientation;
        public float FinalOrientation;
        public float RotationAmount;
        public float AnimationEndPause;
        public Configuration.RotationDirection RotationDirection;


        public RotationBehavior(float multiplier, bool behaviorDriveActive = true) : base(multiplier, behaviorDriveActive)
        {
            BehaviorType = Configuration.Behaviors.Rotate;
        }

        //this function randomizes the Behavior
        public override void PrepareBehavior(Body body, int repetitions, float duration)
        {
            //random direction doesn't pause when changing direction
            AnimationEndPause = 0;
            KeepBehaviorSetting = false;
            var transitionsCount = Configuration.Instance.AvailableTransitions.Count;

            //rotation Behavior
            Configuration.Transitions rotTransition =
                Configuration.Instance.AvailableTransitions[Random.Range(0, transitionsCount)];

            RotationDirection = Configuration.RotationDirection.Random;
            RotationAmount = Random.Range(-540.0f, 540.0f);
            float randomDirection = Random.Range(0, 100) > 50 ? 1 : -1;
            switch (RotationDirection)
            {
                case Configuration.RotationDirection.Left:
                    RotationAmount *= -1.0f;
                    break;
                case Configuration.RotationDirection.Right:
                    RotationAmount *= 1.0f;
                    break;
                case Configuration.RotationDirection.Alternating:
                    RotationAmount *= randomDirection;
                    break;
                case Configuration.RotationDirection.Random:
                    RotationAmount *= randomDirection;
                    break;
            }

            Orientation = body.CurrentRotation;
            RotateTransition = rotTransition;
            FinalOrientation = Orientation + RotationAmount;

            if (rotTransition == Configuration.Transitions.Instant)
            {
                BehaviorDuration = 0.0f;
            }
            else
            {
                BehaviorDuration = duration;
            }
            
            MaxBehaviorRepetitions = repetitions;
            CurrentBehaviorRepetition = 1;
            AnimationIntervalTime = BehaviorDuration / MaxBehaviorRepetitions;
        }

        //this function allows to customize the Behavior in the mind
        public void PrepareBehavior(Body body, float rotationAmount, Configuration.RotationDirection direction, Configuration.Transitions rotationTransition,
            int repetitions, float duration, float changeDirectionPause = 0, bool keepBehaviorParameter = false)
        {
            AnimationEndPause = changeDirectionPause;
            KeepBehaviorSetting = keepBehaviorParameter;

            RotationAmount = rotationAmount;
            RotationDirection = direction;
            float randomDirection = Random.Range(0, 100) > 50 ? 1 : -1;
            switch (RotationDirection)
            {
                case Configuration.RotationDirection.Left:
                    RotationAmount *= -1.0f;
                    break;
                case Configuration.RotationDirection.Right:
                    RotationAmount *= 1.0f;
                    break;
                case Configuration.RotationDirection.Alternating:
                    RotationAmount *= randomDirection;
                    break;
                case Configuration.RotationDirection.Random:
                    RotationAmount *= randomDirection;
                    break;
            }

            Orientation = body.CurrentRotation;
            RotateTransition = rotationTransition;
            FinalOrientation = Orientation + RotationAmount;


            if (rotationTransition == Configuration.Transitions.Instant)
            {
                BehaviorDuration = 0.0f;
            }
            else
            {
                BehaviorDuration = duration;
            }

            if (direction == Configuration.RotationDirection.Alternating)
            {
                MaxBehaviorRepetitions = repetitions + 1;
            }
            else
            {
                MaxBehaviorRepetitions = repetitions;
            }
            CurrentBehaviorRepetition = 1;
            AnimationIntervalTime = BehaviorDuration / MaxBehaviorRepetitions;
        }


        public override void ApplyBehavior(Body agentBody)
        {
            float rotationX = agentBody.transform.rotation.eulerAngles.x;
            float rotationZ = agentBody.transform.rotation.eulerAngles.z;
            float currentRotation = 0;

            //when the animation is over we pause before changing direction
            if ((Time.time - StartTime) > AnimationIntervalTime && (Time.time - StartTime) < (AnimationIntervalTime + AnimationEndPause)
                && CurrentBehaviorRepetition != MaxBehaviorRepetitions)
            {
                return;
            }

            switch (RotateTransition)
            {
                case Configuration.Transitions.Linear:
                    currentRotation = Mathf.Lerp(Orientation, FinalOrientation - Orientation, 1 - ((Time.time - StartTime) / AnimationIntervalTime));
                    break;
                case Configuration.Transitions.Instant:
                {
                    currentRotation = FinalOrientation;
                    break;
                }
                case Configuration.Transitions.EaseIn:
                {
                    Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseInQuint);
                    currentRotation = easeFunction(Orientation, FinalOrientation - Orientation,
                        Time.time - StartTime, AnimationIntervalTime);
                    break;
                }
                case Configuration.Transitions.EaseOut:
                {
                    Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseOutQuint);
                    currentRotation = easeFunction(Orientation, FinalOrientation - Orientation,
                        Time.time - StartTime, AnimationIntervalTime);
                    break;
                }
                case Configuration.Transitions.EaseInOut:
                {
                    float totalTime = AnimationIntervalTime / 2;

                    if (Time.time - StartTime <= totalTime)
                    {
                        Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseInQuint);
                        float distance = FinalOrientation - Orientation;
                        float timeElapsed = Time.time - StartTime;
                        currentRotation = easeFunction(Orientation, distance, timeElapsed, totalTime);
                        //Debug.Log("easing in: " + easeFunction(Orientation, distance, timeElapsed, totalTime));
                    }
                    else
                    {
                        Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseOutQuint);
                        float distance = -(FinalOrientation - Orientation);
                        float timeElapsed = Time.time - StartTime - totalTime;
                        currentRotation = easeFunction(FinalOrientation, distance, timeElapsed, totalTime);
                        //Debug.Log("easing out: " + easeFunction(FinalOrientation, distance, timeElapsed, totalTime));
                    }
                    break;
                }
            }

            Vector3 calculatedRotation = new Vector3(rotationX, currentRotation, rotationZ);
            agentBody.transform.eulerAngles = calculatedRotation;

            Vector3 hitNormal;
            if (agentBody.IsColliding(out hitNormal))
            {
                agentBody.transform.position += hitNormal.normalized * 0.1f;
            }


            if ((Time.time - StartTime)  > AnimationIntervalTime && CurrentBehaviorRepetition == MaxBehaviorRepetitions)
            {
                IsOver = true;
                FinalizeEffects(agentBody);
                //Debug.Log("Behavior ended. Final or: " + FinalOrientation + ", previous or: " + Orientation);
                return;
            }
            else if ((Time.time - StartTime) > (AnimationIntervalTime + AnimationEndPause) && CurrentBehaviorRepetition != MaxBehaviorRepetitions)
            {
                //if rotation alternates always invert the previous orientation
                if (RotationDirection == Configuration.RotationDirection.Alternating)
                {
                    if (CurrentBehaviorRepetition + 1 == MaxBehaviorRepetitions)
                    {
                        Orientation = FinalOrientation;
                        FinalOrientation = agentBody.CurrentRotation;
                    }
                    else
                    {
                        float segmentFinalOrientation = FinalOrientation;
                        RotationAmount *= -1.0f;
                        FinalOrientation = agentBody.CurrentRotation + RotationAmount;
                        Orientation = segmentFinalOrientation;
                    }

                }
                else
                {
                    Orientation = FinalOrientation;
                    FinalOrientation = Orientation + RotationAmount;
                }

                CurrentBehaviorRepetition++;
                StartTime = Time.time;
            }
        }

        public override void FinalizeEffects(Body body)
        {

            if (KeepBehaviorSetting)
            {
                body.CurrentRotation = RotationAmount;
            }
            else
            {
                body.CurrentRotation = FinalOrientation;
            }
        }
    }
}