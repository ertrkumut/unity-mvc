using System;
using System.IO;
using System.Linq;
using MVC.Runtime.Screen;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace MVC.Editor.CodeGenerator.Menus
{
    internal class CreateScreenMenu : CreateViewMenu
    {
        protected override string _classLabelName => "Screen Name: ";
        protected override string _classViewName => "ScreenView";
        protected override string _classMediatorName => "ScreenMediator";

        protected override string _namespace => "Runtime.Views.Screens.";

        protected override string _tempViewName => "TempScreenView";
        protected override string _tempMediatorName => "TempScreenMediator";

        protected override string _targetViewPath => CodeGeneratorStrings.ScreenPath;
        protected override string _tempViewPath => CodeGeneratorStrings.TempScreenViewPath;
        protected override string _tempMediatorPath => CodeGeneratorStrings.TempScreenMediatorPath;

        private string _fileName;
        
        protected override void CreateViewMediator()
        {
            base.CreateViewMediator();

            CreateScene();
        }

        protected override void CreateView(string path, string fileName, string namespaceText)
        {
            _fileName = fileName;
            base.CreateView(path, fileName, namespaceText);
        }

        private void CreateScene()
        {
            var scenePath = CodeGeneratorStrings.ScreenTestScenePath;
            
            PlayerPrefs.SetString("create-screen-menu-clicked", _fileName);
            PlayerPrefs.SetString("create-screen-scene-path", scenePath);

            if (!Directory.Exists(scenePath))
                Directory.CreateDirectory(scenePath);
            
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            scene.name = _fileName + "TestScene";
        }
        
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void CodeGenerationCompleted()
        {
            if(!PlayerPrefs.HasKey("create-screen-menu-clicked"))
                return;
            
            var screenName = PlayerPrefs.GetString("create-screen-menu-clicked");
            var path = PlayerPrefs.GetString("create-screen-scene-path") + screenName + ".unity";
            
            PlayerPrefs.DeleteKey("create-screen-menu-clicked");
            PlayerPrefs.DeleteKey("create-screen-scene-path");
            
            var assemblyList = AppDomain.CurrentDomain.GetAssemblies();
            var currentAssembly = assemblyList.FirstOrDefault(x => x.FullName.StartsWith("Assembly-CSharp"));
            var screenType = currentAssembly.GetTypes().FirstOrDefault(x => x.Name == screenName);

            var canvasGameObject = new GameObject("Canvas", typeof(Canvas));
            var canvasScaler = canvasGameObject.AddComponent<CanvasScaler>();
            canvasScaler.referenceResolution = new Vector2(1080, 1920);
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            canvasGameObject.AddComponent<GraphicRaycaster>();

            var screenManagerPrefab = AssetDatabase.LoadAssetAtPath<ScreenManager>(CodeGeneratorStrings.ScreenManagerPrefabPath);
            var screenManager = (ScreenManager) PrefabUtility.InstantiatePrefab(screenManagerPrefab, canvasGameObject.transform);

            var screenGameObject = new GameObject(screenName, typeof(RectTransform));
            screenGameObject.transform.SetParent(screenManager.ScreenLayerList[0].transform);
                
            screenGameObject.AddComponent(screenType);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
            AssetDatabase.Refresh();
        }
    }
}