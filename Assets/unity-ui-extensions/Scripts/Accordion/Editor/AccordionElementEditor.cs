﻿///Credit ChoMPHi
///Sourced from - http://forum.unity3d.com/threads/accordion-type-layout.271818/

using Assets.Scripts.Accordion;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(AccordionElement), true)]
    public class AccordionElementEditor : ToggleEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_MinHeight"));
            serializedObject.ApplyModifiedProperties();

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_IsOn"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Interactable"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}