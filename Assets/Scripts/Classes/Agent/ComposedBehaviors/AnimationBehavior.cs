using UnityEngine;

namespace Assets.Scripts.Classes.Agent.ComposedBehaviors
{
    public class AnimationBehavior
    {
        private string _animationName;
        private string _animationSpeedMultiplier;
        private readonly string _animationTrigger;
        private readonly Animator _animator;
        public bool IsOver = true;
        private float _startTime;
        private readonly float _clipLength;

        public AnimationBehavior(Animator animator, string animationToPlay, string animationTrigger, string animationMultiplier)
        {
            _animator = animator;
            _animationName = animationToPlay;
            _animationTrigger = animationTrigger;
            _animationSpeedMultiplier = animationMultiplier;

            RuntimeAnimatorController ac = _animator.runtimeAnimatorController;    //Get Animator controller
            for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
            {
                if (ac.animationClips[i].name == _animationName)        //If it has the same name as your clip
                {
                    _clipLength = ac.animationClips[i].length / _animator.GetFloat(_animationSpeedMultiplier);
                    break;
                }
            }

            _startTime = Time.time;
            IsOver = false;
        }

        public void StartBehavior()
        {
            _startTime = Time.time;
            IsOver = false;

            if (_animator != null)
            {
                _animator.SetTrigger(_animationTrigger);
                Debug.Log("Playing " + _animationName + " for " + _clipLength + " seconds.");
            }
        }

        public void ApplyBehavior(Body agentBody)
        {
            Vector3 hitNormal;
            if (agentBody.IsColliding(out hitNormal))
            {
                agentBody.transform.position += hitNormal.normalized * 0.1f;
            }

            //when the animation is over we pause before changing color
            if ((Time.time - _startTime) > _clipLength)
            {
                IsOver = true;
            }
        }
    }
}