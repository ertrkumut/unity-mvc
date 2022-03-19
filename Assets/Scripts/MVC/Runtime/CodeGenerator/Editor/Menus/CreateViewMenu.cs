﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MVC.Runtime.CodeGenerator.Editor.Menus
{
    public class CreateViewMenu : EditorWindow
    {
        private string _viewName = "*Name*";

        private List<string> _actionNames;

        private void OnEnable()
        {
            _actionNames = new List<string>();
        }

        private void OnGUI()
        {
            #region ViewName

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("View Name: ", GUILayout.Width(75));
            _viewName = EditorGUILayout.TextField(_viewName);
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Space(80);
            EditorGUILayout.LabelField(_viewName + "View", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            #endregion

            #region Actions

            EditorGUILayout.Space(25);
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Actions:", EditorStyles.boldLabel);

            GUI.backgroundColor = Color.green;
            var addActionButton = GUILayout.Button("Add Action");
            GUI.backgroundColor = Color.white;
            
            if(addActionButton)
                _actionNames.Add("OnActionName");

            for (var ii = 0; ii < _actionNames.Count; ii++)
            {
                EditorGUILayout.BeginHorizontal("box");

                _actionNames[ii] = EditorGUILayout.TextField(_actionNames[ii]);
                GUI.backgroundColor = Color.red;
                var removeButton = GUILayout.Button("-", GUILayout.Width(75));
                GUI.backgroundColor = Color.white;
                
                if (removeButton)
                {
                    _actionNames.RemoveAt(ii);
                    return;
                }
                
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            #endregion

            #region Create
            
            EditorGUILayout.Space(25);

            GUI.backgroundColor = Color.gray;
            var createButton = GUILayout.Button("Create");
            GUI.backgroundColor = Color.white;
            
            if(createButton)
                CreateViewMediator();

            #endregion
        }

        private void CreateViewMediator()
        {
            var path = Application.dataPath + "/Scripts/Views/" + _viewName;
            var viewName = _viewName.Split('/')[_viewName.Split('/').Length - 1] + "View";
            var mediatorName = _viewName.Split('/')[_viewName.Split('/').Length - 1] + "Mediator";
            
            var namespaceText = "Views." + _viewName.Replace("/",".");
            
            CreateView(path, viewName, namespaceText);
            CreateMediator(path, mediatorName, viewName, namespaceText);
        }

        private void CreateView(string path, string fileName, string namespaceText)
        {
            var newViewPath = path + "/" + fileName + ".cs";
            var tempViewClassPath =
                Application.dataPath + "/Scripts/MVC/Runtime/CodeGenerator/Editor/TempViews/TempView.cs";

            var tempViewContent = File.ReadAllLines(tempViewClassPath);
            var newViewContent = new List<string>();
            
            for (var ii = 0; ii < tempViewContent.Length; ii++)
            {
                var content = tempViewContent[ii];
                if (content.Contains("namespace "))
                {
                    content = "namespace " + namespaceText;
                }
                else if (content.Contains("internal class "))
                {
                    content = "\tpublic class " + fileName + " : MonoBehaviour, IView";
                }
                else if (content.Contains("//@Actions"))
                {
                    foreach (var actionName in _actionNames)
                    {
                        newViewContent.Add("\t\tpublic Action " + actionName + ";");
                    }
                    continue;
                }
                
                newViewContent.Add(content);
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            File.WriteAllLines(newViewPath, newViewContent.ToArray());
        }

        private void CreateMediator(string path, string fileName, string viewName, string namespaceText)
        {
            var newMediatorPath = path + "/" + fileName + ".cs";
            var tempMediatorClassPath =
                Application.dataPath + "/Scripts/MVC/Runtime/CodeGenerator/Editor/TempViews/TempMediator.cs";

            var tempMediatorContent = File.ReadAllLines(tempMediatorClassPath);
            var newMediatorContent = new List<string>();
            
            for (var ii = 0; ii < tempMediatorContent.Length; ii++)
            {
                var content = tempMediatorContent[ii];
                if (content.Contains("namespace "))
                {
                    content = "namespace " + namespaceText;
                }
                else if (content.Contains("internal class "))
                {
                    content = "\tpublic class " + fileName + " : IMediator";
                }
                else if (content.Contains("[Inject]"))
                {
                    content = "\t\t[Inject] private " + viewName + " _view { get; set; }";
                }
                else if (content.Contains("//@Register"))
                {
                    foreach (var actionName in _actionNames)
                    {
                        var line = "\t\t\t_view." + actionName + " += " + actionName + "Listener;";
                        newMediatorContent.Add(line);
                    }
                    continue;
                }
                else if (content.Contains("//@Remove"))
                {
                    foreach (var actionName in _actionNames)
                    {
                        var line = "\t\t\t_view." + actionName + " -= " + actionName + "Listener;";
                        newMediatorContent.Add(line);
                    }
                    continue;
                }
                else if (content.Contains("//@Methods"))
                {
                    foreach (var actionName in _actionNames)
                    {
                        var line = "\t\tprivate void " + actionName + "Listener()";
                        newMediatorContent.Add(line);
                        newMediatorContent.Add("\t\t{");
                        newMediatorContent.Add("\t\t}");
                        newMediatorContent.Add("");
                    }
                    newMediatorContent.RemoveAt(newMediatorContent.Count-1);
                    continue;
                }
                
                newMediatorContent.Add(content);
            }
            
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            File.WriteAllLines(newMediatorPath, newMediatorContent.ToArray());
            
            AssetDatabase.Refresh();
        }
    }
}