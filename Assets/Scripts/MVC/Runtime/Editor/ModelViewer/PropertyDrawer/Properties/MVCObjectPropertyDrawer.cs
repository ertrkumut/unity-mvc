﻿using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MVC.Runtime.Editor.ModelViewer.PropertyDrawer.Properties
{
    public class MVCObjectPropertyDrawer : MVCPropertyDrawer<Object>
    {
        public MVCObjectPropertyDrawer(FieldInfo fieldInfo, object targetObject) : base(fieldInfo, targetObject)
        {
        }
        
        public override void OnDrawGUI()
        {
            base.OnDrawGUI();

            var propertyValue = GetPropertyValue();
            var newValue = EditorGUILayout.ObjectField(new GUIContent(_fieldName), propertyValue, _fieldInfo.FieldType, true);
            if(newValue != propertyValue)
                SetValue(newValue);
        }
    }
}