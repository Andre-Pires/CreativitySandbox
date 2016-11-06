using Assets.Scripts.Classes.Agent.ComposedBehaviors;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using UnityEngine;

namespace Assets.Scripts.Interface
{
    public interface IPropagateStimuli
    {
        void ReceiveStimulus(Vector3 stimulusPosition, ComposedBehavior stimulatingBehavior);
        void SendStimulus(ComposedBehavior stimulatingBehavior);
    }
}
