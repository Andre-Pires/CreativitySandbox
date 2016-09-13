using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace BuildBuddy
{
    public class XcodeSerializer : ScriptableObject
    {
        private static readonly string iOSDataName = "BuildBuddyiOS.xml";

        public static readonly string FRAMEWORK_PATH_TOKEN = "@FRAMEWORKS";
        public static readonly string PBXPROJ_TOKEN = "pbxproj";
        public static readonly string PLIST_TOKEN = "plist";
        public static readonly string HEADER_PATH_TOKEN = "headerpath";
        public static readonly string LIBRARY_PATH_TOKEN = "librarypath";

        private readonly string emptyImportInfo = "<xcodeproj>" +
                                                  "<pbxproj/>" +
                                                  "<plist/>" +
                                                  "</xcodeproj>";

        private readonly string defaultFlags = "-weak_framework CoreMotion -weak-lSystem";

        [SerializeField] private bool dirty;

        public bool display;
        private bool displayFiles;
        private bool displayHeaderPaths;
        private bool displayLibraryPaths;
        private bool displayPlistEditor;
        private bool displaySdkFiles;

        private XmlDocument document;
        [SerializeField] private bool editPlist;
        [SerializeField] public List<string> headerSearchPaths = new List<string>();
        [SerializeField] private string importGroup = "";
        public bool isTemplate;
        [SerializeField] public List<string> librarySearchPaths = new List<string>();

        [SerializeField] private string linkerFlags;
        private string path;

        [SerializeField] public List<PBXFile> pbxFileList = new List<PBXFile>();
        [SerializeField] private readonly HashSet<string> pbxFilePathSet = new HashSet<string>();
        [SerializeField] public List<PBXFile> pbxSdkFileList = new List<PBXFile>();
        [SerializeField] public List<PlistEntry> pListEntryList = new List<PlistEntry>();


        public static XcodeSerializer CreateInstance()
        {
            var serializer = CreateInstance<XcodeSerializer>();
            serializer.document = new XmlDocument();
            serializer.FindPath();
            if (File.Exists(serializer.path))
                serializer.document.Load(serializer.path);
            else
            {
                serializer.document.LoadXml(serializer.emptyImportInfo);
                serializer.document.Save(serializer.path);
            }
            serializer.Initialize();
            return serializer;
        }

        public static XcodeSerializer CreateInstance(string xml, bool isTemplate)
        {
            var serializer = CreateInstance<XcodeSerializer>();
            serializer.document = new XmlDocument();
            serializer.document.LoadXml(xml);
            serializer.Initialize();
            serializer.isTemplate = isTemplate;
            return serializer;
        }

        public void Merge(XcodeSerializer serializer)
        {
            for (var i = 0; i < serializer.pbxFileList.Count; i++)
            {
                pbxFileList.Add(serializer.pbxFileList[i].Clone());
            }
            for (var i = 0; i < serializer.pbxSdkFileList.Count; i++)
            {
                pbxSdkFileList.Add(serializer.pbxSdkFileList[i].Clone());
            }
            for (var i = 0; i < serializer.pListEntryList.Count; i++)
            {
                for (var j = 0; j < pListEntryList.Count; i++)
                {
                    if (pListEntryList[j].key.Equals(serializer.pListEntryList[i].key))
                    {
                        pListEntryList.RemoveAt(j);
                        Debug.Log("Using templated copy of plist entry " + serializer.pListEntryList[i].key);
                    }
                }
                pListEntryList.Add(serializer.pListEntryList[i].Clone());
            }
            headerSearchPaths.AddRange(serializer.headerSearchPaths);
            librarySearchPaths.AddRange(serializer.librarySearchPaths);
            foreach (var s in serializer.pbxFilePathSet)
            {
                pbxFilePathSet.Add(s);
            }
            pbxFileList.Sort();
            for (var i = 1; i < pbxFileList.Count; i++)
            {
                if (pbxFileList[i].absolutePath.Equals(pbxFileList[i - 1].absolutePath))
                {
                    pbxFileList.RemoveAt(i--);
                }
            }
            pbxSdkFileList.Sort();
            for (var i = 1; i < pbxSdkFileList.Count; i++)
            {
                if (pbxSdkFileList[i].absolutePath.Equals(pbxSdkFileList[i - 1].absolutePath))
                {
                    pbxSdkFileList.RemoveAt(i--);
                }
            }
            headerSearchPaths.Sort();
            for (var i = 1; i < headerSearchPaths.Count; i++)
            {
                if (headerSearchPaths[i].Equals(headerSearchPaths[i - 1]))
                {
                    headerSearchPaths.RemoveAt(i--);
                }
            }
            librarySearchPaths.Sort();
            for (var i = 1; i < librarySearchPaths.Count; i++)
            {
                if (librarySearchPaths[i].Equals(librarySearchPaths[i - 1]))
                {
                    librarySearchPaths.RemoveAt(i--);
                }
            }
            linkerFlags += " " + serializer.linkerFlags;
            var flagsList = linkerFlags.Split(new[] {" -"}, StringSplitOptions.None).ToList();
            flagsList.Sort();
            linkerFlags = flagsList.Count > 0 ? flagsList[0] : "";
            for (var i = 1; i < flagsList.Count; i++)
            {
                if (flagsList[i].Equals(flagsList[i - 1]))
                    flagsList.RemoveAt(i--);
                else
                    linkerFlags += " -" + flagsList[i];
            }
            dirty = true;
        }

        private void Initialize()
        {
            pbxFilePathSet.Clear();
            pbxSdkFileList.Clear();
            pbxFileList.Clear();
            headerSearchPaths.Clear();
            librarySearchPaths.Clear();
            pListEntryList.Clear();

            LoadPBXFiles();
            try
            {
                PbxSdkFile.LoadDeviceFrameworks();
            }
            catch
            {
                Debug.LogError("Couldn't Fid iOS SDK");
            }
            for (var i = 0; i < pbxFileList.Count; i++)
            {
                if (pbxFilePathSet.Contains(pbxFileList[i].absolutePath))
                {
                    pbxFileList.RemoveAt(i--);
                }
                else
                {
                    pbxFilePathSet.Add(pbxFileList[i].absolutePath);
                }
            }
            pListEntryList = LoadPListEntries();
            linkerFlags = LoadLinkerFlags();
            headerSearchPaths = LoadHeaderSearchPaths();
            librarySearchPaths = LoadLibrarySearchPaths();

            pbxSdkFileList.Sort();
            pbxFileList.Sort();
            headerSearchPaths.Sort();
            librarySearchPaths.Sort();

            dirty = false;
        }

        #region GUI

        public void OnGUI()
        {
            Undo.RecordObject(this, "Xcode Window");

            #region buttons

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add File"))
                {
                    var import = EditorUtility.OpenFilePanel("Include in Xcode project", Application.dataPath + "/..",
                        "");
                    if (!import.Equals(""))
                    {
                        if (pbxFilePathSet.Contains(import))
                        {
                            Debug.LogWarning("Already importing: " + import);
                        }
                        else
                        {
                            pbxFileList.Add(PBXFile.CreateInstance(import, importGroup));
                            pbxFilePathSet.Add(import);
                            dirty = true;
                        }
                    }
                    for (var i = 0; i < pbxFileList.Count; i++)
                    {
                        if (pbxFileList[i].removed)
                        {
                            pbxFileList.RemoveAt(i--);
                        }
                    }
                }
                if (GUILayout.Button("Add Folder"))
                {
                    var import = EditorUtility.OpenFolderPanel("Include folder in Xcode project",
                        Application.dataPath + "/..", "");
                    if (!import.Equals(""))
                    {
                        dirty = true;
                        if (Path.GetExtension(import).Equals(".framework"))
                        {
                            pbxFileList.Add(PBXFile.CreateInstance(import, importGroup));
                        }
                        else
                        {
                            var filePaths = Directory.GetFiles(import);
                            foreach (var path in filePaths)
                            {
                                if (pbxFilePathSet.Contains(path))
                                {
                                    Debug.LogWarning("Already importing: " + path);
                                }
                                else
                                {
                                    pbxFileList.Add(PBXFile.CreateInstance(path, importGroup));
                                    pbxFilePathSet.Add(path);
                                }
                            }
                            filePaths = Directory.GetDirectories(import, "*.*");
                            foreach (var path in filePaths)
                            {
                                if (pbxFilePathSet.Contains(path))
                                {
                                    Debug.LogWarning("Already importing: " + path);
                                }
                                else
                                {
                                    pbxFileList.Add(PBXFile.CreateInstance(path, importGroup));
                                    pbxFilePathSet.Add(path);
                                }
                            }
                        }
                    }
                    for (var i = 0; i < pbxFileList.Count; i++)
                    {
                        if (pbxFileList[i].removed)
                        {
                            pbxFileList.RemoveAt(i--);
                        }
                    }
                }
                if (GUILayout.Button("Add iOS SDK File"))
                {
                    pbxSdkFileList.Add(PbxSdkFile.CreateInstance());
                    displaySdkFiles = true;
                }
            }
            try
            {
                EditorGUILayout.EndHorizontal();
            }
            catch (InvalidOperationException)
            {
                Debug.Log("Caught invalidoperatinexception");
            }
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Header Search Path"))
                {
                    headerSearchPaths.Add("");
                    displayHeaderPaths = true;
                }
                if (GUILayout.Button("Library Search Path"))
                {
                    librarySearchPaths.Add("");
                    displayLibraryPaths = true;
                }
                if (GUILayout.Button("Plist Entry"))
                {
                    pListEntryList.Add(PlistEntry.CreateInstance());
                    displayPlistEditor = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            #endregion

            EditorGUILayout.Space();

            importGroup = EditorGUILayout.TextField("File Import Group: ", importGroup);
            EditorGUI.BeginChangeCheck();
            linkerFlags = EditorGUILayout.TextField("Linker Flags: ", linkerFlags);
            if (EditorGUI.EndChangeCheck())
                dirty = true;
            EditorGUILayout.Space();

            #region SDKFilesData

            displaySdkFiles = EditorGUILayout.Foldout(displaySdkFiles, "iOS SDK Files: (" + pbxSdkFileList.Count + ")");
            if (displaySdkFiles)
            {
                BBGuiHelper.BeginIndent();
                {
                    for (var i = 0; i < pbxSdkFileList.Count; i++)
                    {
                        if (pbxSdkFileList[i].removed)
                        {
                            pbxFilePathSet.Remove(pbxSdkFileList[i].absolutePath);
                            pbxSdkFileList.RemoveAt(i--);
                            dirty = true;
                            continue;
                        }
                        if (pbxSdkFileList[i] != null)
                        {
                            Undo.RecordObject(pbxSdkFileList[i], "Xcode File Import");
                        }
                        pbxSdkFileList[i].OnGUI();
                        if (pbxSdkFileList[i].edited)
                            dirty = true;
                    }
                }
                BBGuiHelper.EndIndent();
            }

            #endregion

            EditorGUILayout.Space();

            #region FilesData

            displayFiles = EditorGUILayout.Foldout(displayFiles, "Other Files: (" + pbxFileList.Count + ")");
            if (displayFiles)
            {
                BBGuiHelper.BeginIndent();
                {
                    var currentGroup = "";
                    BBGuiHelper.BeginIndent();
                    for (var i = 0; i < pbxFileList.Count; i++)
                    {
                        if (pbxFileList[i].removed)
                        {
                            pbxFilePathSet.Remove(pbxFileList[i].absolutePath);
                            pbxFileList.RemoveAt(i--);
                            dirty = true;
                            continue;
                        }
                        if (pbxFileList[i] != null)
                        {
                            Undo.RecordObject(pbxFileList[i], "Xcode File Import");
                        }
                        if (!pbxFileList[i].group.Equals(currentGroup))
                        {
                            BBGuiHelper.EndIndent();
                            EditorGUILayout.LabelField(pbxFileList[i].group);
                            currentGroup = pbxFileList[i].group;
                            BBGuiHelper.BeginIndent();
                        }
                        pbxFileList[i].OnGUI();
                        if (pbxFileList[i].edited)
                            dirty = true;
                    }
                    BBGuiHelper.EndIndent();
                }
                BBGuiHelper.EndIndent();
            }

            #endregion

            EditorGUILayout.Space();

            #region HeaderPaths

            displayHeaderPaths = EditorGUILayout.Foldout(displayHeaderPaths,
                "Header Search Paths: (" + headerSearchPaths.Count + ")");
            if (displayHeaderPaths)
            {
                BBGuiHelper.BeginIndent();
                {
                    EditorGUI.BeginChangeCheck();
                    for (var i = 0; i < headerSearchPaths.Count; i++)
                    {
                        headerSearchPaths[i] = EditorGUILayout.TextField(headerSearchPaths[i]);
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Set"))
                        {
                            headerSearchPaths[i] = EditorUtility.OpenFolderPanel("Set Header Search Path",
                                Application.dataPath + "/..", "");
                            headerSearchPaths[i] = MakePathRelative(headerSearchPaths[i]);
                        }
                        if (GUILayout.Button("Remove"))
                        {
                            headerSearchPaths.RemoveAt(i--);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (EditorGUI.EndChangeCheck())
                        dirty = true;
                }
                BBGuiHelper.EndIndent();
            }

            #endregion

            EditorGUILayout.Space();

            #region LibraryPaths

            displayLibraryPaths = EditorGUILayout.Foldout(displayLibraryPaths,
                "Library Search Paths: (" + librarySearchPaths.Count + ")");
            if (displayLibraryPaths)
            {
                BBGuiHelper.BeginIndent();
                {
                    EditorGUI.BeginChangeCheck();
                    for (var i = 0; i < librarySearchPaths.Count; i++)
                    {
                        librarySearchPaths[i] = EditorGUILayout.TextField(librarySearchPaths[i]);
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Set"))
                        {
                            librarySearchPaths[i] = EditorUtility.OpenFolderPanel("Set Library Search Path",
                                Application.dataPath + "/..", "");
                            librarySearchPaths[i] = MakePathRelative(librarySearchPaths[i]);
                        }
                        if (GUILayout.Button("Remove"))
                        {
                            librarySearchPaths.RemoveAt(i--);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (EditorGUI.EndChangeCheck())
                        dirty = true;
                }
                BBGuiHelper.EndIndent();
            }

            #endregion

            EditorGUILayout.Space();

            #region plistEditor

            displayPlistEditor = EditorGUILayout.Foldout(displayPlistEditor,
                "Info.plist Editor: (" + pListEntryList.Count + ")");
            if (displayPlistEditor)
            {
                for (var i = 0; i < pListEntryList.Count; i++)
                {
                    if (pListEntryList[i] != null)
                        Undo.RecordObject(pListEntryList[i], "Plist Entry");
                    pListEntryList[i].OnGUI();
                    if (pListEntryList[i].edited)
                        dirty = true;
                    if (pListEntryList[i].removed)
                    {
                        pListEntryList.RemoveAt(i--);
                        dirty = true;
                    }
                }
            }

            #endregion

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                if (dirty)
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.gray;
                }
                if (GUILayout.Button("Apply Changes"))
                {
                    ApplyChanges();
                }
                if (dirty)
                {
                    GUI.color = Color.red;
                }
                else
                {
                    GUI.color = Color.gray;
                }
                if (GUILayout.Button("Cancel Changes"))
                {
                    Initialize();
                }
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
        }

        #endregion

        private void ApplyChanges()
        {
            dirty = false;
            for (var i = 0; i < pbxFileList.Count; i++)
            {
                if (pbxFileList[i].removed)
                {
                    pbxFileList.RemoveAt(i--);
                }
                pbxFileList[i].edited = false;
            }
            for (var i = 0; i < pbxSdkFileList.Count; i++)
            {
                if (pbxSdkFileList[i].removed)
                {
                    pbxSdkFileList.RemoveAt(i--);
                    continue;
                }
                pbxSdkFileList[i].edited = false;
            }
            for (var i = 0; i < pListEntryList.Count; i++)
            {
                if (pListEntryList[i].removed)
                {
                    pListEntryList.RemoveAt(i--);
                    continue;
                }
                pListEntryList[i].edited = false;
            }
            var allFiles = new List<PBXFile>(pbxFileList);
            allFiles.AddRange(pbxSdkFileList);
            AddPBXFiles(allFiles);
            AddLinkerFlags(linkerFlags);
            AddPListEntries(pListEntryList);
            AddHeaderSearchPaths(headerSearchPaths);
            AddLibrarySearchPaths(librarySearchPaths);
            if (!isTemplate)
            {
                Save();
            }
            else
            {
                XcodeTemplateManager.SaveExistingTemplate(this);
            }
            pbxFileList.Sort();
            pbxSdkFileList.Sort();
            headerSearchPaths.Sort();
            librarySearchPaths.Sort();
        }

        private void FindPath()
        {
            try
            {
                path = Directory.GetFiles(Application.dataPath, iOSDataName, SearchOption.AllDirectories)[0];
            }
            catch (IndexOutOfRangeException)
            {
                Debug.LogWarning("Couldn't find " + iOSDataName + ". Creating it now in BuildBuddy directory");
                try
                {
                    path = Directory.GetDirectories(Application.dataPath, "BuildBuddy")[0] + "/" + iOSDataName;
                }
                catch (IndexOutOfRangeException)
                {
                    Debug.LogWarning("Couldn't find BuildBuddy directory, Creating " + iOSDataName + " in Assets folder");
                    path = Application.dataPath + "/" + iOSDataName;
                }
            }
        }

        public void AddPBXFiles(List<PBXFile> files)
        {
            var pbxprojElement = (XmlElement) document.GetElementsByTagName(PBXPROJ_TOKEN)[0];
            pbxprojElement.RemoveAll();
            foreach (var file in files)
            {
                file.Serialize(document);
            }
        }

        public void AddLinkerFlags(string flags)
        {
            var pbxprojElement = (XmlElement) document.GetElementsByTagName(PBXPROJ_TOKEN)[0];
            pbxprojElement.SetAttribute("linkerFlags", flags);
        }

        public void AddHeaderSearchPaths(IEnumerable<string> paths)
        {
            var parent = document.GetElementsByTagName(PBXPROJ_TOKEN)[0];
            foreach (var path in paths)
            {
                if (string.IsNullOrEmpty(path))
                    continue;
                var headerPathElement = document.CreateElement(HEADER_PATH_TOKEN);
                parent.AppendChild(headerPathElement);
                headerPathElement.SetAttribute("path", path);
            }
        }

        public void AddLibrarySearchPaths(IEnumerable<string> paths)
        {
            var parent = document.GetElementsByTagName(PBXPROJ_TOKEN)[0];
            foreach (var path in paths)
            {
                if (string.IsNullOrEmpty(path))
                    continue;
                var headerPathElement = document.CreateElement(LIBRARY_PATH_TOKEN);
                parent.AppendChild(headerPathElement);
                headerPathElement.SetAttribute("path", path);
            }
        }

        public void AddPListEntries(List<PlistEntry> entries)
        {
            var plistElement = (XmlElement) document.GetElementsByTagName(PLIST_TOKEN)[0];
            plistElement.RemoveAll();
            foreach (var entry in entries)
            {
                entry.Serialize(document, plistElement);
            }
        }

        public void Save()
        {
            FindPath();
            document.Save(path);
        }

        public List<PBXFile> LoadPBXFiles()
        {
            pbxFileList.Clear();
            pbxSdkFileList.Clear();
            foreach (XmlElement file in document.GetElementsByTagName("pbxfile"))
            {
                var filePath = file.Attributes["path"].Value;
                if (filePath.Equals(FRAMEWORK_PATH_TOKEN))
                {
                    pbxSdkFileList.Add(PbxSdkFile.CreateInstance(file));
                    continue;
                }
                pbxFileList.Add(PBXFile.CreateInstance(file));
            }
            var allFiles = new List<PBXFile>(pbxFileList);
            allFiles.AddRange(pbxSdkFileList);
            return allFiles;
        }

        public string LoadLinkerFlags()
        {
            var flags = "";
            try
            {
                flags = document.GetElementsByTagName(PBXPROJ_TOKEN)[0].Attributes["linkerFlags"].Value;
            }
            catch (NullReferenceException)
            {
                flags = defaultFlags;
            }
            return flags;
        }

        public List<PlistEntry> LoadPListEntries()
        {
            var entryList = new List<PlistEntry>();
            var plistElement = (XmlElement) document.GetElementsByTagName(PLIST_TOKEN)[0];
            foreach (XmlElement file in plistElement.ChildNodes)
            {
                entryList.Add(PlistEntry.CreateInstance(file));
            }
            return entryList;
        }

        public List<string> LoadHeaderSearchPaths(bool absolute = false)
        {
            var headerPaths = new List<string>();
            var headerNodes = document.GetElementsByTagName(HEADER_PATH_TOKEN);
            foreach (XmlElement node in headerNodes)
            {
                if (absolute)
                {
                    headerPaths.Add(BuildAbsolutePath(node.Attributes["path"].Value));
                }
                else
                {
                    headerPaths.Add(node.Attributes["path"].Value);
                }
            }
            return headerPaths;
        }

        public List<string> LoadLibrarySearchPaths(bool absolute = false)
        {
            var libraryPaths = new List<string>();
            var headerNodes = document.GetElementsByTagName(LIBRARY_PATH_TOKEN);
            foreach (XmlElement node in headerNodes)
            {
                if (absolute)
                {
                    libraryPaths.Add(BuildAbsolutePath(node.Attributes["path"].Value));
                }
                else
                {
                    libraryPaths.Add(node.Attributes["path"].Value);
                }
            }
            return libraryPaths;
        }

        public string MakePathRelative(string path)
        {
            return MakePathRelative(path, Application.dataPath);
        }

        private string MakePathRelative(string path, string relativeTo)
        {
            try
            {
                var fileUri = new Uri(path);
                var relativeUri = new Uri(relativeTo);
                var newPath = relativeUri.MakeRelativeUri(fileUri).ToString();
                if (newPath.StartsWith("Assets/"))
                {
                    return newPath.Substring(7);
                }
                if (string.IsNullOrEmpty(newPath))
                    return "";
                return "../" + newPath;
            }
            catch (UriFormatException)
            {
                return path;
            }
        }

        // absolutePath should equal Application.dataPath + relativePath
        // This correctly removes all instances of "../" from the middle of the absolute path
        public string BuildAbsolutePath(string relativePath)
        {
            var absolutePathArray = Application.dataPath.Split('/');
            var relativePathArray = relativePath.Split('/');
            var numToRemove = Array.FindAll(relativePathArray, s => s.Equals("..")).Length;
            var absolutePath = string.Join("/", absolutePathArray, 0, absolutePathArray.Length - numToRemove)
                               + "/" +
                               string.Join("/", relativePathArray, numToRemove, relativePathArray.Length - numToRemove);
            return absolutePath;
        }

        public override string ToString()
        {
            return name + document.OuterXml;
        }
    }
}