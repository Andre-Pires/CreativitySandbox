using Assets.Scripts.Classes.Agent;

namespace Assets.Scripts.Interface
{
    public interface IBehavior
    {
        void StartBehavior();
        void PrepareBehavior(Body body, int repetitions, float duration);
        void ApplyBehavior(Body body);
    }
}
