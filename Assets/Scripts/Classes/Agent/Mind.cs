using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Classes.Agent.Behaviors;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent
{
    public class Mind : MonoBehaviour
    {
        private Body _body;
        private List<Piece> _otherPieces;
        public Dictionary<Configuration.Behaviors, Behavior> AgentBehaviors;
        //user input
        private bool _userDisturbedPiece;
        //inercia driver - public in order to appear in unity editor
        public int InerciaDriver = Random.Range(0, 50);

        //behavior fields
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
                Debug.Log("user input");
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
                //fires at least one behavior
                int behaviorsToExecute = Random.Range(1, availableBehaviors.Count);

                while (behaviorsToExecute > 0)
                {
                    Behavior currentBehavior = availableBehaviors[Random.Range(0, availableBehaviors.Count)];
                    RunBehaviorAndNotify(currentBehavior);

                    behaviorsToExecute--;
                }

                //reset user input since the user handled the piece
                _userDisturbedPiece = false;
                Debug.Log("User driven behavior");
            }
        }

        private void CheckIfBehaviorsReady()
        {
            foreach (Behavior behavior in AgentBehaviors.Values)
            {
                //Debug.Log("type " + behavior.BehaviorType + ", at " + behavior.BehaviorDrive);
                if (behavior.IsOver && behavior.BehaviorDrive >= 90)
                {
                    RunBehaviorAndNotify(behavior);

                    Debug.Log("Inercia driven behavior");
                }
            }
        }

        private void RunBehaviorAndNotify(Behavior behavior)
        {
            switch (behavior.BehaviorType)
            {
                case Configuration.Behaviors.Blink:
                    (behavior as BlinkBehavior).PrepareBehavior(_body.Color, _behaviorDuration);
                    break;
                case Configuration.Behaviors.Resize:
                    (behavior as ResizeBehavior).PrepareBehavior(_body.Size, _behaviorDuration);
                    break;
                case Configuration.Behaviors.Rotate:
                    (behavior as RotationBehavior).PrepareBehavior(_body.CurrentRotation, _behaviorDuration);
                    break;
            }
            behavior.StartBehavior();

            Vector3 piecePosition = transform.position;
            foreach (Piece piece in _otherPieces)
            {
                piece.Mind.CheckReceivedStimulus(piecePosition, behavior);
            }

            //reset inercia driver
            behavior.BehaviorDrive = 0;

            Debug.Log("executed " + behavior.BehaviorType);
        }

        private void CheckReceivedStimulus(Vector3 stimilusPosition, Behavior activeBehavior)
        {
            Vector3 piecePosition = transform.localPosition;
            float sqrDistance = (stimilusPosition - piecePosition).sqrMagnitude;
            
            //filtering behavior to simulate needs at this point; only affecting inactive behaviors
            List<Behavior> affectedBehaviors = AgentBehaviors.Values.ToList().FindAll(b => (b.BehaviorType == activeBehavior.BehaviorType) && b.IsOver);

            Debug.Log("Check stimuli: piece being called : " + transform.name + ", behavior stimulus sent: " + activeBehavior.BehaviorType);

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

                        Debug.Log("inercia after stimulus " + behavior.BehaviorDrive + ", behavior: " + behavior.BehaviorType);
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