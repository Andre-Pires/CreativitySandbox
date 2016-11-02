using Assets.Scripts.Classes.Agent;

namespace Assets.Scripts.Interface
{
    public interface IBehavior
    {
        void StartBehavior();
        void ApplyBehavior(Body body);
        void FinalizeEffects(Body body);
    }
}
