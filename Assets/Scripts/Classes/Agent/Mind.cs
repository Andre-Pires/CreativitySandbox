using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Classes.Agent.ComposedBehaviors;
using Assets.Scripts.Classes.Agent.SimpleBehaviors;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Interface;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent
{
    public class Mind : MonoBehaviour, IPropagateStimuli
    {
        private bool _mindHalted;
        public bool MindHalted
        {
            set
            {
                _mindHalted = value;
                AgentBehaviors.Values.ToList().ForEach(behavior => behavior.BehaviorHalted = value);
            }

            get { return _mindHalted; }
        }
        private Body _body;
        private List<Piece> _otherPieces;
        public Dictionary<Configuration.ComposedBehaviors, ComposedBehavior> AgentBehaviors;
        //user input
        private bool _userDisturbedPiece;

        //Behavior fields
        private float _behaviorDuration;
        public float IntimateRadius = 10.0f;
        public float PersonalRadius = 17.0f;
        public float SocialRadius = 25.0f;

        public void InitializeParameters(Body body, Configuration.Personality personality, List<Piece> otherPieces)
        {
            AgentBehaviors = new Dictionary<Configuration.ComposedBehaviors, ComposedBehavior>();

            switch (personality)
            {
                case Configuration.Personality.Joy:
                    AgentBehaviors.Add(Configuration.ComposedBehaviors.Joy, new JoyBehavior(Random.Range(1.0f, 1.5f), 0));
                    break;
                case Configuration.Personality.Fear:
                    AgentBehaviors.Add(Configuration.ComposedBehaviors.Fear, new FearBehavior(Random.Range(1.0f, 1.5f), 0));
                    break;
                case Configuration.Personality.Anger:
                    AgentBehaviors.Add(Configuration.ComposedBehaviors.Anger, new AngerBehavior(Random.Range(1.0f, 1.5f), 0));
                    break;
                case Configuration.Personality.Disgust:
                    AgentBehaviors.Add(Configuration.ComposedBehaviors.Disgust, new DisgustBehavior(Random.Range(1.0f, 1.5f), 0));
                    break;
                case Configuration.Personality.Sadness:
                    AgentBehaviors.Add(Configuration.ComposedBehaviors.Sadness, new SadnessBehavior(Random.Range(1.0f, 1.5f), 0));
                    break;
                default: //TODO for now default launches one behavior
                    AgentBehaviors.Add(Configuration.ComposedBehaviors.Sadness, new SadnessBehavior(Random.Range(1.0f, 1.5f), 0));
                    break;
            }

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
            if (MindHalted)
            {
                return;
            }

            //used in the behaviors triggered this frame
            _behaviorDuration = Random.Range(1.0f, 2.5f);

            //CheckForUserInput();

            CheckIfBehaviorsReady();

            //TODO - remove - only for debugging, fires the excited behavior
            if (Input.GetKeyDown(KeyCode.F))
            {
                AgentBehaviors.ToList().FirstOrDefault().Value.ReceiveStimuli(Configuration.ProxemicDistance.Personal, AgentBehaviors.ToList().FirstOrDefault().Value);
            }
        }


        //NOTE: unused at the moment
        private void CheckForUserInput()
        {
            if (_userDisturbedPiece)
            {
                List<ComposedBehavior> availableBehaviors = AgentBehaviors.Values.ToList().FindAll(b => b.IsOver == true);

                //fires at least one Behavior
                int behaviorsToExecute = Random.Range(1, availableBehaviors.Count);

                while (behaviorsToExecute > 0 && availableBehaviors.Count > 0)
                {
                    ComposedBehavior currentBehavior = availableBehaviors[Random.Range(0, availableBehaviors.Count)];
                    RunBehaviorAndNotify(currentBehavior, Configuration.ActiveBehaviors.StandardBehavior);

                    behaviorsToExecute--;
                }

                //reset user input since the user handled the piece
                _userDisturbedPiece = false;
                //Debug.Log("User driven Behavior");
            }
        }

        private void CheckIfBehaviorsReady()
        {
            //first check the excited behaviors, displayed when two pieces with the same behavior interact
            foreach (ComposedBehavior behavior in AgentBehaviors.Values)
            {
                //Debug.Log("behavior type " + behavior.BehaviorType + ", at " + behavior.StandardBehaviorDrive);

                if (behavior.IsOver && behavior.ExcitedBehaviorDrive >= 90)
                {
                    RunBehaviorAndNotify(behavior, Configuration.ActiveBehaviors.ExcitedBehavior);

                    //Debug.Log("Inercia driven Behavior");
                    return;
                }
            }

            //then the standard behaviors, displayed in every other interaction
            foreach (ComposedBehavior behavior in AgentBehaviors.Values)
            {
                //Debug.Log("behavior type " + behavior.BehaviorType + ", at " + behavior.StandardBehaviorDrive);

                if (behavior.IsOver && behavior.StandardBehaviorDrive >= 90)
                {
                    RunBehaviorAndNotify(behavior, Configuration.ActiveBehaviors.StandardBehavior);

                    //Debug.Log("Inercia driven Behavior");
                }
            }
        }

        private void RunBehaviorAndNotify(ComposedBehavior behavior, Configuration.ActiveBehaviors behaviorToPrepare)
        {
            behavior.PrepareBehavior(_body, behaviorToPrepare, _behaviorDuration);
             
            behavior.StartBehavior();

            SendStimulus(behavior);

            Debug.Log("executed " + behavior.BehaviorType + " for " + _behaviorDuration + " seconds");
        }

        public void SendStimulus(ComposedBehavior stimulatingBehavior)
        {
            Vector3 piecePosition = transform.position;
            foreach (Piece piece in _otherPieces.FindAll(p => p.IsPieceVisible))
            {
                piece.Mind.ReceiveStimulus(piecePosition, stimulatingBehavior);
            }
        }

        public void ReceiveStimulus(Vector3 stimilusPosition, ComposedBehavior stimulatingBehavior)
        {
            Vector3 piecePosition = transform.localPosition;
            float sqrDistance = (stimilusPosition - piecePosition).sqrMagnitude;
            
            //filtering Behavior to simulate needs at this point; only affecting inactive behaviors
            List<ComposedBehavior> affectedBehaviors = AgentBehaviors.Values.ToList().FindAll(b => b.IsOver);

            Debug.Log("Check stimuli: piece being called : " + transform.name + ", Behavior stimulus sent: " + stimulatingBehavior.BehaviorType);

            foreach (ComposedBehavior behavior in affectedBehaviors)
            {
                if (sqrDistance < SocialRadius * SocialRadius)
                {
                    if (behavior.StandardBehaviorDrive < 100)
                    {
                        if (sqrDistance < IntimateRadius * IntimateRadius)
                        {
                            behavior.ReceiveStimuli(Configuration.ProxemicDistance.Intimate, stimulatingBehavior);
                        }
                        else if(sqrDistance < PersonalRadius * PersonalRadius)
                        {
                            behavior.ReceiveStimuli(Configuration.ProxemicDistance.Personal, stimulatingBehavior);
                        }
                        else if (sqrDistance < SocialRadius * SocialRadius)
                        {
                            behavior.ReceiveStimuli(Configuration.ProxemicDistance.Social, stimulatingBehavior);
                        }

                        //Debug.Log("inercia after stimulus " + Behavior.StandardBehaviorDrive + ", ComplexBehavior: " + ComposedBehavior.BehaviorType);
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