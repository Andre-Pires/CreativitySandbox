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
        public override void PrepareBehavior(Body body, float duration)
        {
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

        }

        //this function allows to customize the Behavior in the mind
        public void PrepareBehavior(Body body, Configuration.Size finalSize, Configuration.Transitions sizeTransition, float duration)
        {
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
        }

        public override void ApplyBehavior(Body agentBody)
        {
            float currentSize = Configuration.Instance.SizeValues[Size];
            float finalSize = Configuration.Instance.SizeValues[FinalSize];

            switch (SizeTransition)
            {
                case Configuration.Transitions.Linear:
                    var lerp = (Time.time - StartTime)/BehaviorDuration;
                    agentBody.transform.localScale = Vector3.one * (Mathf.Lerp(currentSize, finalSize, lerp));
                    agentBody.transform.localPosition = new Vector3(agentBody.transform.localPosition.x,
                        agentBody.transform.GetComponent<Renderer>().bounds.extents.y, agentBody.transform.localPosition.z);
                    break;
                case Configuration.Transitions.Instant:
                    agentBody.Size = FinalSize;
                    break;
                case Configuration.Transitions.EaseIn:
                {
                    Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseInExpo);
                    agentBody.transform.localScale = Vector3.one * easeFunction(currentSize, finalSize - currentSize, Time.time - StartTime, BehaviorDuration);
                    agentBody.transform.localPosition = new Vector3(agentBody.transform.localPosition.x,
                        agentBody.transform.GetComponent<Renderer>().bounds.extents.y, agentBody.transform.localPosition.z);
                }
                    break;
                case Configuration.Transitions.EaseInOut:
                {
                    float totalTime = BehaviorDuration / 2;

                    if (Time.time - StartTime <= totalTime)
                    {
                        Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseInCubic);
                        float distance = finalSize - currentSize;
                        float timeElapsed = Time.time - StartTime;
                        agentBody.transform.localScale = Vector3.one * easeFunction(currentSize, distance, timeElapsed, totalTime);
                        agentBody.transform.localPosition = new Vector3(agentBody.transform.localPosition.x,
                            agentBody.transform.GetComponent<Renderer>().bounds.extents.y, agentBody.transform.localPosition.z);

                        //Debug.Log("easing in: " + easeFunction(currentSize, distance, timeElapsed, totalTime));
                    }
                    else
                    {
                        Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseOutCubic);
                        float distance = -(finalSize - currentSize);
                        float timeElapsed = Time.time - StartTime - totalTime;
                        agentBody.transform.localScale = Vector3.one * easeFunction(finalSize, distance, timeElapsed, totalTime);
                        agentBody.transform.localPosition = new Vector3(agentBody.transform.localPosition.x,
                            agentBody.transform.GetComponent<Renderer>().bounds.extents.y, agentBody.transform.localPosition.z);

                        //Debug.Log("easing out: " + easeFunction(finalSize, distance, timeElapsed, totalTime));
                    }

                }
                    break;
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
            //by easing out we are leaving the original color in place
            if (SizeTransition == Configuration.Transitions.EaseInOut)
            {
                body.Size = Size;
            }
            else
            {
                body.Size = FinalSize;
            }
        }

    }
}