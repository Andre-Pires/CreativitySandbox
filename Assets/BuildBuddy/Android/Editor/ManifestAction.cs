using System;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace BuildBuddy
{
    [Serializable]
    public sealed class ManifestAction : ManifestElement
    {
        [SerializeField] private new string name = "";

        private XmlElement parent;

        //Constructed from existing entry in AndroidManifest or constructed as child of an Activity
        public static ManifestAction CreateInstance(XmlNode parent, XmlNode node = null)
        {
            var action = CreateInstance<ManifestAction>();
            action.parent = (XmlElement) parent;
            action.node = (XmlElement) node;
            action.elementEditStatus = EditStatus.NONE;
            if (node != null)
            {
                action.Initialize();
            }
            return action;
        }

        public override void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            {
                name = EditorGUILayout.TextField("Action Name: ", name);
                if (EditorGUI.EndChangeCheck())
                {
                    elementEditStatus = EditStatus.EDITED;
                }
                if (GUILayout.Button("Remove Action"))
                {
                    elementEditStatus = EditStatus.REMOVED;
                }
            }
            GUILayout.EndHorizontal();
        }

        private void Initialize()
        {
            if (node.HasAttribute("android:name"))
            {
                name = node.Attributes["android:name"].Value;
            }
        }

        #region override

        protected override void CreateNode(XmlDocument document)
        {
            node = document.CreateElement("action");
            parent.AppendChild(node);
        }

        protected override void UpdateAttributes(XmlDocument document)
        {
            CreateAndroidAttribute(document, "name", name);
        }

        #endregion
    }
}