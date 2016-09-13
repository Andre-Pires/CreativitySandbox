using System;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace BuildBuddy
{
    [Serializable]
    public sealed class ManifestCategory : ManifestElement
    {
        [SerializeField] private new string name = "";

        private XmlElement parent;

        //Constructed from existing entry in AndroidManifest or constructed as child of an Activity
        public static ManifestCategory CreateInstance(XmlNode parent, XmlNode node = null)
        {
            var category = CreateInstance<ManifestCategory>();
            category.parent = (XmlElement) parent;
            category.node = (XmlElement) node;
            category.elementEditStatus = EditStatus.NONE;
            if (node != null)
            {
                category.Initialize();
            }
            return category;
        }

        public override void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            {
                name = EditorGUILayout.TextField("Category Name: ", name);
                if (EditorGUI.EndChangeCheck())
                {
                    elementEditStatus = EditStatus.EDITED;
                }
                if (GUILayout.Button("Remove Category"))
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
            node = document.CreateElement("category");
            parent.AppendChild(node);
        }

        protected override void UpdateAttributes(XmlDocument document)
        {
            CreateAndroidAttribute(document, "name", name);
        }

        #endregion
    }
}