﻿using System;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace BuildBuddy
{
    [Serializable]
    public sealed class ManifestMetaData : ManifestElement
    {
        [SerializeField] private bool display;
        [SerializeField] private bool isResource;

        [SerializeField] private new string name = "";

        private XmlElement parent;
        [SerializeField] private string value = "";

        //Constructed by editor window
        public static ManifestMetaData CreateInstance()
        {
            var metaData = CreateInstance<ManifestMetaData>();
            metaData.node = null;
            metaData.elementEditStatus = EditStatus.EDITED;
            return metaData;
        }

        //Constructed from existing entry in AndroidManifest or constructed as child of an Activity
        public static ManifestMetaData CreateInstance(XmlNode parent, XmlNode node = null)
        {
            var metaData = CreateInstance<ManifestMetaData>();
            metaData.parent = (XmlElement) parent;
            metaData.node = (XmlElement) node;
            metaData.elementEditStatus = EditStatus.NONE;
            if (node != null)
            {
                metaData.Initialize();
            }
            return metaData;
        }

        public override void OnGUI()
        {
            display = EditorGUILayout.Foldout(display, "Meta-Data: " + name);
            if (!display)
                return;
            EditorGUI.BeginChangeCheck();
            name = EditorGUILayout.TextField("Name: ", name);
            BBGuiHelper.BeginIndent();
            {
                value = EditorGUILayout.TextField("Value: ", value);
                GUILayout.BeginHorizontal();
                {
                    isResource = EditorGUILayout.Toggle("Is Resource: ", isResource);

                    if (EditorGUI.EndChangeCheck())
                    {
                        elementEditStatus = EditStatus.EDITED;
                    }
                    if (GUILayout.Button("Remove"))
                    {
                        elementEditStatus = EditStatus.REMOVED;
                    }
                }
                GUILayout.EndHorizontal();
            }
            BBGuiHelper.EndIndent();
        }

        private void Initialize()
        {
            if (node.HasAttribute("android:name"))
            {
                name = node.Attributes["android:name"].Value;
            }
            if (node.HasAttribute("android:value"))
            {
                value = node.Attributes["android:value"].Value;
            }
            else if (node.HasAttribute("android:resource"))
            {
                value = node.Attributes["android:resource"].Value;
                isResource = true;
            }
        }

        #region override

        protected override void CreateNode(XmlDocument document)
        {
            node = document.CreateElement("meta-data");
            if (parent == null)
            {
                parent = (XmlElement) document.GetElementsByTagName("application")[0];
            }
            parent.AppendChild(node);
        }

        protected override void UpdateAttributes(XmlDocument document)
        {
            CreateAndroidAttribute(document, "name", name);
            if (isResource)
            {
                CreateAndroidAttribute(document, "resource", value);
            }
            else
            {
                CreateAndroidAttribute(document, "value", value);
            }
        }

        #endregion
    }
}