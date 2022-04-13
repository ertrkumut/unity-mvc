using System.Linq;
using UnityEditor;

namespace MVC.Screen.Editor
{
    [CustomEditor(typeof(ScreenManager), true)]
    [CanEditMultipleObjects]
    public class ScreenManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var allScreenManagers = FindObjectsOfType<ScreenManager>().ToList();
            foreach (var screenManager in allScreenManagers)
            {
                var managerIndex = screenManager.ManagerIndex;
                var sameIndexManagers = allScreenManagers
                    .Where(x => x.ManagerIndex == managerIndex)
                    .ToList();

                if (sameIndexManagers.Count != 1)
                {
                    EditorGUILayout.HelpBox("There is too many ScreenManagers with same Index!!", MessageType.Error);
                    break;
                }
            }
            
            base.OnInspectorGUI();
        }
    }
}