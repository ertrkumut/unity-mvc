#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MVC.Editor.CodeGenerator
{
    public static class AssemblyHelper
    {
        public static List<Type> GetAllTypesFromAssemblies()
        {
            var assemblyList = AppDomain.CurrentDomain.GetAssemblies();
            var mainAssembly = assemblyList.FirstOrDefault(x => x.FullName.StartsWith("Assembly-CSharp,"));
            var codeGenerationSettings = Resources.Load<MVCCodeGenerationSettings>("MVCCodeGenerationSettings");

            var result = new List<Type>();
            
            if(mainAssembly is not null)
                result.AddRange(mainAssembly.GetTypes());
            
            if (codeGenerationSettings != null)
            {
                foreach (var assemblyDefinitionAsset in codeGenerationSettings.AssemblyDefinitions)
                {
                    var assembly = assemblyList.FirstOrDefault(x => x.FullName.StartsWith(assemblyDefinitionAsset.name));
                    if(assembly == null)
                        continue;
                    
                    result.AddRange(assembly.GetTypes());
                }
            }

            return result;
        }
    }
}
#endif