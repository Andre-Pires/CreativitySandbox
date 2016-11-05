using Assets.Scripts.Classes.Agent;

namespace Assets.Scripts.Interface
{
    public interface IBehavior
    {
        void StartBehavior();
        void PrepareBehavior(Body body, float duration);
        void ApplyBehavior(Body body);
        void FinalizeEffects(Body body);
    }
}
