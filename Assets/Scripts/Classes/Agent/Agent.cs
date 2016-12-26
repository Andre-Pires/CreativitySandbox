using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        public Configuration.ApplicationMode CurrentApplicationMode;

        public Agent(Configuration.ApplicationMode currentApplicationMode)
        {
            CurrentApplicationMode = currentApplicationMode;
            AppUIManager.Instance.PieceSelection.GetComponent<CreateAgentPiece>().OnAutonomousPieceCreation += AddComponent;
            AppUIManager.Instance.PieceSelection.GetComponent<CreateAgentPiece>().OnManualPieceCreation += AddComponent;

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

            List<Piece> autonomousPieces = new List<Piece>();
            _pieces.ToList().FindAll(p => p.Value.Name != pieceName && p.Value.PieceMode == Configuration.ApplicationMode.AutonomousAgent)
                .ForEach(p => autonomousPieces.Add(p.Value));

            Piece newPiece = new Piece(pieceName, Configuration.Personality.CustomPersonality,
                Configuration.Instance.AvailableSizes[Random.Range(0, numberOfSizes)], CurrentApplicationMode, autonomousPieces);
            _pieces.Add(pieceName, newPiece);

            PieceUIManager newPieceManager = new PieceUIManager(newPiece, this);
            _piecesUIManagers.Add(pieceName, newPieceManager);

            //adding the piece to the other agents minds 
            UpdateOtherAutonomousPieces(newPiece);
        }

        public void AddComponent(Configuration.Personality personality, string name = null)
        {
            AddComponent(personality, Configuration.Instance.PersonalitySizes[personality], name);
        }

        public void AddComponent(Configuration.Personality personality, Configuration.Size size, string name = null)
        {
            string pieceName;
            if (name == null)
            {
                pieceName = Constants.Instance.PersonalitiesStrings[personality] + " " + _currentPieceIndex;
            }
            else
            {
                var nameExtension = 0;

                foreach (var piece in _pieces)
                {
                    //allows to detect if the name as already occurred and adds an extension to differenciate it
                    if (Regex.IsMatch(piece.Key, string.Format(@"({0}|({0}(\s\d*)?))\z", Regex.Escape(name))))
                    {
                        nameExtension++;
                    }
                }

                if (nameExtension == 0)
                {
                    pieceName = name;
                }
                else
                {
                    pieceName = name + " " + nameExtension;
                }
            }
            _currentPieceIndex++;

            List<Piece> autonomousPieces = new List<Piece>();
            _pieces.ToList().FindAll(p => p.Value.Name != pieceName && p.Value.PieceMode == Configuration.ApplicationMode.AutonomousAgent)
                .ForEach(p => autonomousPieces.Add(p.Value));

            Piece newPiece = new Piece(pieceName, personality, size, CurrentApplicationMode, autonomousPieces);
            _pieces.Add(pieceName, newPiece);

            PieceUIManager newPieceManager = new PieceUIManager(newPiece, this);
            _piecesUIManagers.Add(pieceName, newPieceManager);

            //adding the piece to the other agents minds 
            UpdateOtherAutonomousPieces(newPiece);
        }

        
        //Allows copying components
        public void AddComponent(Piece piece)
        {
            var pieceName = Constants.Instance.PersonalitiesStrings[piece.Personality] + " " + _currentPieceIndex;
            _currentPieceIndex++;

            List<Piece> autonomousPieces = new List<Piece>();
            _pieces.ToList().FindAll(p => p.Value.Name != pieceName && p.Value.PieceMode == Configuration.ApplicationMode.AutonomousAgent)
                .ForEach(p => autonomousPieces.Add(p.Value));

            Piece newPiece = new Piece(pieceName, piece, autonomousPieces);
            _pieces.Add(pieceName, newPiece);

            PieceUIManager newPieceManager = new PieceUIManager(newPiece, this);
            _piecesUIManagers.Add(pieceName, newPieceManager);

            //adding the piece to the other agents minds 
            UpdateOtherAutonomousPieces(newPiece);
        }

        public void EraseAgentPiece(string pieceName)
        {
            Piece tempPiece =_pieces[pieceName];

            if (tempPiece.PieceMode == Configuration.ApplicationMode.AutonomousAgent)
            {
                //clearing the piece from other agents minds 
                _pieces.ToList().FindAll(p => p.Value.Name != pieceName 
                    && p.Value.PieceMode == Configuration.ApplicationMode.AutonomousAgent).ForEach(p => p.Value.RemoveStoredAgentPiece(tempPiece));
            }

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

        public void UpdateOtherAutonomousPieces(Piece piece)
        {
            if (piece.PieceMode == Configuration.ApplicationMode.AutonomousAgent)
            {
                _pieces.ToList().FindAll(p => p.Value.Name != piece.Name && p.Value.PieceMode == Configuration.ApplicationMode.AutonomousAgent)
                    .ForEach(p => p.Value.StoreAgentPiece(piece));
            }
        }

    }
}