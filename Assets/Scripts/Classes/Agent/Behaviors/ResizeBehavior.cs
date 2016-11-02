using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent.Behaviors
{
    public class ResizeBehavior : Behavior
    {
        public Configuration.Transitions SizeTransition;
        public Configuration.Size Size;
        public Configuration.Size FinalSize;

        public ResizeBehavior(float multiplier) : base(multiplier)
        {
            BehaviorType = Configuration.Behaviors.Resize;
        }

        //this function randomizes the behavior
        public void PrepareBehavior(Configuration.Size currentSize, float duration)
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

                if (finalSize != currentSize)
                {
                    break;
                }
            }

            //currentSize behavior
            Size = currentSize;
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

        //this function allows to customize the behavior in the mind
        public void PrepareBehavior(Configuration.Size currentSize, Configuration.Size finalSize, Configuration.Transitions sizeTransition, float duration)
        {
            Size = currentSize;
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

            if (SizeTransition == Configuration.Transitions.Linear)
            {
                var lerp = (Time.time - StartTime)/BehaviorDuration;
                agentBody.transform.localScale = Vector3.one * (Mathf.Lerp(currentSize, finalSize, lerp));
                agentBody.transform.localPosition = new Vector3(agentBody.transform.localPosition.x,
                    agentBody.transform.GetComponent<Renderer>().bounds.extents.y, agentBody.transform.localPosition.z);
            }
            else if (SizeTransition == Configuration.Transitions.Instant)
            {
                agentBody.Size = FinalSize;
            }
            else if ( SizeTransition == Configuration.Transitions.EaseIn)
            {
                Interpolate.Function easeFunction = Interpolate.Ease(Interpolate.EaseType.EaseInExpo);
                agentBody.transform.localScale = Vector3.one * easeFunction(currentSize, finalSize - currentSize, Time.time - StartTime, BehaviorDuration);
                agentBody.transform.localPosition = new Vector3(agentBody.transform.localPosition.x,
                    agentBody.transform.GetComponent<Renderer>().bounds.extents.y, agentBody.transform.localPosition.z);
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
            body.Size = FinalSize;
        }

    }
}