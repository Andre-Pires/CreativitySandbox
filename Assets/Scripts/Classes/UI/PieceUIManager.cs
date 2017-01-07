using System;
using System.Collections.Generic;
using Assets.Scripts.Classes.Agent;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.IO;
using Assets.Scripts.Layout;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Classes.UI
{
    public class PieceUIManager
    {
        private readonly Piece _piece;
        private readonly Agent.Agent _agent;
        private readonly GameObject _availableAgentPiecesList;
        private GameObject _pieceButton;
        private GameObject _deleteAgentButton;
        private GameObject _hideAgentButton;

        //settings popup references
        private PiecePopupUI _popupUI;

        public PieceUIManager(Piece piece, Agent.Agent agent)
        {
            _piece = piece;
            _agent = agent;

            // Note: due to the way unity works some UI objects reference must be kept since application start given that 
            // Gameobject.Find doesn't find inactive objects
            _availableAgentPiecesList = AppUIManager.Instance.AvailableAgentPiecesList;

            SetupPieceButtonUI();

            if (_agent.CurrentApplicationMode == Configuration.ApplicationMode.ConfigurableAgent)
            {
                //after setting up the piece button pass to the popup so it can reflect its changes
                _popupUI = new PiecePopupUI(_piece, _pieceButton);
            }

            if (SessionLogger.Instance != null)
                SessionLogger.Instance.WriteToLogFile("Created  piece's UI manager: " + _piece.Name);
        }


        private void SetupPieceButtonUI()
        {
            _pieceButton = Object.Instantiate(Resources.Load("Prefabs/UISettings/AgentPieceItem")) as GameObject;

            _deleteAgentButton = Utility.GetChild(_pieceButton, "DeleteAgent");
            _hideAgentButton = Utility.GetChild(_pieceButton, "HideAgent");

            _pieceButton.name = _piece.Name + "_Button";
            _pieceButton.GetComponentInChildren<Text>().text = _piece.Name;
            _deleteAgentButton.GetComponent<Button>().onClick.AddListener(DestroyPiece);
            _pieceButton.GetComponent<Button>().onClick.AddListener(TogglePieceVisibility);
            _hideAgentButton.GetComponent<Button>().onClick.AddListener(TogglePieceVisibility);

            _pieceButton.transform.SetParent(_availableAgentPiecesList.transform, false);
            _pieceButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Agent/" + _piece.Body.Size);
            _pieceButton.GetComponent<Image>().color = _piece.Body.Color;
        }

        public void TogglePieceVisibility()
        {
            //image that show visibility status of a piece
            if (_hideAgentButton.GetComponent<Image>().sprite.name == "ShowAgent")
            {
                _hideAgentButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/ColorfulButtons/HideAgent");
                SessionLogger.Instance.WriteToLogFile("Revealed piece: " + _piece.Name);
            }
            else
            {
                _hideAgentButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/ColorfulButtons/ShowAgent");
                SessionLogger.Instance.WriteToLogFile("Hid piece: " + _piece.Name);
            }

            _piece.ToggleVisibility();
        }

        public void DestroyPiece()
        {
            _agent.EraseAgentPiece(_piece.Name);
        }

        public void Update()
        {
            if(_popupUI != null)
                _popupUI.Update();
        }

        public void OnDrawGizmos()
        {
            /*
            Gizmos.color = new Color(1, 0, 0, 0.5F);
            Gizmos.DrawCube(cubeObject.transform.position + Vector3.up * 10, new Vector3(1, 1, 1));
            */
        }

        public void OnGUI()
        {
        }

        public void DestroyPieceUI()
        {
            if (_popupUI != null)
                _popupUI.DestroyPopupUI();

            Object.Destroy(_pieceButton);

            SessionLogger.Instance.WriteToLogFile("Destroyed piece's UI manager: " + _piece.Name);
        }


    }
}