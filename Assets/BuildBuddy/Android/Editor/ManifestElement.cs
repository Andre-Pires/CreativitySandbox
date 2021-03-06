﻿using System;
using System.Xml;
using UnityEngine;

namespace BuildBuddy
{
    public enum EditStatus
    {
        NONE,
        REMOVED,
        EDITED
    }

    [Serializable]
    public abstract class ManifestElement : ScriptableObject
    {
        private const string emptyDoc =
            "<manifest>" +
            "<application />" +
            "</manifest>";

        [SerializeField] protected EditStatus elementEditStatus;
        public XmlElement node;

        public EditStatus ElementEditStatus
        {
            get { return elementEditStatus; }
            set { elementEditStatus = value; }
        }

        public void ApplyChanges(XmlDocument document)
        {
            if (node == null)
            {
                CreateNode(document);
            }
            if (elementEditStatus == EditStatus.REMOVED)
            {
                if (node.ParentNode != null)
                    node.ParentNode.RemoveChild(node);
            }
            else
            {
                UpdateAttributes(document);
            }
            elementEditStatus = EditStatus.NONE;
        }

        protected void CreateAndroidAttribute(XmlDocument document, string name, string value)
        {
            var attribute = document.CreateAttribute("android", name, "http://schemas.android.com/apk/res/android");
            attribute.Value = value;
            node.SetAttributeNode(attribute);
        }

        protected bool InitializeBoolAttribute(string attribute, bool defaultValue)
        {
            if (!node.HasAttribute(attribute))
            {
                return defaultValue;
            }
            if (node.Attributes[attribute].Value.Equals("true"))
                return true;
            return false;
        }

        protected void UpdateOptionalAttribute(XmlDocument document, string attribute, bool create, string value)
        {
            if (create)
            {
                CreateAndroidAttribute(document, attribute, value);
            }
            else if (node.HasAttribute("android:" + attribute))
            {
                node.RemoveAttribute("android:" + attribute);
            }
        }

        public override string ToString()
        {
            return node.OuterXml;
        }

        public XmlElement CopyNode(XmlDocument document)
        {
            var oldNode = node;
            node = document.CreateElement(node.Name);
            foreach (XmlAttribute attribute in oldNode.Attributes)
            {
                CreateAndroidAttribute(document, attribute.Name, attribute.Value);
            }
            /*foreach (XmlElement child in oldNode.ChildNodes) {
				node.AppendChild(document.C
			}*/
            return node;
        }

        #region abstract

        public abstract void OnGUI();
        protected abstract void CreateNode(XmlDocument document);
        protected abstract void UpdateAttributes(XmlDocument document);

        #endregion
    }
}