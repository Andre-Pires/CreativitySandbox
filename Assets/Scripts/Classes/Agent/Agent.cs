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
        private readonly Dictionary<string, Piece> _pieces;
        private readonly Dictionary<string, PieceUIManager> _piecesUIManagers;
        private int _currentPieceIndex;

        public Agent()
        {
            //provide option to clear agent configuration
            CreateAgentPiece.Instance.OnSelect += AddComponent;

            _pieces = new Dictionary<string, Piece>();
            _piecesUIManagers = new Dictionary<string, PieceUIManager>();
        }

        public void Update()
        {
            _pieces.ToList().ForEach(p => p.Value.Update());
            _piecesUIManagers.ToList().ForEach(p => p.Value.Update());
        }

        //create an initial component with random settings
        public void AddComponent()
        {
            var numberOfSizes = Configuration.Instance.SizeValues.Count;
            var pieceName = Constants.Instance.PersonalitiesStrings[Configuration.Personality.CustomPersonality] + " " + _currentPieceIndex;
            _currentPieceIndex++;

            Piece newPiece = new Piece(pieceName, Configuration.Personality.CustomPersonality,
                Configuration.Instance.AvailableSizes[Random.Range(0, numberOfSizes)], _pieces);
            _pieces.Add(pieceName, newPiece);

            PieceUIManager newPieceManager = new PieceUIManager(newPiece, this);
            _piecesUIManagers.Add(pieceName, newPieceManager);

            //adding the piece to the other agents minds 
            _pieces.ToList().FindAll(p => p.Value.Name != pieceName).ForEach(p => p.Value.StoreAgentPiece(newPiece));
        }

        public void AddComponent(Configuration.Personality personality)
        {
            var numberOfSizes = Configuration.Instance.SizeValues.Count;
            var pieceName = Constants.Instance.PersonalitiesStrings[personality] + " " + _currentPieceIndex;
            _currentPieceIndex++;

            Piece newPiece = new Piece(pieceName, personality,
                Configuration.Instance.PersonalitySizes[personality], _pieces);
            _pieces.Add(pieceName, newPiece);

            PieceUIManager newPieceManager = new PieceUIManager(newPiece, this);
            _piecesUIManagers.Add(pieceName, newPieceManager);

            //adding the piece to the other agents minds 
            _pieces.ToList().FindAll(p => p.Value.Name != pieceName).ForEach(p => p.Value.StoreAgentPiece(newPiece));
        }

        public void AddComponent(Piece piece)
        {
            var pieceName = Constants.Instance.PersonalitiesStrings[piece.Personality] + " " + _currentPieceIndex;
            _currentPieceIndex++;

            Piece newPiece = new Piece(pieceName, piece, _pieces);
            _pieces.Add(pieceName, newPiece);

            PieceUIManager newPieceManager = new PieceUIManager(newPiece, this);
            _piecesUIManagers.Add(pieceName, newPieceManager);

            //adding the piece to the other agents minds 
            _pieces.ToList().FindAll(p => p.Value.Name != pieceName).ForEach(p =>  p.Value.StoreAgentPiece(newPiece));
        }

        public void EraseAgentPiece(string pieceName)
        {
            Piece tempPiece =_pieces[pieceName];

            //clearing the piece from other agents minds 
            _pieces.ToList().FindAll(p => p.Value.Name != pieceName).ForEach(p => p.Value.RemoveStoredAgentPiece(tempPiece));

            _pieces.Remove(pieceName);
            tempPiece.DestroyPiece();

            if (_pieces.Count == 0)
            {
                _currentPieceIndex = 0;
            }

            PieceUIManager tempUI = _piecesUIManagers[pieceName];
            _piecesUIManagers.Remove(pieceName);
            tempUI.DestroyPieceUI();
        }

        public void EraseCurrentAgent()
        {
            _piecesUIManagers.ToList().ForEach(p => p.Value.DestroyPiece());
            _piecesUIManagers.Clear();
            _pieces.ToList().ForEach(p => p.Value.DestroyPiece());
            _pieces.Clear();

            _currentPieceIndex = 0;
        }

        public void ToggleAgentVisibility()
        {
            _piecesUIManagers.ToList().ForEach(p => p.Value.TogglePieceVisibility());
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