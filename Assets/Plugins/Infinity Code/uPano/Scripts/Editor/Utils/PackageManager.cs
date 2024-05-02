/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Editors.Utils;
using UnityEditor;

namespace InfinityCode.uPano.Editors
{
    public class PackageManager
    {
        [MenuItem(EditorUtils.MENU_PATH + "Playmaker Integration Kit", false, 80)]
        public static void ImportPlayMakerIntegrationKit()
        {
            EditorUtils.ImportPackage("Packages/uPano-Playmaker-Integration-Kit.unitypackage",
                new Warning
                {
                    title = "Playmaker Integration Kit",
                    message = "You have Playmaker in your project?",
                    ok = "Yes, I have a Playmaker"
                },
                "Could not find Playmaker Integration Kit."
            );
        }
    }
}