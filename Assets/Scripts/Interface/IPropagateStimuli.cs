using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using UnityEngine;

namespace Assets.Scripts.Interface
{
    public interface IPropagateStimuli
    {
        void ReceiveStimulus(Vector3 stimulusPosition, Behavior stimulatingBehavior);
        void SendStimulus(Behavior stimulatingBehavior);
    }
}
