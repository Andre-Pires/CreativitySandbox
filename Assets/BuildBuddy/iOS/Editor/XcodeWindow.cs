using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BuildBuddy
{
    [Serializable]
    public class XcodeWindow : EditorWindow
    {
        private Vector2 scrollPosition;

        private XcodeSerializer serializer;
        private string templateName;
        private List<XcodeSerializer> templates;

        [MenuItem("Window/BuildBuddy/Xcode Project Editor")]
        private static void ShowWindow()
        {
            GetWindow<XcodeWindow>(false, "Xcode Project Editor");
        }

        private void OnEnable()
        {
            serializer = XcodeSerializer.CreateInstance();
            templates = XcodeTemplateManager.GetTemplates();
        }

        private void OnDestroy()
        {
        }

        private void OnUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (serializer == null)
                OnEnable();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                serializer.OnGUI();
                EditorGUILayout.Space();
                templateName = EditorGUILayout.TextField("Template name: ", templateName);
                if (GUILayout.Button("Save as Template"))
                {
                    var templateSerializer = XcodeSerializer.CreateInstance(serializer.ToString(), true);
                    templateSerializer.name = templateName;
                    XcodeTemplateManager.SaveTemplate(templateSerializer);
                }
                EditorGUILayout.Space();
                for (var i = 0; i < templates.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        templates[i].display = EditorGUILayout.Foldout(templates[i].display, templates[i].name);
                        if (GUILayout.Button("Import", GUILayout.Width(50)))
                            serializer.Merge(templates[i]);
                        if (GUILayout.Button("Delete", GUILayout.Width(50)))
                        {
                            XcodeTemplateManager.DeleteTemplate(templates[i--]);
                            continue;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    BBGuiHelper.BeginIndent();
                    {
                        if (templates[i].display)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("Name: ", GUILayout.Width(75));
                                templates[i].name = EditorGUILayout.TextField(templates[i].name);
                            }
                            EditorGUILayout.EndHorizontal();
                            templates[i].OnGUI();
                        }
                    }
                    BBGuiHelper.EndIndent();
                }
            }
            EditorGUILayout.EndScrollView();
            //Repaint on Undo
            if (Event.current.type == EventType.ValidateCommand)
            {
                switch (Event.current.commandName)
                {
                    case "UndoRedoPerformed":
                        Repaint();
                        break;
                }
            }
        }
    }
}