using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Interface;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent
{
    public class Mind : MonoBehaviour, IPropagateStimuli
    {
        private Body _body;
        private List<Piece> _otherPieces;
        public Dictionary<Configuration.Behaviors, Behavior> AgentBehaviors;
        //user input
        private bool _userDisturbedPiece;

        //Behavior fields
        private float _behaviorDuration;
        public float IntimateRadius = 10.0f;
        public float PersonalRadius = 17.0f;
        public float SocialRadius = 25.0f;

        public void InitializeParameters(Body body, List<Piece> otherPieces)
        {
            AgentBehaviors = new Dictionary<Configuration.Behaviors, Behavior>();
            AgentBehaviors.Add(Configuration.Behaviors.Blink, new BlinkBehavior(Random.Range(1.0f, 2.5f)));
            AgentBehaviors.Add(Configuration.Behaviors.Resize, new ResizeBehavior(Random.Range(1.0f, 2.5f)));
            AgentBehaviors.Add(Configuration.Behaviors.Rotate, new RotationBehavior(Random.Range(1.0f, 2.5f)));

            _body = body;
            _body.AgentBehaviors = AgentBehaviors; 
            _body.NotifyAgentMind += () =>
            {
                _userDisturbedPiece = true;
                //Debug.Log("user input");
            };

            _otherPieces = new List<Piece>();
            otherPieces.ForEach(StoreAgentPiece);
        }

        public void Update()
        {
            //used in the behaviors triggered this frame
            _behaviorDuration = Random.Range(1.0f, 2.5f);

            CheckForUserInput();

            CheckIfBehaviorsReady();
        }

        private void CheckForUserInput()
        {
            if (_userDisturbedPiece)
            {
                List<Behavior> availableBehaviors = AgentBehaviors.Values.ToList().FindAll(b => b.IsOver == true);
                //fires at least one Behavior
                int behaviorsToExecute = Random.Range(1, availableBehaviors.Count);

                while (behaviorsToExecute > 0)
                {
                    Behavior currentBehavior = availableBehaviors[Random.Range(0, availableBehaviors.Count)];
                    RunBehaviorAndNotify(currentBehavior);

                    behaviorsToExecute--;
                }

                //reset user input since the user handled the piece
                _userDisturbedPiece = false;
                //Debug.Log("User driven Behavior");
            }
        }

        private void CheckIfBehaviorsReady()
        {
            foreach (Behavior behavior in AgentBehaviors.Values)
            {
                //Debug.Log("type " + Behavior.BehaviorType + ", at " + ComplexBehavior.BehaviorDrive);
                if (behavior.IsOver && behavior.BehaviorDrive >= 90)
                {
                    RunBehaviorAndNotify(behavior);

                    //Debug.Log("Inercia driven Behavior");
                }
            }
        }

        private void RunBehaviorAndNotify(Behavior behavior)
        {
            //TODO: it can now be simplified
            switch (behavior.BehaviorType)
            {
                case Configuration.Behaviors.Blink:
                    (behavior as BlinkBehavior).PrepareBehavior(_body, _behaviorDuration);
                    break;
                case Configuration.Behaviors.Resize:
                    (behavior as ResizeBehavior).PrepareBehavior(_body, _behaviorDuration);
                    break;
                case Configuration.Behaviors.Rotate:
                    (behavior as RotationBehavior).PrepareBehavior(_body, _behaviorDuration);
                    break;
            }
            behavior.StartBehavior();

            SendStimulus(behavior);

            //reset inercia driver
            behavior.BehaviorDrive = 0;

            //Debug.Log("executed " + Behavior.BehaviorType);
        }

        public void SendStimulus(Behavior stimulatingBehavior)
        {
            Vector3 piecePosition = transform.position;
            foreach (Piece piece in _otherPieces)
            {
                piece.Mind.ReceiveStimulus(piecePosition, stimulatingBehavior);
            }
        }

        public void ReceiveStimulus(Vector3 stimilusPosition, Behavior stimulatingBehavior)
        {
            Vector3 piecePosition = transform.localPosition;
            float sqrDistance = (stimilusPosition - piecePosition).sqrMagnitude;
            
            //filtering Behavior to simulate needs at this point; only affecting inactive behaviors
            List<Behavior> affectedBehaviors = AgentBehaviors.Values.ToList().FindAll(b => (b.BehaviorType == stimulatingBehavior.BehaviorType) && b.IsOver);

            //Debug.Log("Check stimuli: piece being called : " + transform.name + ", Behavior stimulus sent: " + stimulatingBehavior.BehaviorType);

            foreach (Behavior behavior in affectedBehaviors)
            {
                if (sqrDistance < SocialRadius * SocialRadius)
                {
                    if (behavior.BehaviorDrive < 100)
                    {
                        if (sqrDistance < IntimateRadius * IntimateRadius)
                        {
                            behavior.BehaviorDrive += 45;
                        }
                        else if(sqrDistance < PersonalRadius * PersonalRadius)
                        {
                            behavior.BehaviorDrive += 35;
                        }
                        else if (sqrDistance < SocialRadius * SocialRadius)
                        {
                            behavior.BehaviorDrive += 25;
                        }

                        if (behavior.BehaviorDrive > 100)
                        {
                            behavior.BehaviorDrive = 100;
                        }

                        //Debug.Log("inercia after stimulus " + Behavior.BehaviorDrive + ", ComplexBehavior: " + ComposedBehavior.BehaviorType);
                    }
                }
            }
        }

        public void RemoveStoreAgentPiece(Piece piece)
        {
            _otherPieces.Remove(piece);
        }

        public void StoreAgentPiece(Piece piece)
        {
            _otherPieces.Add(piece);
        }   

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(transform.position, IntimateRadius);

            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(transform.position, PersonalRadius);

            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(transform.position, SocialRadius);

        }
    }
}