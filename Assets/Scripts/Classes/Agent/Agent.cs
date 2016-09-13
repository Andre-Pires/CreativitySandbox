using System.Collections.Generic;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent
{
    public class Agent
    {
        private readonly List<InstanceAgentPiece> _newPartListeners;
        private readonly List<Piece> _pieces;

        public Agent()
        {
            _newPartListeners = new List<InstanceAgentPiece>();

            foreach (var agentPieceListener in Object.FindObjectsOfType<InstanceAgentPiece>())
            {
                _newPartListeners.Add(agentPieceListener);
                // Start the event listener
                agentPieceListener.OnSelect += AddNewComponent;
            }

            //provide option to clear agent configuration
            ClearAgentConfiguration.Instance.OnSelect += EraseCurrentAgent;
            CreateAgentPiece.Instance.OnSelect += AddBlankComponent;

            _pieces = new List<Piece>();
        }

        public void Update()
        {
            _pieces.ForEach(p => p.Update());
        }

        public void AddNewComponent(Configuration.Personality personality, Configuration.Size size)
        {
            _pieces.Add(new Piece("Piece_" + _pieces.Count, personality, size));
        }

        //create an initial component with random settings
        public void AddBlankComponent()
        {
            var numberOfPersonalities = Configuration.Instance.AvailablePersonalities.Count;
            var numberOfSizes = Configuration.Instance.SizeValues.Count;

            _pieces.Add(new Piece("Piece_" + _pieces.Count,
                Configuration.Instance.AvailablePersonalities[Random.Range(0, numberOfPersonalities)],
                Configuration.Instance.AvailableSizes[Random.Range(0, numberOfSizes)]));
        }

        public void EraseCurrentAgent()
        {
            _pieces.Clear();

            foreach (var agentPiece in GameObject.FindGameObjectsWithTag("Cube"))
            {
                Object.Destroy(agentPiece);
            }
        }

        public void OnDrawGizmos()
        {
            _pieces.ForEach(p => p.OnDrawGizmos());
        }

        public void OnGUI()
        {
            _pieces.ForEach(p => p.OnGUI());
        }
    }
}