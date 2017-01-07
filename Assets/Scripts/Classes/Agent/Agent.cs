using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.IO;
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

            //check if there are any saved pieces
            if (PlayerPrefs.GetInt("pieceCount", -1) > 0)
            {
                RecoverLastSessionPieces();
            }

            if (SessionLogger.Instance != null)
                SessionLogger.Instance.WriteToLogFile("Agent initialization complete.");
        }

        public void Update()
        {
            _pieces.ToList().ForEach(p => p.Value.Update());
            _piecesUIManagers.ToList().ForEach(p => p.Value.Update());
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
            int pieceIndex = _currentPieceIndex;
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

            StorePieceInformation(pieceIndex, newPiece);

            SessionLogger.Instance.WriteToLogFile("Added custom piece: " + pieceName + " (" + newPiece.Personality + ", " + newPiece.Body.Size + " size, " + CurrentApplicationMode + ").");
        }


        //Allows copying components
        public void AddComponent(Piece piece)
        {
            int pieceIndex = _currentPieceIndex;
            string pieceName = Constants.Instance.PersonalitiesStrings[piece.Personality] + " " + pieceIndex;
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

            StorePieceInformation(pieceIndex, newPiece);

            SessionLogger.Instance.WriteToLogFile("Added copy piece: " + pieceName + " (" + newPiece.Personality + ", " + newPiece.Body.Size + " size, " + CurrentApplicationMode + ").");
        }

        public void EraseAgentPiece(string pieceName)
        {
            Piece tempPiece =_pieces[pieceName];

            ErasePieceInformation(tempPiece);

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
                
                //Update saved pieces - only updates when its 0
                PlayerPrefs.SetInt("pieceCount", _currentPieceIndex);
                PlayerPrefs.Save();
            }

            PieceUIManager tempUI = _piecesUIManagers[pieceName];
            _piecesUIManagers.Remove(pieceName);
            tempUI.DestroyPieceUI();

            SessionLogger.Instance.WriteToLogFile("Erased agent piece: " + pieceName + ".");
        }

        public void EraseCurrentAgent()
        {
            _piecesUIManagers.ToList().ForEach(p => p.Value.DestroyPiece());
            _piecesUIManagers.Clear();
            _pieces.ToList().ForEach(p => p.Value.DestroyPiece());
            _pieces.Clear();

            _currentPieceIndex = 0;
            
            //Update saved pieces
            PlayerPrefs.SetInt("pieceCount", _currentPieceIndex);
            PlayerPrefs.Save();

            SessionLogger.Instance.WriteToLogFile("Erased agent.");
        }

        public void ToggleAgentVisibility()
        {
            _piecesUIManagers.ToList().ForEach(p => p.Value.TogglePieceVisibility());

            SessionLogger.Instance.WriteToLogFile("Toggled agent visibility.");
        }

        public void UpdateOtherAutonomousPieces(Piece piece)
        {
            if (piece.PieceMode == Configuration.ApplicationMode.AutonomousAgent)
            {
                _pieces.ToList().FindAll(p => p.Value.Name != piece.Name && p.Value.PieceMode == Configuration.ApplicationMode.AutonomousAgent)
                    .ForEach(p => p.Value.StoreAgentPiece(piece));
            }
        }

        public void RecoverLastSessionPieces()
        {
            try
            {
                int piecesToRecover = PlayerPrefs.GetInt("pieceCount", -1);
                string name;
                string personalityString;
                string pieceModeString;
                Configuration.Personality personality;

                for (int i = 0; i < piecesToRecover; i++)
                {
                    //if key was removed continue
                    if (!PlayerPrefs.HasKey("piece" + i + "name"))
                    {
                        continue;
                    }

                    name = PlayerPrefs.GetString("piece" + i + "name", "");
                    personalityString = PlayerPrefs.GetString("piece" + i + "personality", "");
                    personality = Configuration.Instance.AvailablePersonalities.Find(p => p.ToString() == personalityString);
                    pieceModeString = PlayerPrefs.GetString("piece" + i + "autonomy", "");


                    //we must return application the agent to the current application in certain cases
                    Configuration.ApplicationMode oldMode = CurrentApplicationMode;
                    int modeEnumLength = Enum.GetNames(typeof(Configuration.ApplicationMode)).Length;
                    for (int j = 0; j < modeEnumLength; j++)
                    {
                        if (pieceModeString == ((Configuration.ApplicationMode) j).ToString())
                        {
                            CurrentApplicationMode = (Configuration.ApplicationMode)j;
                            break;
                        }
                    }

                    if (CurrentApplicationMode == Configuration.ApplicationMode.ManuallyActivatedAgent)
                    {
                        Configuration.Size storedPieceSize = Configuration.Size.Medium;
                        int sizeEnumLength = Enum.GetNames(typeof(Configuration.Size)).Length;
                        for (int j = 0; j < sizeEnumLength; j++)
                        {
                            if (PlayerPrefs.GetString("piece" + i + "size", "") == ((Configuration.Size)j).ToString())
                            {
                                storedPieceSize = (Configuration.Size)j;
                                break;
                            }
                        }
                    
                        AddComponent(personality, storedPieceSize, name);
                    }
                    else
                    {
                        AddComponent(personality, name);
                    }

                    CurrentApplicationMode = oldMode;
                }

            }
            catch (Exception)
            {
                SessionLogger.Instance.WriteToLogFile("Problem while recovering last session's pieces in agent.");
            }
        }

        public void StorePieceInformation(int pieceIndex, Piece piece)
        {
            //Update saved pieces
            PlayerPrefs.SetInt("pieceCount", _currentPieceIndex);
            PlayerPrefs.SetString("piece" + pieceIndex + "name", piece.Name);
            PlayerPrefs.SetString("piece" + pieceIndex + "personality", piece.Personality.ToString());
            PlayerPrefs.SetString("piece" + pieceIndex + "autonomy", CurrentApplicationMode.ToString());

            if (CurrentApplicationMode == Configuration.ApplicationMode.ManuallyActivatedAgent)
            {
                PlayerPrefs.SetString("piece" + pieceIndex + "size", piece.Body.Size.ToString());
            }
            PlayerPrefs.Save();
        }

        public void ErasePieceInformation(Piece piece)
        {
            if (PlayerPrefs.HasKey("pieceCount"))
            {
                for (int i = 0; i < PlayerPrefs.GetInt("pieceCount"); i++)
                {
                    Debug.Log("Erase name: " + piece.Name + ", stored name: " + PlayerPrefs.GetString("piece" + i + "name"));
                    if (piece.Name == PlayerPrefs.GetString("piece" + i + "name"))
                    {
                        //Delete saved piece
                        PlayerPrefs.DeleteKey("piece" + i + "name");
                        PlayerPrefs.DeleteKey("piece" + i + "personality");
                        PlayerPrefs.DeleteKey("piece" + i + "autonomy");

                        if (piece.PieceMode == Configuration.ApplicationMode.ManuallyActivatedAgent)
                        {
                            PlayerPrefs.DeleteKey("piece" + i + "size");
                        }
                        PlayerPrefs.Save();
                        return;
                    }
                }
            }
        }
    }
}
