using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BuildBuddy
{
    public class XcodeProject
    {
        private const string DEFAULT_FRAMEWORK_SEARCH_PATH = "\"$(inherited)\"";
        private readonly List<PBXFile> fileReferences;

        private readonly HashSet<string> frameworkSearchPaths = new HashSet<string>();
        private readonly List<string> headerPaths;
        private readonly List<string> libraryPaths;
        private readonly string linkerFlags = "";
        private readonly List<PlistEntry> plistEntries;
        private readonly string projectPath;

        public XcodeProject(string path)
        {
            projectPath = path;
            var serializer = XcodeSerializer.CreateInstance();
            fileReferences = new List<PBXFile>(serializer.LoadPBXFiles());
            linkerFlags = serializer.LoadLinkerFlags();
            plistEntries = serializer.LoadPListEntries();
            headerPaths = serializer.LoadHeaderSearchPaths();
            for (var i = 0; i < headerPaths.Count; i++)
            {
                headerPaths[i] = serializer.BuildAbsolutePath(headerPaths[i]);
            }
            libraryPaths = serializer.LoadLibrarySearchPaths();
            for (var i = 0; i < libraryPaths.Count; i++)
            {
                libraryPaths[i] = serializer.BuildAbsolutePath(libraryPaths[i]);
            }
        }

        public void EditProject()
        {
            foreach (var fileReference in fileReferences)
            {
                fileReference.MakePathRelative(projectPath);
                if (fileReference.isCustomFramework)
                {
                    frameworkSearchPaths.Add(Path.GetDirectoryName(fileReference.absolutePath));
                }
            }
            var editor = new PBXEditor(projectPath);
            var plistEditor = new PListEditor(projectPath);
            plistEditor.AddPListEntries(plistEntries);
            try
            {
                editor.AddFileReferences(fileReferences);
                editor.AddLinkerFlags(linkerFlags);
                editor.AddHeaderSearchPaths(headerPaths);
                editor.AddLibrarySearchPaths(libraryPaths);
                if (frameworkSearchPaths.Count > 0)
                {
                    frameworkSearchPaths.Add(DEFAULT_FRAMEWORK_SEARCH_PATH);
                    editor.AddFrameworkSearchPaths(frameworkSearchPaths);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.LogError("Another script has modified the Xcode project and BuildBuddy cannot run");
                return;
            }
            editor.Save();
        }
    }
}