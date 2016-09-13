using System;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace BuildBuddy
{
    [Serializable]
    public class AndroidWindowData : ScriptableObject
    {
        public List<ManifestActivity> activityList;
        public ManifestApplication application;
        public bool dirty;

        public bool display;

        public bool displayActivities;
        public bool displayMetaData;
        public bool displayPermissions;
        public bool displayProviders;
        public bool displayReceivers;
        public bool displayServices;
        public bool displayUsesFeatures;
        public bool displayUsesLibrary;
        public bool displayUsesPermissions;
        public List<ManifestElement> editedList = new List<ManifestElement>();
        public List<ManifestMetaData> metaDataList = new List<ManifestMetaData>();
        public List<ManifestPermission> permissionList;
        public List<ManifestProvider> providerList;
        public List<ManifestReceiver> receiverList;
        public List<ManifestService> serviceList;
        public List<ManifestUsesFeature> usesFeatureList;
        public List<ManifestUsesLibrary> usesLibraryList = new List<ManifestUsesLibrary>();
        public List<ManifestUsesPermission> usesPermissionList;
        private AndroidXmlEditor xmlEditor;
        public bool isTemplate { get; set; }

        private XmlDocument document
        {
            get { return xmlEditor.document; }
        }

        public static AndroidWindowData CreateInstance(AndroidXmlEditor editor)
        {
            var instance = CreateInstance<AndroidWindowData>();
            instance.xmlEditor = editor;

            instance.activityList = new List<ManifestActivity>();
            instance.receiverList = new List<ManifestReceiver>();
            instance.serviceList = new List<ManifestService>();
            instance.providerList = new List<ManifestProvider>();
            instance.metaDataList = new List<ManifestMetaData>();
            instance.usesLibraryList = new List<ManifestUsesLibrary>();
            instance.permissionList = new List<ManifestPermission>();
            instance.usesPermissionList = new List<ManifestUsesPermission>();
            instance.usesFeatureList = new List<ManifestUsesFeature>();

            var nodeList = instance.xmlEditor.GetManifestElements();
            foreach (var node in nodeList)
            {
                instance.BuildIManifestElement(node);
            }
            return instance;
        }

        private void BuildIManifestElement(XmlNode node)
        {
            if (node.Name.Equals("uses-permission"))
            {
                usesPermissionList.Add(ManifestUsesPermission.CreateInstance(node));
            }
            else if (node.Name.Equals("uses-feature") && ((XmlElement) node).HasAttribute("android:name"))
            {
                usesFeatureList.Add(ManifestUsesFeature.CreateInstance(node));
            }
            else if (node.Name.Equals("permission"))
            {
                permissionList.Add(ManifestPermission.CreateInstance(node));
            }
            else if (node.Name.Equals("activity"))
            {
                activityList.Add(ManifestActivity.CreateInstance(node));
            }
            else if (node.Name.Equals("receiver"))
            {
                receiverList.Add(ManifestReceiver.CreateInstance(node));
            }
            else if (node.Name.Equals("service"))
            {
                serviceList.Add(ManifestService.CreateInstance(node));
            }
            else if (node.Name.Equals("provider"))
            {
                providerList.Add(ManifestProvider.CreateInstance(node));
            }
            else if (node.Name.Equals("meta-data") && node.ParentNode.Name.Equals("application"))
            {
                metaDataList.Add(ManifestMetaData.CreateInstance(node.ParentNode, node));
            }
            else if (node.Name.Equals("uses-library"))
            {
                usesLibraryList.Add(ManifestUsesLibrary.CreateInstance(node));
            }
            else if (node.Name.Equals("application"))
            {
                application = ManifestApplication.CreateInstance(node);
            }
        }

        public void OnGUI()
        {
            if (isTemplate)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Name:", GUILayout.Width(75));
                    name = EditorGUILayout.TextField(name);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                application.OnGUI();
            }
            EditorGUILayout.LabelField("Application Level:");

            #region Activites

            displayActivities = EditorGUILayout.Foldout(displayActivities, "Activities: (" + activityList.Count + ")");
            if (displayActivities)
            {
                BBGuiHelper.BeginIndent();
                {
                    for (var i = 0; i < activityList.Count; i++)
                    {
                        Undo.RecordObject(activityList[i], "Activity");
                        if (activityList[i].ElementEditStatus == EditStatus.REMOVED)
                        {
                            editedList.Add(activityList[i]);
                            activityList.RemoveAt(i);
                            dirty = true;
                            i--;
                            continue;
                        }
                        activityList[i].OnGUI();
                        if (activityList[i].ElementEditStatus != EditStatus.NONE)
                        {
                            dirty = true;
                        }
                    }
                    if (GUILayout.Button("New Activity"))
                    {
                        activityList.Add(ManifestActivity.CreateInstance());
                    }
                }
                BBGuiHelper.EndIndent();
            }

            #endregion

            #region Receiver

            displayReceivers = EditorGUILayout.Foldout(displayReceivers, "Receivers: (" + receiverList.Count + ")");
            if (displayReceivers)
            {
                BBGuiHelper.BeginIndent();
                {
                    for (var i = 0; i < receiverList.Count; i++)
                    {
                        Undo.RecordObject(receiverList[i], "Receiver");
                        if (receiverList[i].ElementEditStatus == EditStatus.REMOVED)
                        {
                            editedList.Add(receiverList[i]);
                            receiverList.RemoveAt(i);
                            dirty = true;
                            i--;
                            continue;
                        }
                        receiverList[i].OnGUI();
                        if (receiverList[i].ElementEditStatus != EditStatus.NONE)
                        {
                            dirty = true;
                        }
                    }
                    if (GUILayout.Button("New Receiver"))
                    {
                        receiverList.Add(ManifestReceiver.CreateInstance());
                    }
                }
                BBGuiHelper.EndIndent();
            }

            #endregion

            #region Service

            displayServices = EditorGUILayout.Foldout(displayServices, "Services: (" + serviceList.Count + ")");
            if (displayServices)
            {
                BBGuiHelper.BeginIndent();
                {
                    for (var i = 0; i < serviceList.Count; i++)
                    {
                        Undo.RecordObject(serviceList[i], "Service");
                        if (serviceList[i].ElementEditStatus == EditStatus.REMOVED)
                        {
                            editedList.Add(serviceList[i]);
                            serviceList.RemoveAt(i);
                            dirty = true;
                            i--;
                            continue;
                        }
                        serviceList[i].OnGUI();
                        if (serviceList[i].ElementEditStatus != EditStatus.NONE)
                        {
                            dirty = true;
                        }
                    }
                    if (GUILayout.Button("New Service"))
                    {
                        serviceList.Add(ManifestService.CreateInstance());
                    }
                }
                BBGuiHelper.EndIndent();
            }

            #endregion

            #region Provider

            displayProviders = EditorGUILayout.Foldout(displayProviders, "Providers: (" + providerList.Count + ")");
            if (displayProviders)
            {
                BBGuiHelper.BeginIndent();
                {
                    for (var i = 0; i < providerList.Count; i++)
                    {
                        Undo.RecordObject(providerList[i], "Provider");
                        if (providerList[i].ElementEditStatus == EditStatus.REMOVED)
                        {
                            editedList.Add(providerList[i]);
                            providerList.RemoveAt(i);
                            dirty = true;
                            i--;
                            continue;
                        }
                        providerList[i].OnGUI();
                        if (providerList[i].ElementEditStatus != EditStatus.NONE)
                        {
                            dirty = true;
                        }
                    }
                    if (GUILayout.Button("New Provider"))
                    {
                        providerList.Add(ManifestProvider.CreateInstance());
                    }
                }
                BBGuiHelper.EndIndent();
            }

            #endregion

            #region MetaData

            displayMetaData = EditorGUILayout.Foldout(displayMetaData, "Meta-Data: (" + metaDataList.Count + ")");
            if (displayMetaData)
            {
                BBGuiHelper.BeginIndent();
                {
                    for (var i = 0; i < metaDataList.Count; i++)
                    {
                        Undo.RecordObject(metaDataList[i], "Meta-Data");
                        if (metaDataList[i].ElementEditStatus == EditStatus.REMOVED)
                        {
                            editedList.Add(metaDataList[i]);
                            metaDataList.RemoveAt(i);
                            dirty = true;
                            i--;
                            continue;
                        }
                        metaDataList[i].OnGUI();
                        if (metaDataList[i].ElementEditStatus != EditStatus.NONE)
                        {
                            dirty = true;
                        }
                    }
                    if (GUILayout.Button("New Meta-Data"))
                    {
                        metaDataList.Add(ManifestMetaData.CreateInstance());
                    }
                }
                BBGuiHelper.EndIndent();
            }

            #endregion

            #region UsesLibrary

            displayUsesLibrary = EditorGUILayout.Foldout(displayUsesLibrary,
                "Uses-Library: (" + usesLibraryList.Count + ")");
            if (displayUsesLibrary)
            {
                BBGuiHelper.BeginIndent();
                {
                    for (var i = 0; i < usesLibraryList.Count; i++)
                    {
                        Undo.RecordObject(usesLibraryList[i], "Uses-Library");
                        if (usesLibraryList[i].ElementEditStatus == EditStatus.REMOVED)
                        {
                            editedList.Add(usesLibraryList[i]);
                            usesLibraryList.RemoveAt(i);
                            dirty = true;
                            i--;
                            continue;
                        }
                        usesLibraryList[i].OnGUI();
                        if (usesLibraryList[i].ElementEditStatus != EditStatus.NONE)
                        {
                            dirty = true;
                        }
                    }
                    if (GUILayout.Button("New Uses-Library"))
                    {
                        usesLibraryList.Add(ManifestUsesLibrary.CreateInstance());
                    }
                }
                BBGuiHelper.EndIndent();
            }

            #endregion

            EditorGUILayout.LabelField("Manifest Level:");

            #region Permissions

            displayPermissions = EditorGUILayout.Foldout(displayPermissions,
                "Permissions: (" + permissionList.Count + ")");
            if (displayPermissions)
            {
                BBGuiHelper.BeginIndent();
                {
                    for (var i = 0; i < permissionList.Count; i++)
                    {
                        Undo.RecordObject(permissionList[i], "Permission");
                        if (permissionList[i].ElementEditStatus == EditStatus.REMOVED)
                        {
                            editedList.Add(permissionList[i]);
                            permissionList.RemoveAt(i);
                            dirty = true;
                            i--;
                            continue;
                        }
                        permissionList[i].OnGUI();
                        if (permissionList[i].ElementEditStatus != EditStatus.NONE)
                        {
                            dirty = true;
                        }
                    }
                    if (GUILayout.Button("New Permission"))
                    {
                        permissionList.Add(ManifestPermission.CreateInstance());
                    }
                }
                BBGuiHelper.EndIndent();
            }

            #endregion

            #region UsesPermissions

            displayUsesPermissions = EditorGUILayout.Foldout(displayUsesPermissions,
                "Uses-Permissions: (" + usesPermissionList.Count + ")");
            if (displayUsesPermissions)
            {
                BBGuiHelper.BeginIndent();
                {
                    for (var i = 0; i < usesPermissionList.Count; i++)
                    {
                        Undo.RecordObject(usesPermissionList[i], "Uses-Permission");
                        if (usesPermissionList[i].ElementEditStatus == EditStatus.REMOVED)
                        {
                            editedList.Add(usesPermissionList[i]);
                            usesPermissionList.RemoveAt(i);
                            dirty = true;
                            i--;
                            continue;
                        }
                        usesPermissionList[i].OnGUI();
                        if (usesPermissionList[i].ElementEditStatus != EditStatus.NONE)
                        {
                            dirty = true;
                        }
                    }
                    if (GUILayout.Button("New Uses-Permission"))
                    {
                        usesPermissionList.Add(ManifestUsesPermission.CreateInstance());
                    }
                }
                BBGuiHelper.EndIndent();
            }

            #endregion

            #region UsesFeature

            displayUsesFeatures = EditorGUILayout.Foldout(displayUsesFeatures,
                "Uses-Features: (" + usesFeatureList.Count + ")");
            if (displayUsesFeatures)
            {
                BBGuiHelper.BeginIndent();
                {
                    for (var i = 0; i < usesFeatureList.Count; i++)
                    {
                        Undo.RecordObject(usesFeatureList[i], "Uses-Feature");
                        if (usesFeatureList[i].ElementEditStatus == EditStatus.REMOVED)
                        {
                            editedList.Add(usesFeatureList[i]);
                            usesFeatureList.RemoveAt(i);
                            dirty = true;
                            i--;
                            continue;
                        }
                        usesFeatureList[i].OnGUI();
                        if (usesFeatureList[i].ElementEditStatus != EditStatus.NONE)
                        {
                            dirty = true;
                        }
                    }
                    if (GUILayout.Button("New Uses-Feature"))
                    {
                        usesFeatureList.Add(ManifestUsesFeature.CreateInstance());
                    }
                }
                BBGuiHelper.EndIndent();
            }

            #endregion
        }

        public void Merge(AndroidWindowData data)
        {
            var applicationElement = document.GetElementsByTagName("application")[0] as XmlElement;
            var manifestElement = document.GetElementsByTagName("manifest")[0] as XmlElement;
            foreach (var activity in data.activityList)
            {
                activity.ElementEditStatus = EditStatus.EDITED;
                var element = document.ImportNode(activity.node, true) as XmlElement;
                applicationElement.AppendChild(element);
                var newActivity = ManifestActivity.CreateInstance(element);
                for (var i = 0; i < activityList.Count; i++)
                {
                    if (activityList[i].name.Equals(newActivity.name))
                    {
                        activityList[i].ElementEditStatus = EditStatus.REMOVED;
                        break;
                    }
                }
                activityList.Add(newActivity);
            }
            foreach (var receiver in data.receiverList)
            {
                receiver.ElementEditStatus = EditStatus.EDITED;
                var element = document.ImportNode(receiver.node, true) as XmlElement;
                applicationElement.AppendChild(element);
                var newReceiver = ManifestReceiver.CreateInstance(element);
                for (var i = 0; i < receiverList.Count; i++)
                {
                    if (receiverList[i].name.Equals(newReceiver.name))
                    {
                        receiverList[i].ElementEditStatus = EditStatus.REMOVED;
                        break;
                    }
                }
                receiverList.Add(newReceiver);
            }
            foreach (var provider in data.providerList)
            {
                provider.ElementEditStatus = EditStatus.EDITED;
                var element = document.ImportNode(provider.node, true) as XmlElement;
                applicationElement.AppendChild(element);
                var newProvier = ManifestProvider.CreateInstance(element);
                for (var i = 0; i < providerList.Count; i++)
                {
                    if (providerList[i].name.Equals(newProvier.name))
                    {
                        providerList[i].ElementEditStatus = EditStatus.REMOVED;
                        break;
                    }
                }
                providerList.Add(newProvier);
            }
            foreach (var service in data.serviceList)
            {
                service.ElementEditStatus = EditStatus.EDITED;
                var element = document.ImportNode(service.node, true) as XmlElement;
                applicationElement.AppendChild(element);
                var newService = ManifestService.CreateInstance(element);
                for (var i = 0; i < serviceList.Count; i++)
                {
                    if (serviceList[i].name.Equals(newService.name))
                    {
                        serviceList[i].ElementEditStatus = EditStatus.REMOVED;
                        break;
                    }
                }
                serviceList.Add(newService);
            }
            foreach (var metadata in data.metaDataList)
            {
                metadata.ElementEditStatus = EditStatus.EDITED;
                var element = document.ImportNode(metadata.node, true) as XmlElement;
                applicationElement.AppendChild(element);
                var newMetaData = ManifestMetaData.CreateInstance(element);
                for (var i = 0; i < metaDataList.Count; i++)
                {
                    if (metaDataList[i].name.Equals(newMetaData.name))
                    {
                        metaDataList[i].ElementEditStatus = EditStatus.REMOVED;
                        break;
                    }
                }
                metaDataList.Add(newMetaData);
            }
            foreach (var usesLibrary in data.usesLibraryList)
            {
                usesLibrary.ElementEditStatus = EditStatus.EDITED;
                var element = document.ImportNode(usesLibrary.node, true) as XmlElement;
                applicationElement.AppendChild(element);
                var newUsesLibrary = ManifestUsesLibrary.CreateInstance(element);
                for (var i = 0; i < usesLibraryList.Count; i ++)
                {
                    if (usesLibraryList[i].name.Equals(newUsesLibrary.name))
                    {
                        usesLibraryList[i].ElementEditStatus = EditStatus.REMOVED;
                        break;
                    }
                }
                usesLibraryList.Add(newUsesLibrary);
            }
            foreach (var permission in data.permissionList)
            {
                permission.ElementEditStatus = EditStatus.EDITED;
                var element = document.ImportNode(permission.node, true) as XmlElement;
                manifestElement.AppendChild(element);
                var newPermission = ManifestPermission.CreateInstance(element);
                for (var i = 0; i < permissionList.Count; i++)
                {
                    if (permissionList[i].name.Equals(permission.name))
                    {
                        permissionList[i].ElementEditStatus = EditStatus.REMOVED;
                        break;
                    }
                }
                permissionList.Add(newPermission);
            }
            foreach (var usesPermission in data.usesPermissionList)
            {
                usesPermission.ElementEditStatus = EditStatus.EDITED;
                var element = document.ImportNode(usesPermission.node, true) as XmlElement;
                manifestElement.AppendChild(element);
                var newUsesPermission = ManifestUsesPermission.CreateInstance(element);
                for (var i = 0; i < usesPermissionList.Count; i++)
                {
                    if (usesPermissionList[i].name.Equals(newUsesPermission.name))
                    {
                        usesPermissionList[i].ElementEditStatus = EditStatus.REMOVED;
                        break;
                    }
                }
                usesPermissionList.Add(newUsesPermission);
            }
            foreach (var usesFeature in data.usesFeatureList)
            {
                usesFeature.ElementEditStatus = EditStatus.EDITED;
                var element = document.ImportNode(usesFeature.node, true) as XmlElement;
                manifestElement.AppendChild(element);
                var newUsesFeature = ManifestUsesFeature.CreateInstance(element);
                for (var i = 0; i < usesFeatureList.Count; i++)
                {
                    if (usesFeatureList[i].name.Equals(newUsesFeature.name))
                    {
                        usesFeatureList[i].ElementEditStatus = EditStatus.REMOVED;
                        break;
                    }
                }
                usesFeatureList.Add(newUsesFeature);
            }
        }

        public override string ToString()
        {
            return name + xmlEditor;
        }

        public void ApplyChanges()
        {
            if (application.ElementEditStatus != EditStatus.NONE)
                editedList.Add(application);
            foreach (var activity in activityList)
            {
                if (activity.ElementEditStatus != EditStatus.NONE)
                {
                    editedList.Add(activity);
                }
            }
            foreach (var receiver in receiverList)
            {
                if (receiver.ElementEditStatus != EditStatus.NONE)
                {
                    editedList.Add(receiver);
                }
            }
            foreach (var service in serviceList)
            {
                if (service.ElementEditStatus != EditStatus.NONE)
                {
                    editedList.Add(service);
                }
            }
            foreach (var provider in providerList)
            {
                if (provider.ElementEditStatus != EditStatus.NONE)
                {
                    editedList.Add(provider);
                }
            }
            foreach (var metaData in metaDataList)
            {
                if (metaData.ElementEditStatus != EditStatus.NONE)
                {
                    editedList.Add(metaData);
                }
            }
            foreach (var usesLibrary in usesLibraryList)
            {
                if (usesLibrary.ElementEditStatus != EditStatus.NONE)
                {
                    editedList.Add(usesLibrary);
                }
            }
            foreach (var permission in permissionList)
            {
                if (permission.ElementEditStatus != EditStatus.NONE)
                {
                    editedList.Add(permission);
                }
            }
            foreach (var usesPermission in usesPermissionList)
            {
                if (usesPermission.ElementEditStatus != EditStatus.NONE)
                {
                    editedList.Add(usesPermission);
                }
            }
            foreach (var usesFeature in usesFeatureList)
            {
                if (usesFeature.ElementEditStatus != EditStatus.NONE)
                {
                    editedList.Add(usesFeature);
                }
            }
            xmlEditor.ApplyChanges(this);
            editedList.Clear();
            dirty = false;
        }
    }
}