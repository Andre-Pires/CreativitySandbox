using System.Collections.Generic;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent
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

        public void AddNewComponent(Configuration.Personality personality, Configuration.Size size)
        {
            _pieces.Add(new Piece("Piece" + _pieces.Count, personality, size));
        }

        public void OnDrawGizmos()
        {
            _pieces.ForEach(p => p.OnDrawGizmos());
        }
    }
}