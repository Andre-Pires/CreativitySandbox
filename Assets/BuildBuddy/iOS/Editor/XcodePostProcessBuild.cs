using UnityEditor;
using UnityEditor.Callbacks;

namespace BuildBuddy
{
    public static class XcodePostProcessBuild
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToProject)
        {
            if (target != BuildTarget.iOS)
            {
            }
            else
            {
                var project = new XcodeProject(pathToProject);
                project.EditProject();
            }
        }
    }
}