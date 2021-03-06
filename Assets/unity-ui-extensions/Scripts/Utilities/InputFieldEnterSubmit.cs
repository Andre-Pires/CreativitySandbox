﻿/// Credit Vicente Russo  
/// Sourced from - https://bitbucket.org/ddreaper/unity-ui-extensions/issues/23/returnkeytriggersbutton

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Utilities
{
    /// <summary>
    ///     Usage: Add this component to the input and add the function to execute to the EnterSubmit event of this script
    /// </summary>
    [RequireComponent(typeof(InputField))]
    [AddComponentMenu("UI/Extensions/Input Field Submit")]
    public class InputFieldEnterSubmit : MonoBehaviour
    {
        private InputField _input;

        public EnterSubmitEvent EnterSubmit;

        private void Awake()
        {
            _input = GetComponent<InputField>();
            _input.onEndEdit.AddListener(OnEndEdit);
        }

        public void OnEndEdit(string txt)
        {
            if (!Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.KeypadEnter)) return;
            EnterSubmit.Invoke(txt);
        }

        [Serializable]
        public class EnterSubmitEvent : UnityEvent<string>
        {
        }
    }
}