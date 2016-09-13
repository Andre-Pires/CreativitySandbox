using System.Collections.Generic;
using UnityEditor;

namespace BuildBuddy
{
    public static class AndroidTemplateManager
    {
        private const string keyPrefix = "BBMANIFEST";
        private static List<AndroidWindowData> elements;

        public static void SaveTemplate(AndroidWindowData template)
        {
            template.isTemplate = true;
            for (var i = 0; i < elements.Count; i++)
            {
                if (elements[i].name.Equals(template.name))
                {
                    if (!EditorUtility.DisplayDialog("", "Save over existing template?", "Yes", "No"))
                        return;
                    elements[i] = template;
                    EditorPrefs.SetString(keyPrefix + i, template.ToString());
                    return;
                }
            }
            elements.Add(template);
            EditorPrefs.SetString(keyPrefix + (elements.Count - 1), template.ToString());
        }

        public static void SaveExistingTemplate(AndroidWindowData template)
        {
            var index = elements.IndexOf(template);
            if (index == -1)
            {
                return;
            }
            EditorPrefs.SetString(keyPrefix + index, template.ToString());
        }

        public static List<AndroidWindowData> GetTemplates()
        {
            elements = new List<AndroidWindowData>();
            var i = 0;
            while (EditorPrefs.HasKey(keyPrefix + i))
            {
                var savedPref = EditorPrefs.GetString(keyPrefix + i);
                var name = savedPref.Substring(0, savedPref.IndexOf('<'));
                ;
                var xml = savedPref.Substring(savedPref.IndexOf('<'));
                elements.Add(AndroidWindowData.CreateInstance(new AndroidXmlEditor(xml)));
                elements[i].name = name;
                elements[i].isTemplate = true;
                i++;
            }
            return elements;
        }

        public static AndroidWindowData ReloadTemplate(int i)
        {
            var savedPref = EditorPrefs.GetString(keyPrefix + i);
            var name = savedPref.Substring(0, savedPref.IndexOf('<'));
            ;
            var xml = savedPref.Substring(savedPref.IndexOf('<'));
            elements[i] = AndroidWindowData.CreateInstance(new AndroidXmlEditor(xml));
            elements[i].name = name;
            elements[i].isTemplate = true;
            return elements[i];
        }

        public static void DeleteTemplate(AndroidWindowData element)
        {
            var index = elements.IndexOf(element);
            DeleteTemplate(index);
        }

        private static void DeleteTemplate(int index)
        {
            var i = index;
            while (EditorPrefs.HasKey(keyPrefix + ++i))
            {
                EditorPrefs.SetString(keyPrefix + (i - 1), EditorPrefs.GetString(keyPrefix + i));
            }
            elements.RemoveAt(index);
            EditorPrefs.DeleteKey(keyPrefix + index);
        }
    }
}