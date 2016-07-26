using System.Collections.Generic;
using Assets.Scripts.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Classes
{

    public class Agent
    {
        private readonly List<Piece> _pieces;
        private readonly List<CreateAgentPiece> _newPartListeners;

        public Agent()
        {
            _newPartListeners = new List<CreateAgentPiece>();
            
            foreach(CreateAgentPiece agentPieceListener in GameObject.FindObjectsOfType<CreateAgentPiece>())
            {
                _newPartListeners.Add(agentPieceListener);
                // Start the event listener
                agentPieceListener.OnSelect += AddNewComponent;
            }
            
            _pieces = new List<Piece>();
        }

        public void Update()
        {
            _pieces.ForEach(p => p.Update());
        }

        public void AddNewComponent(Configuration.Personality personality)
        {
            Debug.Log("Pers " + personality);

            //TODO ao adicionar aqui o tipo de peça que é para criar ver se está available
            _pieces.Add(new Piece("Piece" + _pieces.Count));
        }

        public void OnDrawGizmos()
        {
            _pieces.ForEach(p => p.OnDrawGizmos());
        }
    }
}