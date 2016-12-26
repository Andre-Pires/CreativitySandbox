using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.SimpleBehaviors
{
    public class ResizeBehavior : Behavior
    {
        public Configuration.Transitions SizeTransition;
        public Configuration.Size Size;
        public Configuration.Size FinalSize;

        public ResizeBehavior(float multiplier, bool behaviorDriveActive = true) : base(multiplier, behaviorDriveActive)
        {
            BehaviorType = Configuration.Behaviors.Resize;
        }

        //this function randomizes the Behavior
        public override void PrepareBehavior(Body body, int repetitions, float duration)
        {
            KeepBehaviorSetting = false;
            var transitionsCount = Configuration.Instance.AvailableTransitions.Count;
            var sizesCount = Configuration.Instance.AvailableSizes.Count;

            Configuration.Transitions sizeTransition =
                Configuration.Instance.AvailableTransitions[Random.Range(0, transitionsCount)];
            Configuration.Size finalSize;

            //ensuring that the transition is to a different value
            while (true)
            {
                finalSize = Configuration.Instance.AvailableSizes[Random.Range(0, sizesCount)];

                if (finalSize != body.Size)
                {
                    break;
                }
            }

            //currentSize Behavior
            Size = body.Size;
            FinalSize = finalSize;
            SizeTransition = sizeTransition;

            if (sizeTransition == Configuration.Transitions.Instant)
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
        public void PrepareBehavior(Body body, Configuration.Size finalSize, Configuration.Transitions sizeTransition,
            int repetitions, float duration, bool keepBehaviorSetting = false)
        {
            KeepBehaviorSetting = keepBehaviorSetting;
            Size = body.Size;
            FinalSize = finalSize;
            SizeTransition = sizeTransition;

            if (sizeTransition == Configuration.Transitions.Instant)
            {
                BehaviorDuration = 0.0f;
            }
            else
            {
                BehaviorDuration = duration;
            }

            if (keepBehaviorSetting)
            {
                MaxBehaviorRepetitions = repetitions +1;
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
            float currentSize = Configuration.Instance.SizeValues[Size];
            float finalSize = Configuration.Instance.SizeValues[FinalSize];
            Renderer renderer = agentBody.Mesh.GetComponent<Renderer>();


            switch (SizeTransition)
            {
                case Configuration.Transitions.Linear:
                    var lerp = (Time.time - StartTime)/AnimationIntervalTime;
                    agentBody.transform.localScale = Vector3.one * (Mathf.Lerp(currentSize, finalSize, lerp));
                    agentBody.transform.localPosition = new Vector3(agentBody.transform.localPosition.x,
                        0/*renderer.bounds.extents.y*/, agentBody.transform.localPosition.z);
                    break;
                case Configuration.Transitions.Instant:
                    agentBody.Size = FinalSize;
                    break;
                case Configuration.Transitions.EaseIn:
                {
                    Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseInSine);
                    agentBody.transform.localScale = Vector3.one*
                                                     easeFunction(currentSize, finalSize - currentSize,
                                                         Time.time - StartTime, AnimationIntervalTime);
                    agentBody.transform.localPosition = new Vector3(agentBody.transform.localPosition.x,
                        0/*renderer.bounds.extents.y*/,
                        agentBody.transform.localPosition.z);
                    break;
                }
                case Configuration.Transitions.EaseOut:
                {
                    Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseOutSine);
                    agentBody.transform.localScale = Vector3.one *
                                                        easeFunction(currentSize, finalSize - currentSize,
                                                            Time.time - StartTime, AnimationIntervalTime);
                    agentBody.transform.localPosition = new Vector3(agentBody.transform.localPosition.x,
                        0/*renderer.bounds.extents.y*/,
                        agentBody.transform.localPosition.z);
                    break;
                }
                case Configuration.Transitions.EaseInOut:
                {
                    float totalTime = AnimationIntervalTime / 2;

                    if (Time.time - StartTime <= totalTime)
                    {
                        Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseInSine);
                        float distance = finalSize - currentSize;
                        float timeElapsed = Time.time - StartTime;
                        agentBody.transform.localScale = Vector3.one * easeFunction(currentSize, distance, timeElapsed, totalTime);
                        agentBody.transform.localPosition = new Vector3(agentBody.transform.localPosition.x,
                            0/*renderer.bounds.extents.y*/, agentBody.transform.localPosition.z);

                        //Debug.Log("easing in: " + easeFunction(currentSize, distance, timeElapsed, totalTime));
                    }
                    else
                    {
                        Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseOutSine);
                        float distance = -(finalSize - currentSize);
                        float timeElapsed = Time.time - StartTime - totalTime;
                        agentBody.transform.localScale = Vector3.one * easeFunction(finalSize, distance, timeElapsed, totalTime);
                        agentBody.transform.localPosition = new Vector3(agentBody.transform.localPosition.x,
                            0/*renderer.bounds.extents.y*/, agentBody.transform.localPosition.z);

                        //Debug.Log("easing out: " + easeFunction(finalSize, distance, timeElapsed, totalTime));
                    }

                }
                    break;
            }

            Vector3 hitNormal;
            if (agentBody.IsColliding(out hitNormal))
            {
                agentBody.transform.position += hitNormal.normalized*0.1f;
            }

            if ((Time.time - StartTime) > AnimationIntervalTime)
            {
                if (CurrentBehaviorRepetition + 1 == MaxBehaviorRepetitions && KeepBehaviorSetting)
                {
                    SizeTransition = Configuration.Transitions.EaseIn;
                }
                else if (CurrentBehaviorRepetition == MaxBehaviorRepetitions)
                {
                    IsOver = true;
                    FinalizeEffects(agentBody);
                    //Debug.Log("Behavior ended");
                    return;
                }
                CurrentBehaviorRepetition++;
                StartTime = Time.time;
            }
        }

        public override void FinalizeEffects(Body body)
        {
            //by easing out we are leaving the original color in place
            if (KeepBehaviorSetting)
            {
                body.Size = FinalSize;
            }
            else
            {
                body.Size = Size;
            }
        }

    }
}