using System;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace BuildBuddy
{
    [Serializable]
    public sealed class ManifestPermission : ManifestElement
    {
        public enum ProtectionLevel
        {
            NORMAL,
            DANGEROUS,
            SIGNATURE,
            SIGNATUREORSYSTEM
        }

        [SerializeField] private string description = "";
        [SerializeField] private bool display;
        [SerializeField] private string icon = "";
        [SerializeField] private string label = "";

        [SerializeField] private new string name = "";
        [SerializeField] private ProtectionLevel protectionLevel;

        //Constructed by editor window
        public static ManifestPermission CreateInstance()
        {
            var permission = CreateInstance<ManifestPermission>();
            permission.node = null;
            permission.elementEditStatus = EditStatus.EDITED;
            permission.protectionLevel = ProtectionLevel.NORMAL;
            return permission;
        }

        //Constructed from existing entry in AndroidManifest
        public static ManifestPermission CreateInstance(XmlNode node)
        {
            var permission = CreateInstance<ManifestPermission>();
            permission.node = (XmlElement) node;
            permission.elementEditStatus = EditStatus.NONE;
            permission.Initialize();
            return permission;
        }

        public override void OnGUI()
        {
            display = EditorGUILayout.Foldout(display, "Permission: " + name);
            if (!display)
                return;
            EditorGUI.BeginChangeCheck();
            name = EditorGUILayout.TextField("Name: ", name);
            BBGuiHelper.BeginIndent();
            {
                protectionLevel = (ProtectionLevel) EditorGUILayout.EnumPopup("Protection Level: ", protectionLevel);
                label = EditorGUILayout.TextField("Label: ", label);
                EditorGUILayout.LabelField("Description: ");
                description = EditorGUILayout.TextArea(description);
                GUILayout.BeginHorizontal();
                {
                    icon = EditorGUILayout.TextField("Icon: ", icon);
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
            if (node.HasAttribute("android:label"))
            {
                label = node.Attributes["android:label"].Value;
            }
            if (node.HasAttribute("android:description"))
            {
                description = node.Attributes["android:description"].Value;
            }
            if (node.HasAttribute("android:icon"))
            {
                icon = node.Attributes["android:icon"].Value;
            }
            if (node.HasAttribute("android:protectionLevel"))
            {
                ProtectionLevelFromString(node.Attributes["android:protectionLevel"].Value);
            }
        }

        private void ProtectionLevelFromString(string pLevelString)
        {
            if (pLevelString.Equals("normal"))
            {
                protectionLevel = ProtectionLevel.NORMAL;
            }
            else if (pLevelString.Equals("dangerous"))
            {
                protectionLevel = ProtectionLevel.DANGEROUS;
            }
            else if (pLevelString.Equals("signature"))
            {
                protectionLevel = ProtectionLevel.SIGNATURE;
            }
            else if (pLevelString.Equals("signatureOrSystem"))
            {
                protectionLevel = ProtectionLevel.SIGNATUREORSYSTEM;
            }
            else
            {
                protectionLevel = ProtectionLevel.NORMAL;
            }
        }

        private string ProtectionLevelString()
        {
            switch (protectionLevel)
            {
                case ProtectionLevel.NORMAL:
                    return "normal";
                case ProtectionLevel.DANGEROUS:
                    return "dangerous";
                case ProtectionLevel.SIGNATURE:
                    return "signature";
                case ProtectionLevel.SIGNATUREORSYSTEM:
                    return "signatureOrSystem";
            }
            return "";
        }

        #region override

        protected override void CreateNode(XmlDocument document)
        {
            node = document.CreateElement("permission");
            var manifestElement = document.GetElementsByTagName("manifest")[0];
            manifestElement.AppendChild(node);
        }

        protected override void UpdateAttributes(XmlDocument document)
        {
            CreateAndroidAttribute(document, "name", name);
            CreateAndroidAttribute(document, "protectoinLevel", ProtectionLevelString());
            if (!label.Equals(""))
            {
                CreateAndroidAttribute(document, "label", label);
            }
            else if (node.HasAttribute("android:label"))
            {
                node.RemoveAttribute("android:label");
            }
            if (!description.Equals(""))
            {
                CreateAndroidAttribute(document, "description", description);
            }
            else if (node.HasAttribute("android:description"))
            {
                node.RemoveAttribute("android:description");
            }
            if (!icon.Equals(""))
            {
                CreateAndroidAttribute(document, "icon", icon);
            }
            else if (node.HasAttribute("android:icon"))
            {
                node.RemoveAttribute("android:icon");
            }
        }

        #endregion
    }
}