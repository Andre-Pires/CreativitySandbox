using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.UI;
using Assets.Scripts.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Classes.Agent
{
    public class Agent
    {
        private readonly List<InstanceAgentPiece> _newPartListeners;
        private readonly Dictionary<string, Piece> _pieces;

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
            CreateAgentPiece.Instance.OnSelect += AddBlankComponent;
            UIManager.Instance.DestroyAgentPiece += EraseAgentPiece;

            _pieces = new Dictionary<string, Piece>();
        }

        public void Update()
        {
            _pieces.ToList().ForEach(p => p.Value.Update());
        }

        public void AddNewComponent(Configuration.Personality personality, Configuration.Size size)
        {
            var pieceName = "Piece_" + _pieces.Count;
            _pieces.Add(pieceName, new Piece(pieceName, personality, size));
        }

        //create an initial component with random settings
        public void AddBlankComponent()
        {
            var numberOfPersonalities = Configuration.Instance.AvailablePersonalities.Count;
            var numberOfSizes = Configuration.Instance.SizeValues.Count;
            var pieceName = "Piece_" + _pieces.Count;

            _pieces.Add(pieceName, new Piece(pieceName,
                Configuration.Instance.AvailablePersonalities[Random.Range(0, numberOfPersonalities)],
                Configuration.Instance.AvailableSizes[Random.Range(0, numberOfSizes)]));

            UIManager.Instance.AddNewAgentPieceUI(pieceName);
            // TODO - adicionar à lista da barra em baixo do ecrã e tentar tirar uma imagem daquilo
            // talvez fazer 1º a lógica de clickar e carregar o objecto só quando não está lá e apagar da barra
            // image fica para depois em último caso pedir especificações do body e usar um icon semelhante
        }

        public void EraseAgentPiece(string pieceName)
        {
            //UIManager.Instance.DestroyAgentPieceUI(pieceName);
            Piece temPiece =_pieces[pieceName];
            _pieces.Remove(pieceName);
            temPiece.DestroyPiece();
        }

        public void EraseCurrentAgent()
        {
            _pieces.ToList().ForEach(p => UIManager.Instance.DestroyAgentPieceUI(p.Value.Name));
            _pieces.ToList().ForEach(p => p.Value.DestroyPiece());
            _pieces.Clear();
        }

        public void OnDrawGizmos()
        {
            _pieces.ToList().ForEach(p => p.Value.OnDrawGizmos());
        }

        public void OnGUI()
        {
            _pieces.ToList().ForEach(p => p.Value.OnGUI());
        }
    }
}