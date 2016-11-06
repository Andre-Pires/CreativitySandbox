using Assets.Scripts.Classes.Agent;
using Assets.Scripts.Classes.Agent.ComposedBehaviors;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;

namespace Assets.Scripts.Interface
{
    public interface IComposedBehavior
    {
        void StartBehavior();
        void PrepareBehavior(Body body, Configuration.ActiveBehaviors behaviorToPrepare, float duration);
        void ApplyBehavior(Body body);
        void ReceiveStimuli(Configuration.ProxemicDistance stimuliDistance, ComposedBehavior stimulatingBehavior);
    }
}
