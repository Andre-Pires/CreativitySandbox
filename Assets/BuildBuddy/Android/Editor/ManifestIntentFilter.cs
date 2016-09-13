using System;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace BuildBuddy
{
    [Serializable]
    public sealed class ManifestIntentFilter : ManifestElement
    {
        [SerializeField] private readonly List<ManifestAction> actionList = new List<ManifestAction>();
        [SerializeField] private readonly List<ManifestCategory> categoryList = new List<ManifestCategory>();
        [SerializeField] private readonly List<ManifestData> dataList = new List<ManifestData>();
        [SerializeField] private bool display;

        [SerializeField] private string icon = "";
        [SerializeField] private string label = "";

        private XmlElement parent;
        [SerializeField] private int priority;
        //Constructed by editor window
        public static ManifestIntentFilter CreateInstance()
        {
            var intentFilter = CreateInstance<ManifestIntentFilter>();
            intentFilter.node = null;
            intentFilter.elementEditStatus = EditStatus.EDITED;
            return intentFilter;
        }

        //Constructed from existing entry in AndroidManifest or constructed as child of an Activity
        public static ManifestIntentFilter CreateInstance(XmlNode parent, XmlNode node = null)
        {
            var intentFilter = CreateInstance<ManifestIntentFilter>();
            intentFilter.parent = (XmlElement) parent;
            intentFilter.node = (XmlElement) node;
            intentFilter.elementEditStatus = EditStatus.NONE;
            if (node != null)
            {
                intentFilter.Initialize();
            }
            return intentFilter;
        }

        public override void OnGUI()
        {
            display = EditorGUILayout.Foldout(display, "Intent-Filter: " + name);
            if (!display)
                return;
            EditorGUI.BeginChangeCheck();
            BBGuiHelper.BeginIndent();
            {
                icon = EditorGUILayout.TextField("Icon: ", icon);
                label = EditorGUILayout.TextField("Label: ", label);
                priority = EditorGUILayout.IntField("Priority: ", priority);
                for (var i = 0; i < actionList.Count; i++)
                {
                    Undo.RecordObject(actionList[i], "Action");
                    if (actionList[i].ElementEditStatus != EditStatus.REMOVED)
                    {
                        actionList[i].OnGUI();
                    }
                }
                for (var i = 0; i < categoryList.Count; i++)
                {
                    Undo.RecordObject(categoryList[i], "Category");
                    if (categoryList[i].ElementEditStatus != EditStatus.REMOVED)
                    {
                        categoryList[i].OnGUI();
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    elementEditStatus = EditStatus.EDITED;
                }
                for (var i = 0; i < dataList.Count; i++)
                {
                    Undo.RecordObject(dataList[i], "Data");
                    if (dataList[i].ElementEditStatus != EditStatus.REMOVED)
                    {
                        dataList[i].OnGUI();
                    }
                    if (dataList[i].ElementEditStatus != EditStatus.NONE)
                    {
                        elementEditStatus = EditStatus.EDITED;
                    }
                }
                EditorGUI.BeginChangeCheck();
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("New Action: "))
                    {
                        actionList.Add(ManifestAction.CreateInstance(node));
                    }
                    if (GUILayout.Button("New Category: "))
                    {
                        categoryList.Add(ManifestCategory.CreateInstance(node));
                    }
                    if (GUILayout.Button("New Data: "))
                    {
                        dataList.Add(ManifestData.CreateInstance(node));
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        elementEditStatus = EditStatus.EDITED;
                    }
                    if (GUILayout.Button("Remove Intent-Filter"))
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
            if (node.HasAttribute("android:icon"))
            {
                icon = node.Attributes["android:icon"].Value;
            }
            if (node.HasAttribute("android:label"))
            {
                label = node.Attributes["android:label"].Value;
            }
            if (node.HasAttribute("android:priority"))
            {
                priority = Convert.ToInt32(node.Attributes["android:name"].Value);
            }
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name.Equals("action"))
                {
                    actionList.Add(ManifestAction.CreateInstance(node, child));
                }
                else if (child.Name.Equals("category"))
                {
                    categoryList.Add(ManifestCategory.CreateInstance(node, child));
                }
                else if (child.Name.Equals("data"))
                {
                    dataList.Add(ManifestData.CreateInstance(node, child));
                }
            }
        }

        #region override

        protected override void CreateNode(XmlDocument document)
        {
            node = document.CreateElement("intent-filter");
            parent.AppendChild(node);
        }

        protected override void UpdateAttributes(XmlDocument document)
        {
            UpdateOptionalAttribute(document, "icon", !icon.Equals(""), icon);
            UpdateOptionalAttribute(document, "label", !label.Equals(""), label);
            UpdateOptionalAttribute(document, "priority", priority != 0, priority + "");
            for (var i = 0; i < actionList.Count; i++)
            {
                var action = actionList[i];
                if (action.ElementEditStatus == EditStatus.REMOVED)
                {
                    actionList.RemoveAt(i);
                }
                action.ApplyChanges(document);
            }
            for (var i = 0; i < categoryList.Count; i++)
            {
                var category = categoryList[i];
                if (category.ElementEditStatus == EditStatus.REMOVED)
                {
                    categoryList.RemoveAt(i);
                }
                category.ApplyChanges(document);
            }
            for (var i = 0; i < dataList.Count; i++)
            {
                var data = dataList[i];
                if (data.ElementEditStatus == EditStatus.REMOVED)
                {
                    dataList.RemoveAt(i);
                }
                data.ApplyChanges(document);
            }
        }

        #endregion
    }
}