using UnityEngine;

namespace BuildBuddy
{
    public static class BBGuiHelper
    {
        private static readonly float buttonWidth = 200;

        public static void BeginIndent(int indent = 12)
        {
            GUILayout.BeginHorizontal(); //GUI.skin.box
            GUILayout.Space(indent);
            GUILayout.BeginVertical();
        }

        public static void EndIndent()
        {
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        public static GUILayoutOption ButtonWidth()
        {
            return GUILayout.Width(buttonWidth);
        }
    }
}